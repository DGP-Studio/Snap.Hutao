// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Data;

namespace Snap.Hutao.Control;

/// <summary>
/// 值转换器
/// </summary>
/// <typeparam name="TFrom">源类型</typeparam>
/// <typeparam name="TTo">目标类型</typeparam>
internal abstract class ValueConverter<TFrom, TTo> : IValueConverter
{
    /// <inheritdoc/>
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        return Convert((TFrom)value);
    }

    /// <inheritdoc/>
    public object? ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return ConvertBack((TTo)value);
    }

    /// <summary>
    /// 从源类型转换到目标类型
    /// </summary>
    /// <param name="from">源</param>
    /// <returns>目标</returns>
    public abstract TTo Convert(TFrom from);

    /// <summary>
    /// 从目标类型转换到源类型
    /// 重写时请勿调用基类方法
    /// </summary>
    /// <param name="to">目标</param>
    /// <returns>源</returns>
    public virtual TFrom ConvertBack(TTo to)
    {
        throw Must.NeverHappen();
    }
}