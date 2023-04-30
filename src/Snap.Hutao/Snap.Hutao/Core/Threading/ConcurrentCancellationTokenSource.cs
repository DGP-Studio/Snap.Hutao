// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Concurrent;

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
    public CancellationToken CancelPreviousOne()
    {
        source.Cancel();
        source = new();
        return source.Token;
    }
}

/// <summary>
/// 有区分项的并发<see cref="CancellationTokenSource"/>
/// </summary>
/// <typeparam name="TItem">项类型</typeparam>
[HighQuality]
[SuppressMessage("", "SA1402")]
internal class ConcurrentCancellationTokenSource<TItem>
    where TItem : notnull
{
    private readonly ConcurrentDictionary<TItem, CancellationTokenSource> waitingItems = new();

    /// <summary>
    /// 为某个项注册取消令牌
    /// </summary>
    /// <param name="item">区分项</param>
    /// <returns>取消令牌</returns>
    public CancellationToken Register(TItem item)
    {
        if (waitingItems.TryRemove(item, out CancellationTokenSource? previousSource))
        {
            previousSource.Cancel();
        }

        return waitingItems.GetOrAdd(item, new CancellationTokenSource()).Token;
    }
}