// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Extension;

/// <summary>
/// 数高性能扩展
/// </summary>
[HighQuality]
internal static class NumberExtension
{
    /// <summary>
    /// 计算给定整数的位数
    /// </summary>
    /// <param name="x">给定的整数</param>
    /// <returns>位数</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Place(in this int x)
    {
        // Benchmarked and compared as a most optimized solution
        return (int)(MathF.Log10(x) + 1);
    }
}