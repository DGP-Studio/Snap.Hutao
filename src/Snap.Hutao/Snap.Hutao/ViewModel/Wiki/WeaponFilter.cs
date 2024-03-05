// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control.AutoSuggestBox;
using Snap.Hutao.Model.Intrinsic.Frozen;
using Snap.Hutao.Model.Metadata.Weapon;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel.Wiki;

/// <summary>
/// 武器筛选器
/// </summary>
internal static class WeaponFilter
{
    /// <summary>
    /// 构建筛选操作
    /// </summary>
    /// <param name="input">输入</param>
    /// <returns>筛选操作</returns>
    public static Predicate<Weapon> Compile(ObservableCollection<SearchToken> input)
    {
        return (Weapon weapon) => DoFilter(input, weapon);
    }

    private static bool DoFilter(ObservableCollection<SearchToken> input, Weapon weapon)
    {
        List<bool> matches = [];

        foreach ((SearchTokenKind kind, IEnumerable<string> tokens) in input.GroupBy(token => token.Kind, token => token.Value))
        {
            switch (kind)
            {
                case SearchTokenKind.WeaponType:
                    if (IntrinsicFrozen.WeaponTypes.Overlaps(tokens))
                    {
                        matches.Add(tokens.Contains(weapon.WeaponType.GetLocalizedDescriptionOrDefault()));
                    }

                    break;
                case SearchTokenKind.ItemQuality:
                    if (IntrinsicFrozen.ItemQualities.Overlaps(tokens))
                    {
                        matches.Add(tokens.Contains(weapon.Quality.GetLocalizedDescriptionOrDefault()));
                    }

                    break;
                case SearchTokenKind.FightProperty:
                    if (IntrinsicFrozen.FightProperties.Overlaps(tokens))
                    {
                        matches.Add(tokens.Contains(weapon.GrowCurves.ElementAtOrDefault(1)?.Type.GetLocalizedDescriptionOrDefault()));
                    }

                    break;
                case SearchTokenKind.Weapon:
                    matches.Add(tokens.Contains(weapon.Name));
                    break;
                default:
                    matches.Add(false);
                    break;
            }
        }

        return matches.Count > 0 && matches.Aggregate((a, b) => a && b);
    }
}