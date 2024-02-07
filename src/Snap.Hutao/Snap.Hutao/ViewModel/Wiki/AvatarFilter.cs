// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Primitives;
using Snap.Hutao.Model.Intrinsic.Frozen;
using Snap.Hutao.Model.Metadata.Avatar;

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
    public static Predicate<Avatar> Compile(string input)
    {
        return (Avatar avatar) => DoFilter(input, avatar);
    }

    private static bool DoFilter(string input, Avatar avatar)
    {
        List<bool> matches = [];
        foreach (StringSegment segment in new StringTokenizer(input, [' ']))
        {
            string value = segment.ToString();

            if (avatar.Name == value)
            {
                matches.Add(true);
                continue;
            }

            if (IntrinsicFrozen.ElementNames.Contains(value))
            {
                matches.Add(avatar.FetterInfo.VisionBefore == value);
                continue;
            }

            if (IntrinsicFrozen.AssociationTypes.Contains(value))
            {
                matches.Add(avatar.FetterInfo.Association.GetLocalizedDescriptionOrDefault() == value);
                continue;
            }

            if (IntrinsicFrozen.WeaponTypes.Contains(value))
            {
                matches.Add(avatar.Weapon.GetLocalizedDescriptionOrDefault() == value);
                continue;
            }

            if (IntrinsicFrozen.ItemQualities.Contains(value))
            {
                matches.Add(avatar.Quality.GetLocalizedDescriptionOrDefault() == value);
                continue;
            }

            if (IntrinsicFrozen.BodyTypes.Contains(value))
            {
                matches.Add(avatar.Body.GetLocalizedDescriptionOrDefault() == value);
                continue;
            }

            matches.Add(false);
        }

        return matches.Count > 0 && matches.Aggregate((a, b) => a && b);
    }
}