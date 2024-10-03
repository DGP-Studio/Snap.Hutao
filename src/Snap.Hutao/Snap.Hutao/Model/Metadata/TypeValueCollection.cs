// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using System.Collections;

namespace Snap.Hutao.Model.Metadata;

internal sealed partial class TypeValueCollection<TType, TValue> : ICollection<TypeValue<TType, TValue>>
    where TType : notnull
{
    private readonly SortedDictionary<TType, TValue> inner = [];

    public bool IsReadOnly
    {
        get => false;
    }

    public int Count { get => inner.Count; }

    IEnumerator<TypeValue<TType, TValue>> IEnumerable<TypeValue<TType, TValue>>.GetEnumerator()
    {
        return new TypeValueEnumerator(inner.GetEnumerator());
    }

    public void Add(TypeValue<TType, TValue> item)
    {
        inner.Add(item.Type, item.Value);
    }

    public bool Contains(TypeValue<TType, TValue> item)
    {
        return inner.ContainsKey(item.Type) && EqualityComparer<TValue>.Default.Equals(inner[item.Type], item.Value);
    }

    public void CopyTo(TypeValue<TType, TValue>[] array, int arrayIndex)
    {
        HutaoException.NotSupported();
    }

    public bool Remove(TypeValue<TType, TValue> item)
    {
        return Contains(item) && inner.Remove(item.Type);
    }

    public void Clear()
    {
        inner.Clear();
    }

    public IEnumerator GetEnumerator()
    {
        return ((IEnumerable<TypeValue<TType, TValue>>)this).GetEnumerator();
    }

    public TValue? GetValueOrDefault(TType type)
    {
        return inner.GetValueOrDefault(type);
    }

    private struct TypeValueEnumerator : IEnumerator<TypeValue<TType, TValue>>
    {
        private SortedDictionary<TType, TValue>.Enumerator inner;

        internal TypeValueEnumerator(SortedDictionary<TType, TValue>.Enumerator inner)
        {
            this.inner = inner;
        }

        public TypeValue<TType, TValue> Current { get => new(inner.Current.Key, inner.Current.Value); }

        object? IEnumerator.Current { get => Current; }

        public bool MoveNext()
        {
            return inner.MoveNext();
        }

        public void Dispose()
        {
            inner.Dispose();
        }

        public void Reset()
        {
            ((IEnumerator)inner).Reset();
        }
    }
}