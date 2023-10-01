// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Control;
using Snap.Hutao.Core;
using System.Runtime.CompilerServices;
using Windows.UI;

namespace Snap.Hutao.View.Converter;

/// <summary>
/// Int32 转 色阶颜色
/// </summary>
[DependencyProperty("MaximumValue", typeof(int), 90)]
[DependencyProperty("MinimumValue", typeof(int), 1)]
[DependencyProperty("Maximum", typeof(Color))]
[DependencyProperty("Minimum", typeof(Color))]
internal sealed partial class Int32ToGradientColorConverter : DependencyValueConverter<int, Color>
{
    public Int32ToGradientColorConverter()
    {
        Maximum = StructMarshal.Color(0xFFFF4949);
        Minimum = StructMarshal.Color(0xFF48FF7A);
    }

    public override Color Convert(int from)
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