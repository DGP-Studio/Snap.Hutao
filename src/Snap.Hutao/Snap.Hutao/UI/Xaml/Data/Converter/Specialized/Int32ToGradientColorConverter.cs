// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using Windows.UI;

namespace Snap.Hutao.UI.Xaml.Data.Converter.Specialized;

/// <summary>
/// Int32 转 色阶颜色
/// </summary>
[DependencyProperty<int>("MaximumValue", DefaultValue = 90, NotNull = true)]
[DependencyProperty<int>("MinimumValue", DefaultValue = 1, NotNull = true)]
[DependencyProperty<Color>("Maximum", NotNull = true)]
[DependencyProperty<Color>("Minimum", NotNull = true)]
internal sealed partial class Int32ToGradientColorConverter : DependencyValueConverter<int, Color>
{
    public Int32ToGradientColorConverter()
    {
        Maximum = ColorHelper.ToColor(0xFFFF4949);
        Minimum = ColorHelper.ToColor(0xFF48FF7A);
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