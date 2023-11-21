// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Primitives;
using Snap.Hutao.Model.Intrinsic.Frozen;
using Snap.Hutao.Model.Metadata.Weapon;

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
    public static Predicate<object> Compile(string input)
    {
        return (object o) => o is Weapon weapon && DoFilter(input, weapon);
    }

    private static bool DoFilter(string input, Weapon weapon)
    {
        List<bool> matches = [];

        foreach (StringSegment segment in new StringTokenizer(input, [' ']))
        {
            string value = segment.ToString();

            if (weapon.Name == value)
            {
                matches.Add(true);
                continue;
            }

            if (IntrinsicFrozen.WeaponTypes.Contains(value))
            {
                matches.Add(weapon.WeaponType.GetLocalizedDescriptionOrDefault() == value);
                continue;
            }

            if (IntrinsicFrozen.ItemQualities.Contains(value))
            {
                matches.Add(weapon.Quality.GetLocalizedDescriptionOrDefault() == value);
                continue;
            }

            if (IntrinsicFrozen.FightProperties.Contains(value))
            {
                matches.Add(weapon.GrowCurves.ElementAtOrDefault(1)?.Type.GetLocalizedDescriptionOrDefault() == value);
                continue;
            }
        }

        return matches.Count > 0 && matches.Aggregate((a, b) => a && b);
    }
}