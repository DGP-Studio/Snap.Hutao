// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Buffers.Binary;
using Windows.UI;

namespace Snap.Hutao.UI;

internal static class ColorHelper
{
    public static unsafe Bgra32 ToBgra32(Color color)
    {
        uint value = BinaryPrimitives.ReverseEndianness(*(uint*)&color);
        return *(Bgra32*)&value;
    }

    public static unsafe Color ToColor(Bgra32 bgra32)
    {
        uint value = BinaryPrimitives.ReverseEndianness(*(uint*)&bgra32);
        return *(Color*)&value;
    }

    public static unsafe Color ToColor(Rgba32 rgba32)
    {
        // Goal : Rgba32:RRGGBBAA(0xAABBGGRR) -> Color: AARRGGBB(0xBBGGRRAA)
        // Step1: Rgba32:RRGGBBAA(0xAABBGGRR) -> UInt32:AA000000(0x000000AA)
        uint a = *(uint*)&rgba32 >>> 24;

        // Step2: Rgba32:RRGGBBAA(0xAABBGGRR) -> UInt32:00RRGGBB(0xRRGGBB00)
        uint rgb = *(uint*)&rgba32 << 8;

        // Step2: UInt32:00RRGGBB(0xRRGGBB00) | UInt32:AA000000(0x000000AA) -> UInt32:AARRGGBB(0xRRGGBBAA)
        uint rgba = rgb | a;

        return *(Color*)&rgba;
    }

    public static unsafe Color ToColor(uint value)
    {
        uint reversed = BinaryPrimitives.ReverseEndianness(value);
        return *(Color*)&reversed;
    }

    public static Hsla256 ToHsla32(Rgba32 rgba32)
    {
        const double ToDouble = 1.0 / 255;

        double r = ToDouble * rgba32.R;
        double g = ToDouble * rgba32.G;
        double b = ToDouble * rgba32.B;
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

        Hsla256 ret;
        ret.H = 60 * h1;
        ret.S = saturation;
        ret.L = lightness;
        ret.A = ToDouble * rgba32.A;
        return ret;
    }

    public static Rgba32 ToRgba32(Hsla256 hsla256)
    {
        double chroma = (1 - Math.Abs((2 * hsla256.L) - 1)) * hsla256.S;
        double h1 = hsla256.H / 60;
        double x = chroma * (1 - Math.Abs((h1 % 2) - 1));
        double m = hsla256.L - (0.5 * chroma);
        (double r1, double g1, double b1) = ((double, double, double))(h1 switch
        {
            < 1 => (chroma, x, 0),
            < 2 => (x, chroma, 0),
            < 3 => (0, chroma, x),
            < 4 => (0, x, chroma),
            < 5 => (x, 0, chroma),
            _ => (chroma, 0, x),
        });

        byte r = (byte)(255 * (r1 + m));
        byte g = (byte)(255 * (g1 + m));
        byte b = (byte)(255 * (b1 + m));
        byte a = (byte)(255 * hsla256.A);

        return new Rgba32(r, g, b, a);
    }
}