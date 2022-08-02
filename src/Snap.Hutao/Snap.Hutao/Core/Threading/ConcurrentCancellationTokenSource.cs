// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Concurrent;

namespace Snap.Hutao.Core.Threading;

/// <summary>
/// 并发<see cref="CancellationTokenSource"/>
/// </summary>
/// <typeparam name="TItem">项类型</typeparam>
internal class ConcurrentCancellationTokenSource<TItem>
    where TItem : notnull
{
    private readonly ConcurrentDictionary<TItem, CancellationTokenSource> waitingItems = new();

    /// <summary>
    /// 为某个项注册取消令牌
    /// </summary>
    /// <param name="item">项</param>
    /// <returns>取消令牌</returns>
    public CancellationToken Register(TItem item)
    {
        if (waitingItems.TryRemove(item, out CancellationTokenSource? prevSource))
        {
            prevSource.Cancel();
        }

        CancellationTokenSource current = waitingItems.GetOrAdd(item, new CancellationTokenSource());

        return current.Token;
    }
}
