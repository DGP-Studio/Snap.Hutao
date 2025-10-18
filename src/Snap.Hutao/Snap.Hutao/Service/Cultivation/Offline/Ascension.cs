// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Cultivation.Offline;

internal static class Ascension
{
    public static ImmutableArray<int> GetRequiredLevels(ImmutableArray<int> levels, AvatarPromotionDelta delta)
    {
        return GetRequiredLevels(levels, (int)delta.AvatarLevelCurrent, (int)delta.AvatarLevelTarget, (int)delta.AvatarPromoteLevel);
    }

    public static ImmutableArray<int> GetRequiredLevels(ImmutableArray<int> levels, PromotionDelta delta)
    {
        return GetRequiredLevels(levels, (int)delta.LevelCurrent, (int)delta.LevelTarget, (int)delta.WeaponPromoteLevel);
    }

    public static ImmutableArray<int> GetRequiredLevels(ImmutableArray<int> levels, int currentLevel, int targetLevel, int currentPromoteLevel)
    {
        int lowerBound = LowerBound(levels, currentLevel, currentPromoteLevel);
        int upperBound = UpperBound(levels, targetLevel);

        if (lowerBound >= upperBound)
        {
            return [];
        }

        return levels[lowerBound..upperBound];
    }

    private static int LowerBound(ImmutableArray<int> arr, int value, int leftStart)
    {
        int left = leftStart, right = arr.Length;
        while (left < right)
        {
            int mid = (left + right) >> 1;
            if (arr[mid] >= value)
            {
                right = mid;
            }
            else
            {
                left = mid + 1;
            }
        }

        return left;
    }

    private static int UpperBound(ImmutableArray<int> arr, int value)
    {
        int left = 0, right = arr.Length;
        while (left < right)
        {
            int mid = (left + right) >> 1;
            if (arr[mid] <= value)
            {
                left = mid + 1;
            }
            else
            {
                right = mid;
            }
        }

        return left;
    }
}