// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using JetBrains.Annotations;
using System.Collections.Immutable;

namespace Snap.Hutao.Core.Collection.Generic;

internal sealed partial class TwoEnumerableEnumerator<TFirst, TSecond> : IDisposable
{
    private readonly IEnumerator<TFirst> firstEnumerator;
    private readonly IEnumerator<TSecond> secondEnumerator;

    public TwoEnumerableEnumerator(IEnumerable<TFirst> firstEnumerable, IEnumerable<TSecond> secondEnumerable)
    {
        firstEnumerator = GetNoThrowEnumeratorIfPossible(firstEnumerable);
        secondEnumerator = GetNoThrowEnumeratorIfPossible(secondEnumerable);
    }

    public (TFirst? First, TSecond? Second) Current { get => (firstEnumerator.Current, secondEnumerator.Current); }

    public bool MoveNext(ref bool moveFirst, ref bool moveSecond)
    {
        moveFirst = moveFirst && firstEnumerator.MoveNext();
        moveSecond = moveSecond && secondEnumerator.MoveNext();

        return moveFirst || moveSecond;
    }

    public void Dispose()
    {
        firstEnumerator.Dispose();
        secondEnumerator.Dispose();
    }

    [MustDisposeResource]
    private static IEnumerator<T> GetNoThrowEnumeratorIfPossible<T>(IEnumerable<T> enumerable)
    {
        if (enumerable is ImmutableArray<T> immutableArray)
        {
            return new ImmutableArrayEnumeratorNoThrow<T>(immutableArray);
        }

        return enumerable.GetEnumerator();
    }
}