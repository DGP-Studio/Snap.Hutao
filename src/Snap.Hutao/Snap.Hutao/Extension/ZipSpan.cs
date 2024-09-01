// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Extension;

internal readonly ref struct ZipSpan<TFirst, TSecond>
{
    private readonly Span<TFirst> first;
    private readonly Span<TSecond> second;

    public ZipSpan(Span<TFirst> first, Span<TSecond> second)
    {
        this.first = first;
        this.second = second;
    }

    public Enumerator GetEnumerator()
    {
        return new Enumerator(first, second);
    }

    internal ref struct Enumerator
    {
        private readonly Span<TFirst> first;
        private readonly Span<TSecond> second;
        private int index = -1;

        public Enumerator(Span<TFirst> first, Span<TSecond> second)
        {
            this.first = first;
            this.second = second;
        }

        public RefTuple<TFirst, TSecond> Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => new(ref first[index], ref second[index]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            int index = this.index + 1;
            if (index < first.Length)
            {
                this.index = index;
                return true;
            }

            return false;
        }
    }
}