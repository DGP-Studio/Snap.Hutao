// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Intrinsic.Frozen;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.UI.Xaml.Control.AutoSuggestBox;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Snap.Hutao.ViewModel.Cultivation;

internal static class CultivateEntryViewFilter
{
    public static Predicate<CultivateEntryView>? Compile(SearchData? searchData, ICultivationMetadataContext metadataContext)
    {
        return searchData?.FilterTokens is null or [] ? default : Compile(searchData.FilterTokens, metadataContext);
    }

    public static Predicate<CultivateEntryView> Compile(ObservableCollection<SearchToken> input, ICultivationMetadataContext metadataContext)
    {
        return avatar => DoFilter(input, avatar, metadataContext);
    }

    private static bool DoFilter(ObservableCollection<SearchToken> input, CultivateEntryView cultivateEntryView, ICultivationMetadataContext metadataContext)
    {
        List<bool> matches = [];

        foreach ((SearchTokenKind kind, IEnumerable<string> tokens) in input.GroupBy(token => token.Kind, token => token.Value))
        {
            switch (kind)
            {
                case SearchTokenKind.ElementName:
                    if (cultivateEntryView.Type is CultivateType.AvatarAndSkill && IntrinsicFrozen.ElementNames.Overlaps(tokens))
                    {
                        Avatar avatar = metadataContext.GetAvatar(cultivateEntryView.Id);
                        matches.Add(tokens.Contains(avatar.FetterInfo.VisionBefore));
                    }

                    break;
                case SearchTokenKind.WeaponType:
                    if (IntrinsicFrozen.WeaponTypes.Overlaps(tokens))
                    {
                        WeaponType weaponType = cultivateEntryView.Type switch
                        {
                            CultivateType.AvatarAndSkill => metadataContext.GetAvatar(cultivateEntryView.Id).Weapon,
                            CultivateType.Weapon => metadataContext.GetWeapon(cultivateEntryView.Id).WeaponType,
                            _ => WeaponType.WEAPON_NONE,
                        };

                        matches.Add(tokens.Contains(weaponType.GetLocalizedDescriptionOrDefault(SH.ResourceManager, CultureInfo.CurrentCulture)));
                    }

                    break;
                case SearchTokenKind.ItemQuality:
                    if (IntrinsicFrozen.ItemQualities.Overlaps(tokens))
                    {
                        matches.Add(tokens.Contains(cultivateEntryView.Quality.GetLocalizedDescriptionOrDefault(SH.ResourceManager, CultureInfo.CurrentCulture)));
                    }

                    break;
                case SearchTokenKind.CultivateType:
                    if (IntrinsicFrozen.CultivateTypes.Overlaps(tokens))
                    {
                        matches.Add(tokens.Contains(cultivateEntryView.Type.GetLocalizedDescriptionOrDefault(SH.ResourceManager)));
                    }

                    break;
                default:
                    matches.Add(false);
                    break;
            }
        }

        return matches.Count > 0 && matches.Aggregate((a, b) => a && b);
    }
}