// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;
using Windows.UI;

namespace Snap.Hutao.Control.Image;

/// <summary>
/// BGRA8 结构
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct Bgra8
{
    /// <summary>
    /// B
    /// </summary>
    [FieldOffset(0)]
    public byte B;

    /// <summary>
    /// G
    /// </summary>
    [FieldOffset(1)]
    public byte G;

    /// <summary>
    /// R
    /// </summary>
    [FieldOffset(2)]
    public byte R;

    /// <summary>
    /// A
    /// </summary>
    [FieldOffset(3)]
    public byte A;

    [FieldOffset(0)]
    private readonly uint data;

    /// <summary>
    /// 构造一个新的 BGRA8 结构
    /// </summary>
    /// <param name="b">B</param>
    /// <param name="g">G</param>
    /// <param name="r">R</param>
    /// <param name="a">A</param>
    public Bgra8(byte b, byte g, byte r, byte a)
    {
        B = b;
        G = g;
        R = r;
        A = a;
    }

    /// <summary>
    ///  从Color值转换
    /// </summary>
    /// <param name="color">颜色</param>
    /// <returns>新的 BGRA8 结构</returns>
    public static Bgra8 FromColor(Color color)
    {
        return new(color.B, color.G, color.R, color.A);
    }

    /// <summary>
    /// 从RGB值转换
    /// </summary>
    /// <param name="r">R</param>
    /// <param name="g">G</param>
    /// <param name="b">B</param>
    /// <returns>新的 BGRA8 结构</returns>
    public static Bgra8 FromRgb(byte r, byte g, byte b)
    {
        return new(b, g, r, 0xFF);
    }
}