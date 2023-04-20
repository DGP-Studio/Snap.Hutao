// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using Windows.UI;

namespace Snap.Hutao.Control.Media;

/// <summary>
/// BGRA8 结构
/// </summary>
[HighQuality]
internal struct Bgra8
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

    /// <summary>
    /// 从 Color 转换
    /// </summary>
    /// <param name="color">颜色</param>
    /// <returns>新的 BGRA8 结构</returns>
    public static unsafe Bgra8 FromColor(Color color)
    {
        Unsafe.SkipInit(out Bgra8 bgra8);
        *(uint*)&bgra8 = BinaryPrimitives.ReverseEndianness(*(uint*)&color);
        return bgra8;
    }
}