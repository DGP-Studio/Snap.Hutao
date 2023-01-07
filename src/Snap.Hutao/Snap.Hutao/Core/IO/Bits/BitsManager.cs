// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Snap.Hutao.Core.IO;
using System.IO;
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
        bool result = await Task.Run(() => DownloadCore(uri, tempFile.Path, progress.Report, token), token).ConfigureAwait(false);
        return new(result, tempFile);
    }

    private bool DownloadCore(Uri uri, string tempFile, Action<ProgressUpdateStatus> progress, CancellationToken token)
    {
        IBackgroundCopyManager value;

        try
        {
            value = lazyBackgroundCopyManager.Value;
        }
        catch (System.Exception ex)
        {
            logger?.LogWarning("BITS download engine not supported: {message}", ex.Message);
            return false;
        }

        using (BitsJob bitsJob = BitsJob.CreateJob(serviceProvider, value, uri, tempFile))
        {
            try
            {
                bitsJob.WaitForCompletion(progress, token);
            }
            catch (System.Exception ex)
            {
                logger?.LogWarning(ex, "BITS download failed:");
                return false;
            }

            if (bitsJob.ErrorCode != 0)
            {
                return false;
            }
        }

        return true;
    }
}