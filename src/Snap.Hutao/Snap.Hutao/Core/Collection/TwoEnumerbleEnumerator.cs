// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Collection;

internal sealed partial class TwoEnumerbleEnumerator<TFirst, TSecond> : IDisposable
{
    private readonly IEnumerator<TFirst> firstEnumerator;
    private readonly IEnumerator<TSecond> secondEnumerator;

    public TwoEnumerbleEnumerator(IEnumerable<TFirst> firstEnumerable, IEnumerable<TSecond> secondEnumerable)
    {
        firstEnumerator = firstEnumerable.GetEnumerator();
        secondEnumerator = secondEnumerable.GetEnumerator();
    }

    public (TFirst First, TSecond Second) Current { get => (firstEnumerator.Current, secondEnumerator.Current); }

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
}