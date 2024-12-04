// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Snap.Hutao.Core.ExceptionService;

namespace Snap.Hutao.UI.Xaml.Data.Converter;

internal abstract class ValueConverter<TFrom, TTo> : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        try
        {
            return Convert((TFrom)value);
        }
        catch
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public object? ConvertBack(object value, Type targetType, object parameter, string language)
    {
        try
        {
            return ConvertBack((TTo)value);
        }
        catch
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public abstract TTo Convert(TFrom from);

    public virtual TFrom ConvertBack(TTo to)
    {
        throw HutaoException.NotSupported();
    }
}