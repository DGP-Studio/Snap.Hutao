// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Snap.Hutao.Control;
using Snap.Hutao.Win32;
using System.Runtime.CompilerServices;
using Windows.UI;

namespace Snap.Hutao.View.Converter;

/// <summary>
/// Int32 转 色阶颜色
/// </summary>
internal sealed class Int32ToGradientColorConverter : DependencyObject, IValueConverter
{
    private static readonly DependencyProperty MaximumProperty = Property<Int32ToGradientColorConverter>.Depend(nameof(Maximum), StructMarshal.Color(0xFFFF4949));
    private static readonly DependencyProperty MinimumProperty = Property<Int32ToGradientColorConverter>.Depend(nameof(Minimum), StructMarshal.Color(0xFF48FF7A));
    private static readonly DependencyProperty MaximumValueProperty = Property<Int32ToGradientColorConverter>.Depend(nameof(MaximumValue), 90);
    private static readonly DependencyProperty MinimumValueProperty = Property<Int32ToGradientColorConverter>.Depend(nameof(MinimumValue), 1);

    /// <summary>
    /// 最小颜色
    /// </summary>
    public Color Minimum
    {
        get => (Color)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    /// <summary>
    /// 最大颜色
    /// </summary>
    public Color Maximum
    {
        get => (Color)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    /// <summary>
    /// 最小值
    /// </summary>
    public int MinimumValue
    {
        get => (int)GetValue(MinimumValueProperty);
        set => SetValue(MinimumValueProperty, value);
    }

    /// <summary>
    /// 最大值
    /// </summary>
    public int MaximumValue
    {
        get => (int)GetValue(MaximumValueProperty);
        set => SetValue(MaximumValueProperty, value);
    }

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        double n = (value != null ? (int)value : MinimumValue) - MinimumValue;
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

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}