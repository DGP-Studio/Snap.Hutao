// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.UI.Converters;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Snap.Hutao.View.Converter;

/// <summary>
/// Int32 转 Visibility
/// </summary>
public class Int32ToVisibilityConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return (int)value == 0 ? Visibility.Collapsed : Visibility.Visible;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}