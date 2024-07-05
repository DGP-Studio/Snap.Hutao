// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Data;
using Snap.Hutao.Core.ExceptionService;

namespace Snap.Hutao.UI.Xaml.Data.Converter;

internal abstract class ValueConverter<TFrom, TTo> : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        return Convert((TFrom)value);
    }

    public object? ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return ConvertBack((TTo)value);
    }

    public abstract TTo Convert(TFrom from);

    public virtual TFrom ConvertBack(TTo to)
    {
        throw HutaoException.NotSupported();
    }
}