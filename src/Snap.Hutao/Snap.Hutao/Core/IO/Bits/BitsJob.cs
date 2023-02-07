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
[SuppressMessage("", "SA1600")]
internal class BitsJob : DisposableObject, IBackgroundCopyCallback
{
    /// <summary>
    /// 任务名称前缀
    /// </summary>
    public const string JobNamePrefix = "SnapHutaoBitsJob";

    private const uint BitsEngineNoProgressTimeout = 30;
    private const int MaxResumeAttempts = 10;

    private readonly string displayName;
    private readonly ILogger<BitsJob> log;
    private readonly object lockObj = new();

    private IBackgroundCopyJob? nativeJob;
    private Exception? jobException;
    private BG_JOB_PROGRESS progress;
    private BG_JOB_STATE state;
    private bool isJobComplete;
    private int resumeAttempts;

    private BitsJob(IServiceProvider serviceProvider, string displayName, IBackgroundCopyJob job)
    {
        this.displayName = displayName;
        nativeJob = job;
        log = serviceProvider.GetRequiredService<ILogger<BitsJob>>();
    }

    public HRESULT ErrorCode { get; private set; }

    public static BitsJob CreateJob(IServiceProvider serviceProvider, IBackgroundCopyManager backgroundCopyManager, Uri uri, string filePath)
    {
        ILogger<BitsJob> service = serviceProvider.GetRequiredService<ILogger<BitsJob>>();
        string text = $"{JobNamePrefix} - {uri}";
        IBackgroundCopyJob ppJob;
        try
        {
            backgroundCopyManager.CreateJob(text, BG_JOB_TYPE.BG_JOB_TYPE_DOWNLOAD, out Guid _, out ppJob);

            // BG_NOTIFY_JOB_TRANSFERRED & BG_NOTIFY_JOB_ERROR & BG_NOTIFY_JOB_MODIFICATION
            ppJob.SetNotifyFlags(0b1011);
            ppJob.SetNoProgressTimeout(BitsEngineNoProgressTimeout);
            ppJob.SetPriority(BG_JOB_PRIORITY.BG_JOB_PRIORITY_FOREGROUND);
            ppJob.SetProxySettings(BG_JOB_PROXY_USAGE.BG_JOB_PROXY_USAGE_AUTODETECT, null, null);
        }
        catch (COMException ex)
        {
            service.LogInformation("Failed to create job. {message}", ex.Message);
            throw;
        }

        BitsJob bitsJob = new(serviceProvider, text, ppJob);
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
            log.LogInformation("Failed to job transfer: {message}", ex.Message);
        }
    }

    public void JobError(IBackgroundCopyJob job, IBackgroundCopyError error)
    {
        IBackgroundCopyError error2 = error;
        try
        {
            log.LogInformation("Failed job: {message}", displayName);
            UpdateJobState();
            BG_ERROR_CONTEXT errorContext = BG_ERROR_CONTEXT.BG_ERROR_CONTEXT_NONE;
            HRESULT returnCode = new(0);

            Invoke(() => error2.GetError(out errorContext, out returnCode), "GetError", throwOnFailure: false);
            ErrorCode = returnCode;
            jobException = new IOException(string.Format("Error context: {0}, Error code: {1}", errorContext, returnCode));
            CompleteOrCancel();
            log.LogInformation(jobException, "Job Exception:");
        }
        catch (Exception ex)
        {
            log?.LogInformation("Failed to handle job error: {message}", ex.Message);
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
                if (errorCode == -2145844944)
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

                log.LogInformation("Max resume attempts for job '{name}' exceeded. Canceling.", displayName);
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
            log.LogInformation(ex, "message");
        }
    }

    public void Cancel()
    {
        log.LogInformation("Canceling job {name}", displayName);
        lock (lockObj)
        {
            if (!isJobComplete)
            {
                Invoke(() => nativeJob?.Cancel(), "Bits Cancel");
                jobException = new OperationCanceledException();
                isJobComplete = true;
            }
        }
    }

    public void WaitForCompletion(Action<ProgressUpdateStatus> callback, CancellationToken cancellationToken)
    {
        CancellationTokenRegistration cancellationTokenRegistration = cancellationToken.Register(Cancel);
        int noProgressSeconds = 0;
        try
        {
            UpdateJobState();
            while (IsProgressingState(state) || state == BG_JOB_STATE.BG_JOB_STATE_QUEUED)
            {
                if (noProgressSeconds > BitsEngineNoProgressTimeout)
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
                    callback(new ProgressUpdateStatus((long)progress.BytesTransferred, (long)progress.BytesTotal));
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

        lock (lockObj)
        {
            if (isJobComplete)
            {
                return;
            }

            if (state == BG_JOB_STATE.BG_JOB_STATE_TRANSFERRED)
            {
                log.LogInformation("Completing job '{name}'.", displayName);
                Invoke(() => nativeJob?.Complete(), "Bits Complete");
                while (state == BG_JOB_STATE.BG_JOB_STATE_TRANSFERRED)
                {
                    Thread.Sleep(50);
                    UpdateJobState();
                }
            }
            else
            {
                log.LogInformation("Canceling job '{name}'.", displayName);
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
            Invoke(() => nativeJob?.GetProgress(out progress), "GetProgress");
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
            log.LogInformation("{name} failed. {exception}", displayName, ex);
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