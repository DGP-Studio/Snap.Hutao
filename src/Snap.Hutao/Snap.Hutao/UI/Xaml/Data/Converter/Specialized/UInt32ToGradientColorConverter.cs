// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using Windows.UI;

namespace Snap.Hutao.UI.Xaml.Data.Converter.Specialized;

[DependencyProperty<int>("MaximumValue", DefaultValue = 6, NotNull = true)]
[DependencyProperty<int>("MinimumValue", DefaultValue = 1, NotNull = true)]
[DependencyProperty<Color>("Maximum", NotNull = true)]
[DependencyProperty<Color>("Minimum", NotNull = true)]
internal sealed partial class UInt32ToGradientColorConverter : DependencyValueConverter<uint, Color>
{
    public UInt32ToGradientColorConverter()
    {
        Maximum = ColorHelper.ToColor(0xFFFD0093);
        Minimum = ColorHelper.ToColor(0xFF4B00D9);
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