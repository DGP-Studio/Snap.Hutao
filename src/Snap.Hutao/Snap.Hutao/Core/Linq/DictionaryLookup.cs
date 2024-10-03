// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections;

namespace Snap.Hutao.Core.Linq;

internal sealed partial class DictionaryLookup<TKey, TEnumerable, TValue> : ILookup<TKey, TValue>
    where TKey : notnull
    where TEnumerable : IEnumerable<TValue>
{
    private readonly Dictionary<TKey, TEnumerable> inner;

    public DictionaryLookup(Dictionary<TKey, TEnumerable> source)
    {
        inner = source;
    }

    public int Count { get => inner.Count; }

    public IEnumerable<TValue> this[TKey key] { get => inner[key]; }

    public bool Contains(TKey key)
    {
        return inner.ContainsKey(key);
    }

    public IEnumerator<IGrouping<TKey, TValue>> GetEnumerator()
    {
        foreach ((TKey key, TEnumerable values) in inner)
        {
            yield return new Grouping(key, values);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    internal sealed class Grouping : IGrouping<TKey, TValue>
    {
        private readonly TEnumerable enumerable;

        public Grouping(TKey key, TEnumerable enumerable)
        {
            Key = key;
            this.enumerable = enumerable;
        }

        public TKey Key { get; }

        public IEnumerator<TValue> GetEnumerator()
        {
            return enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}