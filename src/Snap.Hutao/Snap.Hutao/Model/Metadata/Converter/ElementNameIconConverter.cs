// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;
using Snap.Hutao.Model.Intrinsic;
using System.Collections.Frozen;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 元素名称图标转换器
/// </summary>
[HighQuality]
internal sealed class ElementNameIconConverter : ValueConverter<string, Uri>
{
    private static readonly FrozenDictionary<string, string> LocalizedNameToElementIconName = FrozenDictionary.ToFrozenDictionary(
    [
        KeyValuePair.Create(SH.ModelIntrinsicElementNameElec, "Electric"),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameFire, "Fire"),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameGrass, "Grass"),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameIce, "Ice"),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameRock, "Rock"),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameWater, "Water"),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameWind, "Wind"),
    ]);

    private static readonly FrozenDictionary<string, ElementType> LocalizedNameToElementType = FrozenDictionary.ToFrozenDictionary(
    [
        KeyValuePair.Create(SH.ModelIntrinsicElementNameElec, ElementType.Electric),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameFire, ElementType.Fire),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameGrass, ElementType.Grass),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameIce, ElementType.Ice),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameRock, ElementType.Rock),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameWater, ElementType.Water),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameWind, ElementType.Wind),
    ]);

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
            : Web.HutaoEndpoints.StaticRaw("IconElement", $"UI_Icon_Element_{element}.png").ToUri();

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