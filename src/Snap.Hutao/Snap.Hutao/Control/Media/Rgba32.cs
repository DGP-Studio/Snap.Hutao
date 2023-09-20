// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
// Some part of this file came from:
// https://github.com/xunkong/desktop/tree/main/src/Desktop/Desktop/Pages/CharacterInfoPage.xaml.cs

using System.Buffers.Binary;
using Windows.UI;

namespace Snap.Hutao.Control.Media;

/// <summary>
/// RGBA 颜色
/// </summary>
[HighQuality]
internal struct Rgba32
{
    /// <summary>
    /// R
    /// </summary>
    public byte R;

    /// <summary>
    /// G
    /// </summary>
    public byte G;

    /// <summary>
    /// B
    /// </summary>
    public byte B;

    /// <summary>
    /// A
    /// </summary>
    public byte A;

    /// <summary>
    /// 构造一个新的 RGBA8 颜色
    /// </summary>
    /// <param name="hex">色值字符串</param>
    public Rgba32(string hex)
        : this(hex.Length == 6 ? Convert.ToUInt32($"{hex}FF", 16) : Convert.ToUInt32(hex, 16))
    {
    }

    /// <summary>
    /// 使用 RGBA 代码初始化新的结构
    /// </summary>
    /// <param name="code">RGBA 代码</param>
    public unsafe Rgba32(uint code)
    {
        // RRGGBBAA -> AABBGGRR
        fixed (Rgba32* pSelf = &this)
        {
            *(uint*)pSelf = BinaryPrimitives.ReverseEndianness(code);
        }
    }

    private Rgba32(byte r, byte g, byte b, byte a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public static unsafe implicit operator Color(Rgba32 hexColor)
    {
        // AABBGGRR -> BBGGRRAA
        // AABBGGRR -> 000000AA
        uint a = (*(uint*)&hexColor) >> 24;

        // AABBGGRR -> BBGGRR00
        uint rgb = (*(uint*)&hexColor) << 8;

        // BBGGRR00 + 000000AA
        uint rgba = rgb + a;

        return *(Color*)&rgba;
    }

    /// <summary>
    /// 从 HSL 颜色转换
    /// </summary>
    /// <param name="hsl">HSL 颜色</param>
    /// <returns>RGBA8颜色</returns>
    public static Rgba32 FromHsl(Hsl32 hsl)
    {
        double chroma = (1 - Math.Abs((2 * hsl.L) - 1)) * hsl.S;
        double h1 = hsl.H / 60;
        double x = chroma * (1 - Math.Abs((h1 % 2) - 1));
        double m = hsl.L - (0.5 * chroma);
        double r1, g1, b1;

        if (h1 < 1)
        {
            r1 = chroma;
            g1 = x;
            b1 = 0;
        }
        else if (h1 < 2)
        {
            r1 = x;
            g1 = chroma;
            b1 = 0;
        }
        else if (h1 < 3)
        {
            r1 = 0;
            g1 = chroma;
            b1 = x;
        }
        else if (h1 < 4)
        {
            r1 = 0;
            g1 = x;
            b1 = chroma;
        }
        else if (h1 < 5)
        {
            r1 = x;
            g1 = 0;
            b1 = chroma;
        }
        else
        {
            r1 = chroma;
            g1 = 0;
            b1 = x;
        }

        byte r = (byte)(255 * (r1 + m));
        byte g = (byte)(255 * (g1 + m));
        byte b = (byte)(255 * (b1 + m));
        byte a = (byte)(255 * hsl.A);

        return new(r, g, b, a);
    }

    /// <summary>
    /// 转换到 HSL 颜色
    /// </summary>
    /// <returns>HSL 颜色</returns>
    public readonly Hsl32 ToHsl()
    {
        const double toDouble = 1.0 / 255;
        double r = toDouble * R;
        double g = toDouble * G;
        double b = toDouble * B;
        double max = Math.Max(Math.Max(r, g), b);
        double min = Math.Min(Math.Min(r, g), b);
        double chroma = max - min;
        double h1;

        if (chroma == 0)
        {
            h1 = 0;
        }
        else if (max == r)
        {
            // The % operator doesn't do proper modulo on negative
            // numbers, so we'll add 6 before using it
            h1 = (((g - b) / chroma) + 6) % 6;
        }
        else if (max == g)
        {
            h1 = 2 + ((b - r) / chroma);
        }
        else
        {
            h1 = 4 + ((r - g) / chroma);
        }

        double lightness = 0.5 * (max + min);
        double saturation = chroma == 0 ? 0 : chroma / (1 - Math.Abs((2 * lightness) - 1));

        Hsl32 ret;
        ret.H = 60 * h1;
        ret.S = saturation;
        ret.L = lightness;
        ret.A = toDouble * A;
        return ret;
    }
}