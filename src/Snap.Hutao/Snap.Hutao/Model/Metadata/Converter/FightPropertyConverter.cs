// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Data;
using Snap.Hutao.Extension;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 战斗属性转换器
/// </summary>
internal class FightPropertyConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return ((FightProperty)value).GetDescription();
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw Must.NeverHappen();
    }
}