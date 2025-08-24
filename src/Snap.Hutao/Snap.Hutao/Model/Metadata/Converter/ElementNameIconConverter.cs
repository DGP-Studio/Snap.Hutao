// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.UI.Xaml.Data.Converter;
using Snap.Hutao.Web.Endpoint.Hutao;
using System.Collections.Frozen;

namespace Snap.Hutao.Model.Metadata.Converter;

internal sealed partial class ElementNameIconConverter : ValueConverter<string, Uri>
{
    private static readonly FrozenDictionary<string, string> LocalizedNameToElementIconName = WinRTAdaptive.ToFrozenDictionary(
    [
        KeyValuePair.Create(SH.ModelIntrinsicElementNameElec, "Electric"),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameFire, "Fire"),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameGrass, "Grass"),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameIce, "Ice"),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameRock, "Rock"),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameWater, "Water"),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameWind, "Wind"),
    ]);

    private static readonly FrozenDictionary<string, ElementType> LocalizedNameToElementType = WinRTAdaptive.ToFrozenDictionary(
    [
        KeyValuePair.Create(SH.ModelIntrinsicElementNameElec, ElementType.Electric),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameFire, ElementType.Fire),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameGrass, ElementType.Grass),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameIce, ElementType.Ice),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameRock, ElementType.Rock),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameWater, ElementType.Water),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameWind, ElementType.Wind),
    ]);

    public static Uri ElementNameToUri(string elementName)
    {
        string? element = LocalizedNameToElementIconName.GetValueOrDefault(elementName);

        return string.IsNullOrEmpty(element)
            ? StaticResourcesEndpoints.UIIconNone
            : StaticResourcesEndpoints.StaticRaw("IconElement", $"UI_Icon_Element_{element}.png").ToUri();
    }

    public static ElementType ElementNameToElementType(string elementName)
    {
        return LocalizedNameToElementType.GetValueOrDefault(elementName);
    }

    public override Uri Convert(string from)
    {
        return ElementNameToUri(from);
    }
}