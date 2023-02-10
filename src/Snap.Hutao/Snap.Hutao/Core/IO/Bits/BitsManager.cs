// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Networking.BackgroundIntelligentTransferService;

namespace Snap.Hutao.Core.IO.Bits;

/// <summary>
/// BITS 管理器
/// </summary>
[Injection(InjectAs.Singleton)]
internal class BitsManager
{
    private readonly Lazy<IBackgroundCopyManager> lazyBackgroundCopyManager = new(() => (IBackgroundCopyManager)new BackgroundCopyManager());
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<BitsManager> logger;

    /// <summary>
    /// 构造一个新的 BITS 管理器
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public BitsManager(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
        logger = serviceProvider.GetRequiredService<ILogger<BitsManager>>();
    }

    /// <summary>
    /// 异步下载文件
    /// </summary>
    /// <param name="uri">文件uri</param>
    /// <param name="progress">进度</param>
    /// <param name="token">取消令牌</param>
    /// <returns>是否下载成功，以及创建的文件</returns>
    public async Task<ValueResult<bool, TempFile>> DownloadAsync(Uri uri, IProgress<ProgressUpdateStatus> progress, CancellationToken token = default)
    {
        TempFile tempFile = new(true);
        await ThreadHelper.SwitchToBackgroundAsync();
        bool result = DownloadCore(uri, tempFile.Path, progress, token);
        return new(result, tempFile);
    }

    /// <summary>
    /// 取消所有先前创建的任务
    /// </summary>
    public void CancelAllJobs()
    {
        IBackgroundCopyManager value;
        try
        {
            value = lazyBackgroundCopyManager.Value;
        }
        catch (Exception ex)
        {
            logger?.LogWarning("BITS download engine not supported: {message}", ex.Message);
            return;
        }

        value.EnumJobs(0, out IEnumBackgroundCopyJobs pJobs);
        pJobs.GetCount(out uint count);

        List<IBackgroundCopyJob> jobsToCancel = new();

        for (int i = 0; i < count; i++)
        {
            uint actualFetched = 0;
            pJobs.Next(1, out IBackgroundCopyJob pJob, ref actualFetched);
            pJob.GetDisplayName(out PWSTR name);
            if (name.AsSpan().StartsWith(BitsJob.JobNamePrefix))
            {
                jobsToCancel.Add(pJob);
            }
        }

        jobsToCancel.ForEach(job => job.Cancel());
    }

    private bool DownloadCore(Uri uri, string tempFile, IProgress<ProgressUpdateStatus> progress, CancellationToken token)
    {
        IBackgroundCopyManager value;

        try
        {
            value = lazyBackgroundCopyManager.Value;
        }
        catch (Exception ex)
        {
            logger?.LogWarning("BITS download engine not supported: {message}", ex.Message);
            return false;
        }

        try
        {
            using (BitsJob bitsJob = BitsJob.CreateJob(serviceProvider, value, uri, tempFile))
            {
                try
                {
                    bitsJob.WaitForCompletion(progress, token);
                }
                catch (Exception ex)
                {
                    logger?.LogWarning(ex, "BITS download failed:");
                    return false;
                }

                if (bitsJob.ErrorCode != 0)
                {
                    return false;
                }
            }
        }
        catch (COMException)
        {
            // BITS job creation failed
            return false;
        }

        return true;
    }
}