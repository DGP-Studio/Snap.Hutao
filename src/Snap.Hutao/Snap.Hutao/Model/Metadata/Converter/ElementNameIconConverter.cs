// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;
using Snap.Hutao.Control.Media;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 元素名称图标转换器
/// </summary>
internal class ElementNameIconConverter : ValueConverterBase<string, Uri>
{
    private const string BaseUrl = "https://static.snapgenshin.com/IconElement/UI_Icon_Element_{0}.png";
    private static readonly Uri UIIconNone = new("https://static.snapgenshin.com/Bg/UI_Icon_None.png");

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
            _ => string.Empty,
        };

        return string.IsNullOrEmpty(element)
            ? UIIconNone
            : new Uri(string.Format(BaseUrl, element));
    }

    /// <summary>
    /// 将中文元素名称转换为元素类型
    /// </summary>
    /// <param name="elementName">元素名称</param>
    /// <returns>元素类型</returns>
    public static ElementType ElementNameToElementType(string elementName)
    {
        return elementName switch
        {
            "雷" => ElementType.Electric,
            "火" => ElementType.Fire,
            "草" => ElementType.Grass,
            "冰" => ElementType.Ice,
            "岩" => ElementType.Rock,
            "水" => ElementType.Water,
            "风" => ElementType.Wind,
            _ => ElementType.None,
        };
    }

    /// <summary>
    /// 将元素类型转换为 Bgra8
    /// </summary>
    /// <param name="type">元素类型</param>
    /// <returns>Bgra8</returns>
    public static Bgra8 ElementTypeToBgra8(ElementType type)
    {
        return type switch
        {
            ElementType.Electric => Bgra8.FromRgb(0xDF, 0xBB, 0xFF),
            ElementType.Fire => Bgra8.FromRgb(0xFF, 0xA8, 0x70),
            ElementType.Grass => Bgra8.FromRgb(0xB1, 0xEB, 0x29),
            ElementType.Ice => Bgra8.FromRgb(0xCC, 0xFF, 0xFF),
            ElementType.Rock => Bgra8.FromRgb(0xF4, 0xD6, 0x60),
            ElementType.Water => Bgra8.FromRgb(0x08, 0xE4, 0xFF),
            ElementType.Wind => Bgra8.FromRgb(0xA7, 0xF7, 0xD0),
            _ => Bgra8.FromRgb(0x80, 0x80, 0x80),
        };
    }

    /// <inheritdoc/>
    public override Uri Convert(string from)
    {
        return ElementNameToIconUri(from);
    }
}