// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Snap.Hutao.Core.Abstraction;
using System.IO;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Networking.BackgroundIntelligentTransferService;

namespace Snap.Hutao.Core.IO.Bits;

/// <summary>
/// BITS Job
/// </summary>
[HighQuality]
[SuppressMessage("", "SA1600")]
internal sealed class BitsJob : DisposableObject, IBackgroundCopyCallback
{
    /// <summary>
    /// 任务名称前缀
    /// </summary>
    public const string JobNamePrefix = "SnapHutaoBitsJob";

    private const uint Timeout = 29;
    private const int MaxResumeAttempts = 3;

    private readonly string displayName;
    private readonly ILogger<BitsJob> logger;
    private readonly object syncRoot = new();

    private IBackgroundCopyJob? nativeJob;
    private Exception? jobException;
    private BG_JOB_PROGRESS jobProgress;
    private BG_JOB_STATE state;
    private bool isJobComplete;
    private int resumeAttempts;

    private BitsJob(IServiceProvider serviceProvider, string displayName, IBackgroundCopyJob job)
    {
        this.displayName = displayName;
        nativeJob = job;
        logger = serviceProvider.GetRequiredService<ILogger<BitsJob>>();
    }

    public HRESULT ErrorCode { get; private set; }

    public static BitsJob CreateJob(IServiceProvider serviceProvider, IBackgroundCopyManager backgroundCopyManager, Uri uri, string filePath)
    {
        ILogger<BitsJob> logger = serviceProvider.GetRequiredService<ILogger<BitsJob>>();
        string jobName = $"{JobNamePrefix} - {uri}";
        IBackgroundCopyJob job;
        try
        {
            backgroundCopyManager.CreateJob(jobName, BG_JOB_TYPE.BG_JOB_TYPE_DOWNLOAD, out Guid _, out job);

            // BG_NOTIFY_JOB_TRANSFERRED & BG_NOTIFY_JOB_ERROR & BG_NOTIFY_JOB_MODIFICATION
            job.SetNotifyFlags(0B1011);
            job.SetNoProgressTimeout(Timeout);
            job.SetPriority(BG_JOB_PRIORITY.BG_JOB_PRIORITY_FOREGROUND);
            job.SetProxySettings(BG_JOB_PROXY_USAGE.BG_JOB_PROXY_USAGE_AUTODETECT, default, default);
        }
        catch (COMException ex)
        {
            logger.LogInformation("Failed to create job. {message}", ex.Message);
            throw;
        }

        BitsJob bitsJob = new(serviceProvider, jobName, job);
        bitsJob.InitJob(uri.AbsoluteUri, filePath);
        return bitsJob;
    }

    public void JobTransferred(IBackgroundCopyJob job)
    {
        try
        {
            UpdateProgress();
            UpdateJobState();
            CompleteOrCancel();
        }
        catch (Exception ex)
        {
            logger.LogInformation("Failed to job transfer: {message}", ex.Message);
        }
    }

    public void JobError(IBackgroundCopyJob job, IBackgroundCopyError error)
    {
        IBackgroundCopyError error2 = error;
        try
        {
            logger.LogInformation("Failed job: {message}", displayName);
            UpdateJobState();
            BG_ERROR_CONTEXT errorContext = BG_ERROR_CONTEXT.BG_ERROR_CONTEXT_NONE;
            HRESULT returnCode = new(0);

            Invoke(() => error2.GetError(out errorContext, out returnCode), "GetError", throwOnFailure: false);
            ErrorCode = returnCode;
            jobException = new IOException(string.Format("Error context: {0}, Error code: {1}", errorContext, returnCode));
            CompleteOrCancel();
            logger.LogInformation(jobException, "Job Exception:");
        }
        catch (Exception ex)
        {
            logger?.LogInformation("Failed to handle job error: {message}", ex.Message);
        }
    }

    public void JobModification(IBackgroundCopyJob job, uint reserved)
    {
        try
        {
            UpdateJobState();
            if (state == BG_JOB_STATE.BG_JOB_STATE_TRANSIENT_ERROR)
            {
                HRESULT errorCode = GetErrorCode(job);

                // BG_E_HTTP_ERROR_304
                if (errorCode == 0x80190130)
                {
                    ErrorCode = errorCode;
                    CompleteOrCancel();
                    return;
                }

                resumeAttempts++;
                if (resumeAttempts <= MaxResumeAttempts)
                {
                    Resume();
                    return;
                }

                logger.LogInformation("Max resume attempts for job '{name}' exceeded. Canceling.", displayName);
                CompleteOrCancel();
            }
            else if (IsProgressingState(state))
            {
                UpdateProgress();
            }
            else if (state == BG_JOB_STATE.BG_JOB_STATE_CANCELLED || state == BG_JOB_STATE.BG_JOB_STATE_ERROR)
            {
                CompleteOrCancel();
            }
        }
        catch (Exception ex)
        {
            logger.LogInformation(ex, "message");
        }
    }

