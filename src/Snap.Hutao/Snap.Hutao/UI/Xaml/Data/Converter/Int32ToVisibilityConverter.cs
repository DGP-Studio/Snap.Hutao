// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Snap.Hutao.UI.Xaml.Data.Converter;

/// <summary>
/// Int32 转 Visibility
/// </summary>
[HighQuality]
internal sealed partial class Int32ToVisibilityConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is not null && value is not 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return DependencyProperty.UnsetValue;
    }
}