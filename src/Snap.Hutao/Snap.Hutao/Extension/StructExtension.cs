// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Graphics;

namespace Snap.Hutao.Extension;

/// <summary>
/// 结构扩展
/// </summary>
[HighQuality]
internal static class StructExtension
{
    /// <summary>
    /// 转换到宽高比
    /// </summary>
    /// <param name="size">尺寸</param>
    /// <returns>宽高比</returns>
    public static double AspectRatio(this Windows.Foundation.Size size)
    {
        return size.Width / size.Height;
    }

    /// <summary>
    /// 比例缩放
    /// </summary>
    /// <param name="rectInt32">源</param>
    /// <param name="scale">比例</param>
    /// <returns>结果</returns>
    public static RectInt32 Scale(this RectInt32 rectInt32, double scale)
    {
        return new((int)(rectInt32.X * scale), (int)(rectInt32.Y * scale), (int)(rectInt32.Width * scale), (int)(rectInt32.Height * scale));
    }

    /// <summary>
    /// 比例缩放
    /// </summary>
    /// <param name="sizeInt32">源</param>
    /// <param name="scale">比例</param>
    /// <returns>结果</returns>
    public static SizeInt32 Scale(this SizeInt32 sizeInt32, double scale)
    {
        return new((int)(sizeInt32.Width * scale), (int)(sizeInt32.Height * scale));
    }

    /// <summary>
    /// 尺寸
    /// </summary>
    /// <param name="rectInt32">源</param>
    /// <returns>结果</returns>
    public static int Size(this RectInt32 rectInt32)
    {
        return rectInt32.Width * rectInt32.Height;
    }

    /// <summary>
    /// 尺寸
    /// </summary>
    /// <param name="sizeInt32">源</param>
    /// <returns>结果</returns>
    public static int Size(this SizeInt32 sizeInt32)
    {
        return sizeInt32.Width * sizeInt32.Height;
    }
}