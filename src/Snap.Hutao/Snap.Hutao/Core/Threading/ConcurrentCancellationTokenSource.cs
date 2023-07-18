// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

/// <summary>
/// 无区分项的并发<see cref="CancellationTokenSource"/>
/// </summary>
[HighQuality]
[SuppressMessage("", "CA1001")]
internal class ConcurrentCancellationTokenSource
{
    private CancellationTokenSource source = new();

    /// <summary>
    /// 注册取消令牌
    /// </summary>
    /// <returns>取消令牌</returns>
    public CancellationToken Register()
    {
        source.Cancel();
        source = new();
        return source.Token;
    }
}