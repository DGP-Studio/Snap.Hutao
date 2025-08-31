// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Concurrent;

namespace Snap.Hutao.Service.Hutao;

internal abstract class StatisticsCache
{
    private readonly ConcurrentDictionary<Type, TaskCompletionSource> typeToTcs = [];

    protected static IEnumerable<TResult> CurrentLeftJoinLast<TElement, TKey, TResult>(IEnumerable<TElement> current, IEnumerable<TElement>? last, Func<TElement, TKey> keySelector, Func<TElement, TElement?, TResult> resultSelector)
        where TKey : notnull
    {
        if (last is null)
        {
            foreach (TElement element in current)
            {
                yield return resultSelector(element, default);
            }
        }
        else
        {
            Dictionary<TKey, TElement> lastMap = [];
            foreach (TElement element in last)
            {
                lastMap[keySelector(element)] = element;
            }

            foreach (TElement element in current)
            {
                yield return resultSelector(element, lastMap.GetValueOrDefault(keySelector(element)));
            }
        }
    }

    protected async ValueTask InitializeForTypeAsync<T, TContext>(TContext context, Func<TContext, Task> initialization)
    {
        if (typeToTcs.TryGetValue(typeof(T), out TaskCompletionSource? tcs))
        {
            await tcs.Task.ConfigureAwait(false);
            return;
        }

        tcs = new();
        if (typeToTcs.TryAdd(typeof(T), tcs))
        {
            await initialization(context).ConfigureAwait(false);
            tcs.TrySetResult();
        }
    }
}