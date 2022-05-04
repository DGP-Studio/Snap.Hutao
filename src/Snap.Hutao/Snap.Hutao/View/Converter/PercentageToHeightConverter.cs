// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Snap.Hutao.Core;

namespace Snap.Hutao.View.Converter;

/// <summary>
/// 百分比转换高度
/// </summary>
public sealed class PercentageToHeightConverter : DependencyObject, IValueConverter
{
    private static readonly DependencyProperty TargetWidthProperty = Property<PercentageToHeightConverter>.Depend(nameof(TargetWidth), 1080D);
    private static readonly DependencyProperty TargetHeightProperty = Property<PercentageToHeightConverter>.Depend(nameof(TargetHeight), 390D);

    /// <summary>
    /// 目标宽度
    /// </summary>
    public double TargetWidth
    {
        get => (double)GetValue(TargetWidthProperty);

        set => SetValue(TargetWidthProperty, value);
    }

    /// <summary>
    /// 目标高度
    /// </summary>
    public double TargetHeight
    {
        get => (double)GetValue(TargetHeightProperty);

        set => SetValue(TargetHeightProperty, value);
    }

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string culture)
    {
        return (double)value * (TargetHeight / TargetWidth);
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string culture)
    {
        throw Must.NeverHappen();
    }
}