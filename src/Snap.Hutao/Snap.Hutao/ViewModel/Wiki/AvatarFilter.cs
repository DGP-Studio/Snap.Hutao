// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic.Frozen;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.UI.Xaml.Control.AutoSuggestBox;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel.Wiki;

/// <summary>
/// 角色筛选器
/// </summary>
internal static class AvatarFilter
{
    /// <summary>
    /// 构建筛选操作
    /// </summary>
    /// <param name="input">输入</param>
    /// <returns>筛选操作</returns>
    public static Predicate<Avatar> Compile(ObservableCollection<SearchToken> input)
    {
        return (Avatar avatar) => DoFilter(input, avatar);
    }

    private static bool DoFilter(ObservableCollection<SearchToken> input, Avatar avatar)
    {
        List<bool> matches = [];

        foreach ((SearchTokenKind kind, IEnumerable<string> tokens) in input.GroupBy(token => token.Kind, token => token.Value))
        {
            switch (kind)
            {
                case SearchTokenKind.ElementName:
                    if (IntrinsicFrozen.ElementNames.Overlaps(tokens))
                    {
                        matches.Add(tokens.Contains(avatar.FetterInfo.VisionBefore));
                    }

                    break;
                case SearchTokenKind.AssociationType:
                    if (IntrinsicFrozen.AssociationTypes.Overlaps(tokens))
                    {
                        matches.Add(tokens.Contains(avatar.FetterInfo.Association.GetLocalizedDescriptionOrDefault()));
                    }

                    break;
                case SearchTokenKind.WeaponType:
                    if (IntrinsicFrozen.WeaponTypes.Overlaps(tokens))
                    {
                        matches.Add(tokens.Contains(avatar.Weapon.GetLocalizedDescriptionOrDefault()));
                    }

                    break;
                case SearchTokenKind.ItemQuality:
                    if (IntrinsicFrozen.ItemQualities.Overlaps(tokens))
                    {
                        matches.Add(tokens.Contains(avatar.Quality.GetLocalizedDescriptionOrDefault()));
                    }

                    break;
                case SearchTokenKind.BodyType:
                    if (IntrinsicFrozen.BodyTypes.Overlaps(tokens))
                    {
                        matches.Add(tokens.Contains(avatar.Body.GetLocalizedDescriptionOrDefault()));
                    }

                    break;
                case SearchTokenKind.Avatar:
                    matches.Add(tokens.Contains(avatar.Name));
                    break;
                default:
                    matches.Add(false);
                    break;
            }
        }

        return matches.Count > 0 && matches.Aggregate((a, b) => a && b);
    }
}