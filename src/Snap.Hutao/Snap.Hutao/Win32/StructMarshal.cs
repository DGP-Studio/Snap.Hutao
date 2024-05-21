// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.Graphics;

namespace Snap.Hutao.Win32;

/// <summary>
/// 结构体封送
/// </summary>
[HighQuality]
internal static class StructMarshal
{
    /// <summary>
    /// 使用四字节颜色代码初始化一个新的颜色
    /// </summary>
    /// <param name="value">颜色代码</param>
    /// <returns>对应的颜色</returns>
    public static unsafe Windows.UI.Color Color(uint value)
    {
        Unsafe.SkipInit(out Windows.UI.Color color);
        *(uint*)&color = BinaryPrimitives.ReverseEndianness(value);
        return color;
    }

    public static Rect Rect(Vector2 size)
    {
        return new(0, 0, size.X, size.Y);
    }

    public static RECT RECT(RectInt32 rect)
    {
        return new(rect.X, rect.Y, rect.X + rect.Width, rect.Y + rect.Height);
    }

    public static RectInt32 RectInt32(RECT rect)
    {
        return new(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
    }

    public static RectInt32 RectInt32(SizeInt32 size)
    {
        return new(0, 0, size.Width, size.Height);
    }

    public static RectInt32 RectInt32(PointInt32 point, Vector2 size)
    {
        return RectInt32(point.X, point.Y, size);
    }

    public static RectInt32 RectInt32(int x, int y, Vector2 size)
    {
        return new(x, y, (int)size.X, (int)size.Y);
    }

    public static RectInt32 RectInt32(PointInt32 point, SizeInt32 size)
    {
        return new(point.X, point.Y, size.Width, size.Height);
    }

    public static SizeInt32 SizeInt32(RectInt32 rect)
    {
        return new(rect.Width, rect.Height);
    }
}