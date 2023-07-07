// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Primitives;
using Snap.Hutao.Model.Intrinsic.Immutable;
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
    public static Predicate<object> Compile(string input)
    {
        return (object o) => o is Avatar avatar && DoFilter(input, avatar);
    }

    private static bool DoFilter(string input, Avatar avatar)
    {
        List<bool> matches = new();

        foreach (StringSegment segment in new StringTokenizer(input, new char[] { ' ' }))
        {
            string value = segment.ToString();

            if (avatar.Name == value)
            {
                matches.Add(true);
                continue;
            }

            if (IntrinsicImmutable.ElementNames.Contains(value))
            {
                matches.Add(avatar.FetterInfo.VisionBefore == value);
                continue;
            }

            if (IntrinsicImmutable.AssociationTypes.Contains(value))
            {
                matches.Add(avatar.FetterInfo.Association.GetLocalizedDescriptionOrDefault() == value);
                continue;
            }

            if (IntrinsicImmutable.WeaponTypes.Contains(value))
            {
                matches.Add(avatar.Weapon.GetLocalizedDescriptionOrDefault() == value);
                continue;
            }

            if (IntrinsicImmutable.ItemQualities.Contains(value))
            {
                matches.Add(avatar.Quality.GetLocalizedDescriptionOrDefault() == value);
                continue;
            }

            if (IntrinsicImmutable.BodyTypes.Contains(value))
            {
                matches.Add(avatar.Body.GetLocalizedDescriptionOrDefault() == value);
                continue;
            }

            matches.Add(false);
        }

        return matches.Count > 0 && matches.Aggregate((a, b) => a && b);
    }
}