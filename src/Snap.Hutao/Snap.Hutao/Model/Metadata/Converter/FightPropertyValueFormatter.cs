// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Data;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Annotation;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 战斗属性数值格式化器
/// </summary>
internal class FightPropertyValueFormatter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        FormatMethod method = ((FightProperty)parameter).GetFormat();

        return method switch
        {
            FormatMethod.Integer => Math.Round((double)value, MidpointRounding.AwayFromZero),
            FormatMethod.Percent => string.Format("{0:P1}", value),
            _ => value,
        };
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw Must.NeverHappen();
    }
}