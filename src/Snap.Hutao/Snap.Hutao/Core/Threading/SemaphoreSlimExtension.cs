// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;

namespace Snap.Hutao.Core.Threading;

/// <summary>
/// 信号量扩展
/// </summary>
internal static class SemaphoreSlimExtension
{
    /// <summary>
    /// 异步进入信号量
    /// </summary>
    /// <param name="semaphoreSlim">信号量</param>
    /// <param name="token">取消令牌</param>
    /// <returns>可释放的对象，用于释放信号量</returns>
    public static async ValueTask<IDisposable> EnterAsync(this SemaphoreSlim semaphoreSlim, CancellationToken token = default)
    {
        try
        {
            await semaphoreSlim.WaitAsync(token).ConfigureAwait(false);
        }
        catch (ObjectDisposedException ex)
        {
            ThrowHelper.OperationCanceled(SH.CoreThreadingSemaphoreSlimDisposed, ex);
        }

        return new SemaphoreSlimReleaser(semaphoreSlim);
    }
}

[SuppressMessage("", "SA1201")]
[SuppressMessage("", "SA1400")]
[SuppressMessage("", "SA1600")]
file readonly struct SemaphoreSlimReleaser : IDisposable
{
    private readonly SemaphoreSlim semaphoreSlim;

    public SemaphoreSlimReleaser(SemaphoreSlim semaphoreSlim)
    {
        this.semaphoreSlim = semaphoreSlim;
    }

    public void Dispose()
    {
        semaphoreSlim.Release();
    }
}
