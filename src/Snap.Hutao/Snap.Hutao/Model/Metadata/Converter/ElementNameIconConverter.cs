// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 元素名称图标转换器
/// </summary>
[HighQuality]
internal sealed class ElementNameIconConverter : ValueConverter<string, Uri>
{
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
            ? Web.HutaoEndpoints.UIIconNone
            : Web.HutaoEndpoints.StaticFile("IconElement", $"UI_Icon_Element_{element}.png").ToUri();

        // $"UI_Icon_Element_{element}.png"
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

    /// <inheritdoc/>
    public override Uri Convert(string from)
    {
        return ElementNameToIconUri(from);
    }
}