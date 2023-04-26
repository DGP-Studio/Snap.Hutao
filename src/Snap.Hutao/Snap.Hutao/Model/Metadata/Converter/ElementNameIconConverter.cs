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
    private static readonly Dictionary<string, string> LocalizedNameToElementIconName = new()
    {
        [SH.ModelIntrinsicElementNameElec] = "Electric",
        [SH.ModelIntrinsicElementNameFire] = "Fire",
        [SH.ModelIntrinsicElementNameGrass] = "Grass",
        [SH.ModelIntrinsicElementNameIce] = "Ice",
        [SH.ModelIntrinsicElementNameRock] = "Rock",
        [SH.ModelIntrinsicElementNameWater] = "Water",
        [SH.ModelIntrinsicElementNameWind] = "Wind",
    };

    private static readonly Dictionary<string, ElementType> LocalizedNameToElementType = new()
    {
        [SH.ModelIntrinsicElementNameElec] = ElementType.Electric,
        [SH.ModelIntrinsicElementNameFire] = ElementType.Fire,
        [SH.ModelIntrinsicElementNameGrass] = ElementType.Grass,
        [SH.ModelIntrinsicElementNameIce] = ElementType.Ice,
        [SH.ModelIntrinsicElementNameRock] = ElementType.Rock,
        [SH.ModelIntrinsicElementNameWater] = ElementType.Water,
        [SH.ModelIntrinsicElementNameWind] = ElementType.Wind,
    };

    /// <summary>
    /// 将中文元素名称转换为图标链接
    /// </summary>
    /// <param name="elementName">元素名称</param>
    /// <returns>图标链接</returns>
    public static Uri ElementNameToIconUri(string elementName)
    {
        string? element = LocalizedNameToElementIconName.GetValueOrDefault(elementName);

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
        return LocalizedNameToElementType.GetValueOrDefault(elementName);
    }

    /// <inheritdoc/>
    public override Uri Convert(string from)
    {
        return ElementNameToIconUri(from);
    }
}