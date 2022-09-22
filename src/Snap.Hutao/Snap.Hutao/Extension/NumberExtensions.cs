// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Extension;

/// <summary>
/// 数高性能扩展
/// </summary>
public static class NumberExtensions
{
    /// <summary>
    /// 计算给定整数的位数
    /// </summary>
    /// <param name="x">给定的整数</param>
    /// <returns>位数</returns>
    public static int Place(this int x)
    {
        // Benchmarked and compared as a most optimized solution
        return (int)(MathF.Log10(x) + 1);
    }

    /// <summary>
    /// 计算给定整数的位数
    /// </summary>
    /// <param name="x">给定的整数</param>
    /// <returns>位数</returns>
    public static int Place(this long x)
    {
        // Benchmarked and compared as a most optimized solution
        return (int)(MathF.Log10(x) + 1);
    }
}