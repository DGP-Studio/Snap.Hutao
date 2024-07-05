// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Snap.Hutao.Core.ExceptionService;

namespace Snap.Hutao.UI.Xaml.Data.Converter;

/// <summary>
/// Int32 转 Visibility
/// </summary>
[HighQuality]
internal sealed class Int32ToVisibilityConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is not 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw HutaoException.NotSupported();
    }
}