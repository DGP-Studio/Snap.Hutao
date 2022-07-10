// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Data;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 物品等级转换器
/// </summary>
internal class QualityConverter : IValueConverter
{
    private const string BaseUrl = "https://static.snapgenshin.com/Bg/UI_{0}.png";

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        string? name = value.ToString();
        if (name == nameof(ItemQuality.QUALITY_ORANGE_SP))
        {
            name = "QUALITY_RED";
        }

        return new Uri(string.Format(BaseUrl, name));
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw Must.NeverHappen();
    }
}