// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Setting;
using Windows.Storage;

namespace Snap.Hutao.ViewModel.Guide;

internal static class StaticResource
{
    private const string ContractMap = "StaticResourceContractMap";

    private static readonly Lock SyncRoot = new();

    private static readonly ApplicationDataCompositeValue DefaultResourceVersionMap = new()
    {
        // DO NOT MODIFY THIS MAP EXCEPT WHEN ADDING ENTRY
        { "AchievementIcon", 0 },
        { "AvatarCard", 0 },
        { "AvatarIcon", 0 },
        { "AvatarIconCircle", 0 },
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
        { "Mark", 0 },
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
        { "AchievementIcon", 2 },
        { "AvatarCard", 2 },
        { "AvatarIcon", 5 },
        { "AvatarIconCircle", 1 },
        { "Bg", 3 },
        { "ChapterIcon", 3 },
        { "CodexMonster", 0 },
        { "Costume", 2 },
        { "EmotionIcon", 3 },
        { "EquipIcon", 5 },
        { "GachaAvatarIcon", 5 },
        { "GachaAvatarImg", 4 },
        { "GachaEquipIcon", 4 },
        { "GcgCharAvatarIcon", 0 },
        { "IconElement", 3 },
        { "ItemIcon", 4 },
        { "LoadingPic", 2 },
        { "Mark", 0 },
        { "MonsterIcon", 3 },
        { "MonsterSmallIcon", 2 },
        { "NameCardIcon", 3 },
        { "NameCardPic", 4 },
        { "NameCardPicAlpha", 0 },
        { "Property", 2 },
        { "RelicIcon", 4 },
        { "Skill", 4 },
        { "Talent", 4 },
    };

    public static void Fulfill(string contractKey)
    {
        lock (SyncRoot)
        {
            ApplicationDataCompositeValue map = LocalSetting.Get(ContractMap, DefaultResourceVersionMap);
            map[contractKey] = LatestResourceVersionMap[contractKey];
            LocalSetting.Set(ContractMap, map);
        }
    }

    public static void FulfillAll()
    {
        LocalSetting.Set(ContractMap, LatestResourceVersionMap);
    }

    public static void FailAll()
    {
        LocalSetting.Set(ContractMap, DefaultResourceVersionMap);
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