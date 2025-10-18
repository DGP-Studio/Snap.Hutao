// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Service.Cultivation.Offline;

internal static class QualityIndexedArray
{
    public static readonly int EnumCount = Enum.GetValues<QualityType>().Length;

    public static QualityIndexedImmutableArray<T> Create<T>(ReadOnlySpan<T> span)
    {
        if (span.Length != EnumCount)
        {
            // ReSharper disable once LocalizableElement
            throw new ArgumentException($"The length of the array must be {EnumCount}.", nameof(span));
        }

        return new(span.ToArray());
    }
}

[CollectionBuilder(typeof(QualityIndexedArray), nameof(QualityIndexedArray.Create))]
internal sealed class QualityIndexedImmutableArray<T>
{
    private readonly T[] array;

    public QualityIndexedImmutableArray(T[] array)
    {
        this.array = array;
    }

    public T this[QualityType quality]
    {
        get => array[(int)quality];
    }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (T item in array)
        {
            yield return item;
        }
    }
}