// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using Windows.UI;

namespace Snap.Hutao.Control.Media;

/// <summary>
/// BGRA 结构
/// </summary>
[HighQuality]
internal struct Bgra32
{
    /// <summary>
    /// B
    /// </summary>
    public byte B;

    /// <summary>
    /// G
    /// </summary>
    public byte G;

    /// <summary>
    /// R
    /// </summary>
    public byte R;

    /// <summary>
    /// A
    /// </summary>
    public byte A;

    public Bgra32(byte b, byte g, byte r, byte a)
    {
        B = b;
        G = g;
        R = r;
        A = a;
    }

    public readonly double Luminance { get => ((0.299 * R) + (0.587 * G) + (0.114 * B)) / 255; }

    /// <summary>
    /// 从 Color 转换
    /// </summary>
    /// <param name="color">颜色</param>
    /// <returns>新的 BGRA8 结构</returns>
    public static unsafe implicit operator Bgra32(Color color)
    {
        Unsafe.SkipInit(out Bgra32 bgra8);
        *(uint*)&bgra8 = BinaryPrimitives.ReverseEndianness(*(uint*)&color);
        return bgra8;
    }

    public static unsafe implicit operator Color(Bgra32 bgra8)
    {
        Unsafe.SkipInit(out Color color);
        *(uint*)&color = BinaryPrimitives.ReverseEndianness(*(uint*)&bgra8);
        return color;
    }
}