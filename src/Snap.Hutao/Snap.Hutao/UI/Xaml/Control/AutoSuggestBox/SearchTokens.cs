// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Intrinsic.Frozen;
using Snap.Hutao.Model.Metadata.Converter;
using System.Collections.Frozen;

namespace Snap.Hutao.UI.Xaml.Control.AutoSuggestBox;

internal static class SearchTokens
{
    private static readonly LazySlim<FrozenDictionary<string, SearchToken>> AvatarPropertyTokens = new(() => FrozenDictionary.ToFrozenDictionary(
    [
        .. IntrinsicFrozen.ElementNameValues.Select(static nv => KeyValuePair.Create(nv.Name, new SearchToken(SearchTokenKind.ElementName, nv.Name, nv.Value, iconUri: ElementNameIconConverter.ElementNameToUri(nv.Name)))),
        .. IntrinsicFrozen.ItemQualityNameValues.Where(static nv => nv.Value >= QualityType.QUALITY_PURPLE).Select(nv => KeyValuePair.Create(nv.Name, new SearchToken(SearchTokenKind.ItemQuality, nv.Name, (int)nv.Value, quality: QualityColorConverter.QualityToColor(nv.Value)))),
        .. IntrinsicFrozen.WeaponTypeNameValues.Select(static nv => KeyValuePair.Create(nv.Name, new SearchToken(SearchTokenKind.WeaponType, nv.Name, (int)nv.Value, iconUri: WeaponTypeIconConverter.WeaponTypeToIconUri(nv.Value)))),
    ]));

    public static FrozenDictionary<string, SearchToken> GetForAvatarProperty()
    {
        return AvatarPropertyTokens.Value;
    }
}