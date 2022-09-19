// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Data;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 元素名称图标转换器
/// </summary>
internal class ElementNameIconConverter : IValueConverter
{
    private const string BaseUrl = "https://static.snapgenshin.com/IconElement/UI_Icon_Element_{0}.png";

    /// <summary>
    /// 将中文元素名称转换为图标链接
    /// </summary>
    /// <param name="elementName">元素名称</param>
    /// <returns>图标链接</returns>
    public static Uri ElementNameToIconUri(string elementName)
    {
        string element = elementName switch
        {
            "雷" => "Electric",
            "火" => "Fire",
            "草" => "Grass",
            "冰" => "Ice",
            "岩" => "Rock",
            "水" => "Water",
            "风" => "Wind",
            _ => throw Must.NeverHappen(),
        };

        return new Uri(string.Format(BaseUrl, element));
    }

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return ElementNameToIconUri((string)value);
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw Must.NeverHappen();
    }
}
