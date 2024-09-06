// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Extension;

internal static class NumberExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint StringLength(in this uint x)
    {
        // Benchmarked and compared as a most optimized solution
        return (uint)(MathF.Log10(x) + 1);
    }
}