// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Control;
using Snap.Hutao.Core;
using System.Runtime.CompilerServices;
using Windows.UI;

namespace Snap.Hutao.View.Converter;

/// <summary>
/// UInt32 转 色阶颜色
/// </summary>
[DependencyProperty("MaximumValue", typeof(int), 6)]
[DependencyProperty("MinimumValue", typeof(int), 1)]
[DependencyProperty("Maximum", typeof(Color))]
[DependencyProperty("Minimum", typeof(Color))]
internal sealed partial class UInt32ToGradientColorConverter : DependencyValueConverter<uint, Color>
{
    public UInt32ToGradientColorConverter()
    {
        Maximum = StructMarshal.Color(0xFFFD0093);
        Minimum = StructMarshal.Color(0xFF4B00D9);
    }

    public override Color Convert(uint from)
    {
        double n = Math.Clamp(from, MinimumValue, MaximumValue) - MinimumValue;
        int step = MaximumValue - MinimumValue;
        double a = Minimum.A + ((Maximum.A - Minimum.A) * n / step);
        double r = Minimum.R + ((Maximum.R - Minimum.R) * n / step);
        double g = Minimum.G + ((Maximum.G - Minimum.G) * n / step);
        double b = Minimum.B + ((Maximum.B - Minimum.B) * n / step);

        Unsafe.SkipInit(out Color color);
        color.A = (byte)a;
        color.R = (byte)r;
        color.G = (byte)g;
        color.B = (byte)b;
        return color;
    }
}