    public void Cancel()
    {
        logger.LogInformation("Canceling job {name}", displayName);
        lock (syncRoot)
        {
            if (!isJobComplete)
            {
                Invoke(() => nativeJob?.Cancel(), "Bits Cancel");
                jobException = new OperationCanceledException();
                isJobComplete = true;
            }
        }
    }

    public void WaitForCompletion(IProgress<ProgressUpdateStatus> progress, CancellationToken cancellationToken)
    {
        CancellationTokenRegistration cancellationTokenRegistration = cancellationToken.Register(Cancel);
        int noProgressSeconds = 0;
        int noTransferSeconds = 0;
        ulong previousTransferred = 0;
        try
        {
            UpdateJobState();
            while (IsProgressingState(state) || state == BG_JOB_STATE.BG_JOB_STATE_QUEUED)
            {
                if (noProgressSeconds > Timeout || noTransferSeconds > Timeout)
                {
                    jobException = new TimeoutException($"Timeout reached for job {displayName} whilst in state {state}");
                    break;
                }

                cancellationToken.ThrowIfCancellationRequested();
                UpdateJobState();
                UpdateProgress();

                if (state is BG_JOB_STATE.BG_JOB_STATE_TRANSFERRING or BG_JOB_STATE.BG_JOB_STATE_TRANSFERRED or BG_JOB_STATE.BG_JOB_STATE_ACKNOWLEDGED)
                {
                    noProgressSeconds = 0;
                    if (jobProgress.BytesTransferred > previousTransferred)
                    {
                        previousTransferred = jobProgress.BytesTransferred;
                        noTransferSeconds = 0;
                        logger.LogInformation("{job}: {read} / {total}", displayName, jobProgress.BytesTransferred, jobProgress.BytesTotal);
                        progress.Report(new ProgressUpdateStatus((long)jobProgress.BytesTransferred, (long)jobProgress.BytesTotal));
                    }
                    else
                    {
                        ++noTransferSeconds;
                        logger.LogInformation("{job} no transfer for {x} seconds", displayName, noTransferSeconds);
                    }
                }

                // Refresh every seconds.
                Thread.Sleep(1000);
                ++noProgressSeconds;
            }
        }
        finally
        {
            cancellationTokenRegistration.Dispose();
            CompleteOrCancel();
        }

        if (jobException != null)
        {
            throw jobException;
        }
    }

    protected override void Dispose(bool isDisposing)
    {
        UpdateJobState();
        CompleteOrCancel();
        nativeJob = null;

        base.Dispose(isDisposing);
    }

    private static bool IsProgressingState(BG_JOB_STATE state)
    {
        if (state != BG_JOB_STATE.BG_JOB_STATE_CONNECTING && state != BG_JOB_STATE.BG_JOB_STATE_TRANSIENT_ERROR)
        {
            return state == BG_JOB_STATE.BG_JOB_STATE_TRANSFERRING;
        }

        return true;
    }

    private void CompleteOrCancel()
    {
        if (isJobComplete)
        {
            return;
        }

        lock (syncRoot)
        {
            if (isJobComplete)
            {
                return;
            }

            if (state == BG_JOB_STATE.BG_JOB_STATE_TRANSFERRED)
            {
                logger.LogInformation("Completing job '{name}'.", displayName);
                Invoke(() => nativeJob?.Complete(), "Bits Complete");
                while (state == BG_JOB_STATE.BG_JOB_STATE_TRANSFERRED)
                {
                    Thread.Sleep(50);
                    UpdateJobState();
                }
            }
            else
            {
                logger.LogInformation("Canceling job '{name}'.", displayName);
                Invoke(() => nativeJob?.Cancel(), "Bits Cancel");
            }

            isJobComplete = true;
        }
    }

    private void UpdateJobState()
    {
        if (nativeJob is IBackgroundCopyJob job)
        {
            Invoke(() => job.GetState(out state), "GetState");
        }
    }

    private void UpdateProgress()
    {
        if (!isJobComplete)
        {
            Invoke(() => nativeJob?.GetProgress(out jobProgress), "GetProgress");
        }
    }

    private void Resume()
    {
        Invoke(() => nativeJob?.Resume(), "Bits Resume");
    }

    private void Invoke(Action action, string displayName, bool throwOnFailure = true)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            logger.LogInformation("{name} failed. {exception}", displayName, ex);
            if (throwOnFailure)
            {
                throw;
            }
        }
    }

    private void InitJob(string remoteUrl, string filePath)
    {
        nativeJob?.AddFile(remoteUrl, filePath);
        nativeJob?.SetNotifyInterface(this);
        Resume();
    }

    private HRESULT GetErrorCode(IBackgroundCopyJob job)
    {
        IBackgroundCopyJob job2 = job;
        IBackgroundCopyError? error = null;

        Invoke(() => job2.GetError(out error), "GetError", false);
        if (error != null)
        {
            HRESULT returnCode = new(0);
            Invoke(() => error.GetError(out _, out returnCode), "GetError", false);
            return returnCode;
        }

        return new(0);
    }
}