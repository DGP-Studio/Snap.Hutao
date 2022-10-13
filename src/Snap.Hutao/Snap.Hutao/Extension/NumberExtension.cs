// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Extension;

/// <summary>
/// 数高性能扩展
/// </summary>
public static class NumberExtension
{
    /// <summary>
    /// 获取从右向左某位上的数字
    /// </summary>
    /// <param name="x">源</param>
    /// <param name="place">位</param>
    /// <returns>数字</returns>
    public static int AtPlace(this int x, int place)
    {
        return (int)(x / Math.Pow(10, place - 1)) % 10;
    }

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