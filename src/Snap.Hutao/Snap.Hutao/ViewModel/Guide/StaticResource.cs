// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Setting;
using Windows.Storage;

namespace Snap.Hutao.ViewModel.Guide;

/// <summary>
/// 静态资源
/// </summary>
[HighQuality]
internal static class StaticResource
{
    private const string ContractMap = "StaticResourceContractMap";

    private static readonly ApplicationDataCompositeValue DefaultResourceVersionMap = new()
    {
        { "AchievementIcon", 0 },
        { "AvatarCard", 0 },
        { "AvatarIcon", 0 },
        { "Bg", 0 },
        { "ChapterIcon", 0 },
        { "CodexMonster", 0 },
        { "Costume", 0 },
        { "EmotionIcon", 0 },
        { "EquipIcon", 0 },
        { "GachaAvatarIcon", 0 },
        { "GachaAvatarImg", 0 },
        { "GachaEquipIcon", 0 },
        { "GcgCharAvatarIcon", 0 },
        { "IconElement", 0 },
        { "ItemIcon", 0 },
        { "LoadingPic", 0 },
        { "MonsterIcon", 0 },
        { "MonsterSmallIcon", 0 },
        { "NameCardIcon", 0 },
        { "NameCardPic", 0 },
        { "NameCardPicAlpha", 0 },
        { "Property", 0 },
        { "RelicIcon", 0 },
        { "Skill", 0 },
        { "Talent", 0 },
    };

    private static readonly ApplicationDataCompositeValue LatestResourceVersionMap = new()
    {
        { "AchievementIcon", 1 },
        { "AvatarCard", 1 },
        { "AvatarIcon", 3 },
        { "Bg", 2 },
        { "ChapterIcon", 1 },
        { "CodexMonster", 0 },
        { "Costume", 1 },
        { "EmotionIcon", 1 },
        { "EquipIcon", 2 },
        { "GachaAvatarIcon", 2 },
        { "GachaAvatarImg", 2 },
        { "GachaEquipIcon", 2 },
        { "GcgCharAvatarIcon", 0 },
        { "IconElement", 2 },
        { "ItemIcon", 2 },
        { "LoadingPic", 1 },
        { "MonsterIcon", 2 },
        { "MonsterSmallIcon", 1 },
        { "NameCardIcon", 1 },
        { "NameCardPic", 2 },
        { "NameCardPicAlpha", 0 },
        { "Property", 1 },
        { "RelicIcon", 2 },
        { "Skill", 2 },
        { "Talent", 2 },
    };

    public static void FulfillAll()
    {
        LocalSetting.Set(ContractMap, LatestResourceVersionMap);
    }

    public static void FailAll()
    {
        LocalSetting.Set(ContractMap, DefaultResourceVersionMap);
    }

    [Obsolete]
    public static bool IsContractUnfulfilled(string contractKey)
    {
        return !LocalSetting.Get(contractKey, false);
    }

    public static bool IsAnyUnfulfilledCategoryPresent()
    {
        ApplicationDataCompositeValue map = LocalSetting.Get(ContractMap, DefaultResourceVersionMap);
        if (map.Count < LatestResourceVersionMap.Count)
        {
            return true;
        }

        foreach ((string key, object value) in map)
        {
            if ((int)value < (int)LatestResourceVersionMap[key])
            {
                return true;
            }
        }

        return false;
    }

    public static HashSet<string> GetUnfulfilledCategorySet()
    {
        HashSet<string> result = [];
        ApplicationDataCompositeValue map = LocalSetting.Get(ContractMap, DefaultResourceVersionMap);
        foreach ((string key, object value) in LatestResourceVersionMap)
        {
            if (!map.TryGetValue(key, out object current) || (int)value > (int)current)
            {
                result.Add(key);
            }
        }

        return result;
    }
}