// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.UI;

namespace Snap.Hutao.UI;

internal struct Bgra32
{
    public byte B;
    public byte G;
    public byte R;
    public byte A;

    public Bgra32(byte b, byte g, byte r, byte a)
    {
        B = b;
        G = g;
        R = r;
        A = a;
    }

    public readonly double Luminance { get => ((0.2126 * R) + (0.7152 * G) + (0.0722 * B)) / 255; }

    public static implicit operator Bgra32(Color color)
    {
        return ColorHelper.ToBgra32(color);
    }

    public static implicit operator Color(Bgra32 bgra8)
    {
        return ColorHelper.ToColor(bgra8);
    }
}