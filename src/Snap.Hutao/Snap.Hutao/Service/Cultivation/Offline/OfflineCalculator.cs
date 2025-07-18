// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;
using System.Collections.Immutable;
using MetadataAvatar = Snap.Hutao.Model.Metadata.Avatar.Avatar;
using MetadataWeapon = Snap.Hutao.Model.Metadata.Weapon.Weapon;

namespace Snap.Hutao.Service.Cultivation.Offline;

internal static class OfflineCalculator
{
    private const int PurpleExpBookExp = 20000;
    private const int PurpleWeaponExp = 10000;

    private const int PurpleExpBookMoraPer = 4000;
    private const int PurpleWeaponExpMoraPer = 1000;

    private const uint MoraItemId = 202U;
    private const uint AvatarExpBookId = 104003U;
    private const uint WeaponExpBookId = 104013U;
    private const uint CrownOfInsightId = 104319U;

    private static readonly int[] AscensionLevels = [20, 40, 50, 60, 70, 80];

    private static readonly int[] AvatarAscensionMoraCosts = [20000, 40000, 60000, 80000, 100000, 120000];
    private static readonly uint[] AvatarBossMaterialCounts = [0, 2, 4, 8, 12, 20];
    private static readonly uint[] AvatarSpecialtyCounts = [3, 10, 20, 30, 45, 60];

    private static readonly int[] TalentMoraCosts = [0, 12500, 17500, 25000, 30000, 37500, 120000, 260000, 450000, 700000];
    private static readonly uint[] WeeklyBossCounts = [0, 0, 0, 0, 0, 0, 1, 1, 2, 2];

    public static BatchConsumption? CalculateAvatarConsumption(AvatarPromotionDelta delta, MetadataAvatar avatar)
    {
        Dictionary<uint, uint> itemCounts = [];
        int totalMora = 0;

        // 计算角色经验
        if (delta.AvatarLevelCurrent < delta.AvatarLevelTarget)
        {
            (int expMora, uint expBooks) = CalculateAvatarExperience(delta.AvatarLevelCurrent, delta.AvatarLevelTarget);
            totalMora += expMora;
            AddOrUpdateItem(itemCounts, AvatarExpBookId, expBooks);
        }

        // 计算角色突破
        if (delta.AvatarLevelCurrent <= delta.AvatarLevelTarget)
        {
            (int ascensionMora, Dictionary<uint, uint> ascensionItems) = CalculateAvatarAscension(
                avatar,
                delta.AvatarLevelCurrent,
                delta.AvatarLevelTarget,
                delta.AvatarPromoteLevel);

            totalMora += ascensionMora;
            foreach ((uint id, uint count) in ascensionItems)
            {
                AddOrUpdateItem(itemCounts, id, count);
            }
        }

        // 计算天赋升级
        if (delta.SkillList.Length > 0)
        {
            foreach (PromotionDelta skill in delta.SkillList)
            {
                if (skill.LevelCurrent < skill.LevelTarget)
                {
                    (int talentMora, Dictionary<uint, uint> talentItems) = CalculateTalentUpgrade(
                        avatar,
                        skill.LevelCurrent,
                        skill.LevelTarget);

                    totalMora += talentMora;
                    foreach ((uint id, uint count) in talentItems)
                    {
                        AddOrUpdateItem(itemCounts, id, count);
                    }
                }
            }
        }

        // 添加摩拉
        if (totalMora > 0)
        {
            AddOrUpdateItem(itemCounts, MoraItemId, (uint)totalMora);
        }

        return CreateBatchConsumption(itemCounts);
    }

    public static BatchConsumption? CalculateWeaponConsumption(AvatarPromotionDelta delta, MetadataWeapon weapon)
    {
        if (delta.Weapon is null)
        {
            return default;
        }

        Dictionary<uint, uint> itemCounts = [];
        int totalMora = 0;

        // 计算武器经验
        if (delta.Weapon.LevelCurrent < delta.Weapon.LevelTarget)
        {
            int starRarity = (int)weapon.RankLevel;
            (int expMora, uint expBooks) = CalculateWeaponExperience(starRarity, delta.Weapon.LevelCurrent, delta.Weapon.LevelTarget);
            totalMora += expMora;
            AddOrUpdateItem(itemCounts, WeaponExpBookId, expBooks);
        }

        // 计算武器突破
        if (delta.Weapon.LevelCurrent <= delta.Weapon.LevelTarget)
        {
            (int ascensionMora, Dictionary<uint, uint> ascensionItems) = CalculateWeaponAscension(
                weapon,
                delta.Weapon.LevelCurrent,
                delta.Weapon.LevelTarget);

            totalMora += ascensionMora;
            foreach ((uint id, uint count) in ascensionItems)
            {
                AddOrUpdateItem(itemCounts, id, count);
            }
        }

        // 添加摩拉
        if (totalMora > 0)
        {
            AddOrUpdateItem(itemCounts, MoraItemId, (uint)totalMora);
        }

        return CreateBatchConsumption(itemCounts);
    }

    #region Avatar

    private static (int Mora, uint ExpBooks) CalculateAvatarExperience(uint currentLevel, uint targetLevel)
    {
        int requiredExp = AvatarLevelExperience.CalculateTotalExperience((int)currentLevel, (int)targetLevel);
        uint expBooks = (uint)Math.Round((double)requiredExp / PurpleExpBookExp, MidpointRounding.ToPositiveInfinity);
        int mora = (int)(expBooks * PurpleExpBookMoraPer);

        return (mora, expBooks);
    }

    private static (int Mora, Dictionary<uint, uint> Items) CalculateAvatarAscension(
        MetadataAvatar avatar,
        uint currentLevel,
        uint targetLevel,
        uint currentPromoteLevel)
    {
        List<int> requiredAscensions = [];

        for (int i = 0; i < AscensionLevels.Length; i++)
        {
            int ascensionLevel = AscensionLevels[i];
            uint requiredPromoteLevel = (uint)(i + 1);

            if (targetLevel >= ascensionLevel && currentLevel <= ascensionLevel)
            {
                if (currentLevel == ascensionLevel && currentPromoteLevel >= requiredPromoteLevel)
                {
                    continue;
                }

                if (currentLevel < ascensionLevel ||
                    (currentLevel == ascensionLevel && currentPromoteLevel < requiredPromoteLevel))
                {
                    requiredAscensions.Add(ascensionLevel);
                }
            }
        }

        if (requiredAscensions.Count == 0)
        {
            return (0, new Dictionary<uint, uint>());
        }

        int totalMora = 0;
        Dictionary<uint, uint> items = [];

        foreach (int ascensionLevel in requiredAscensions)
        {
            int ascensionIndex = Array.IndexOf(AscensionLevels, ascensionLevel);
            uint ascensionIndexU = (uint)(ascensionIndex + 1);

            // 添加元素宝石
            uint gemBaseId = ElementNameIconConverter.ElementNameToElementType(avatar.FetterInfo.VisionBefore) switch
            {
                ElementType.Fire => 104111U,
                ElementType.Water => 104121U,
                ElementType.Grass => 104131U,
                ElementType.Electric => 104141U,
                ElementType.Wind => 104151U,
                ElementType.Ice => 104161U,
                ElementType.Rock => 104171U,
                _ => throw HutaoException.NotSupported(),
            };

            // 根据突破等级选择宝石品质和数量
            (uint gemId, uint gemCount) = ascensionIndexU switch
            {
                1 => (gemBaseId, 1u),
                2 => (gemBaseId + 1u, 3u),
                3 => (gemBaseId + 1u, 6u),
                4 => (gemBaseId + 2u, 3u),
                5 => (gemBaseId + 2u, 6u),
                6 => (gemBaseId + 3u, 6u),
                _ => throw HutaoException.Argument("Invalid ascension level", nameof(ascensionIndexU)),
            };
            AddOrUpdateItem(items, gemId, gemCount);

            // 添加摩拉消耗
            totalMora += AvatarAscensionMoraCosts[ascensionIndex];

            // 添加boss材料
            uint bossMaterialCount = AvatarBossMaterialCounts[ascensionIndex];
            if (bossMaterialCount > 0)
            {
                AddOrUpdateItem(items, avatar.CultivationItems[1], bossMaterialCount);
            }

            // 添加特产材料
            uint specialtyCount = AvatarSpecialtyCounts[ascensionIndex];
            AddOrUpdateItem(items, avatar.CultivationItems[2], specialtyCount);

            // 添加怪物材料
            uint highestMaterialId = avatar.CultivationItems[3];
            (uint materialId, uint materialCount) = ascensionIndexU switch
            {
                1 => (highestMaterialId - 2U, 3u),
                2 => (highestMaterialId - 2U, 15u),
                3 => (highestMaterialId - 1U, 12u),
                4 => (highestMaterialId - 1U, 18u),
                5 => (highestMaterialId, 12u),
                6 => (highestMaterialId, 24u),
                _ => throw HutaoException.NotSupported(),
            };
            AddOrUpdateItem(items, materialId, materialCount);
        }

        return (totalMora, items);
    }

    private static (int Mora, Dictionary<uint, uint> Items) CalculateTalentUpgrade(
        MetadataAvatar avatar,
        uint currentLevel,
        uint targetLevel)
    {
        if (currentLevel >= targetLevel || currentLevel >= 10 || targetLevel > 10)
        {
            return (0, new Dictionary<uint, uint>());
        }

        int totalMora = 0;
        Dictionary<uint, uint> items = [];

        uint talentBookId = avatar.CultivationItems[4];
        uint monsterMaterialId = avatar.CultivationItems[3];
        uint weeklyBossMaterialId = avatar.CultivationItems[5];

        for (uint level = currentLevel; level < targetLevel; level++)
        {
            int upgradeIndex = (int)level;

            // 添加摩拉消耗
            totalMora += TalentMoraCosts[upgradeIndex];

            // 添加天赋书
            (uint talentBookMaterialId, uint talentBookCount) = upgradeIndex switch
            {
                1 => (talentBookId - 2U, 3u),
                2 => (talentBookId - 1U, 2u),
                3 => (talentBookId - 1U, 4u),
                4 => (talentBookId - 1U, 6u),
                5 => (talentBookId - 1U, 9u),
                6 => (talentBookId, 4u),
                7 => (talentBookId, 6u),
                8 => (talentBookId, 12u),
                9 => (talentBookId, 16u),
                _ => throw HutaoException.NotSupported(),
            };
            AddOrUpdateItem(items, talentBookMaterialId, talentBookCount);

            // 添加怪物材料
            (uint monsterMaterialItemId, uint monsterMaterialCount) = upgradeIndex switch
            {
                1 => (monsterMaterialId - 2U, 6u),
                2 => (monsterMaterialId - 1U, 3u),
                3 => (monsterMaterialId - 1U, 4u),
                4 => (monsterMaterialId - 1U, 6u),
                5 => (monsterMaterialId - 1U, 9u),
                6 => (monsterMaterialId, 4u),
                7 => (monsterMaterialId, 6u),
                8 => (monsterMaterialId, 9u),
                9 => (monsterMaterialId, 12u),
                _ => throw HutaoException.NotSupported(),
            };
            AddOrUpdateItem(items, monsterMaterialItemId, monsterMaterialCount);

            // 添加周常BOSS材料
            if (upgradeIndex >= 6)
            {
                uint weeklyBossCount = WeeklyBossCounts[upgradeIndex];
                AddOrUpdateItem(items, weeklyBossMaterialId, weeklyBossCount);
            }

            // 添加智识之冠
            if (upgradeIndex == 9)
            {
                AddOrUpdateItem(items, CrownOfInsightId, 1u);
            }
        }

        return (totalMora, items);
    }

    #endregion

    #region Weapon

    private static (int Mora, uint ExpBooks) CalculateWeaponExperience(int starRarity, uint currentLevel, uint targetLevel)
    {
        if (starRarity is < 3 or > 5)
        {
            throw HutaoException.NotSupported($"Weapon star rarity {starRarity} is not supported");
        }

        int requiredExp = WeaponLevelExperience.CalculateTotalExperience(starRarity, (int)currentLevel, (int)targetLevel);
        uint expBooks = (uint)Math.Round((double)requiredExp / PurpleWeaponExp, MidpointRounding.ToPositiveInfinity);
        int mora = (int)(expBooks * PurpleWeaponExpMoraPer);

        return (mora, expBooks);
    }

    private static (int Mora, Dictionary<uint, uint> Items) CalculateWeaponAscension(
        MetadataWeapon weapon,
        uint currentLevel,
        uint targetLevel)
    {
        int starRarity = (int)weapon.RankLevel;
        if (starRarity < 3 || starRarity > 5)
        {
            throw HutaoException.NotSupported($"Weapon star rarity {starRarity} is not supported");
        }

        List<int> requiredAscensions = [];

        foreach (int ascensionLevel in AscensionLevels)
        {
            if (targetLevel >= ascensionLevel && currentLevel <= ascensionLevel)
            {
                requiredAscensions.Add(ascensionLevel);
            }
        }

        if (requiredAscensions.Count == 0)
        {
            return (0, new Dictionary<uint, uint>());
        }

        int totalMora = 0;
        Dictionary<uint, uint> items = [];

        foreach (int ascensionLevel in requiredAscensions)
        {
            uint ascensionIndex = (uint)(Array.IndexOf(AscensionLevels, ascensionLevel) + 1);

            // 添加摩拉消耗
            totalMora += (starRarity, ascensionIndex) switch
            {
                (5, 1) => 10000,
                (5, 2) => 20000,
                (5, 3) => 30000,
                (5, 4) => 45000,
                (5, 5) => 55000,
                (5, 6) => 65000,

                (4, 1) => 5000,
                (4, 2) => 15000,
                (4, 3) => 20000,
                (4, 4) => 30000,
                (4, 5) => 35000,
                (4, 6) => 45000,

                (3, 1) => 5000,
                (3, 2) => 10000,
                (3, 3) => 15000,
                (3, 4) => 20000,
                (3, 5) => 25000,
                (3, 6) => 30000,

                (2, 1) => 5000,
                (2, 2) => 5000,
                (2, 3) => 10000,
                (2, 4) => 15000,

                (1, 1) => 0,
                (1, 2) => 5000,
                (1, 3) => 5000,
                (1, 4) => 10000,

                _ => throw HutaoException.NotSupported(),
            };

            // 添加武器突破材料
            uint weaponMaterialId = weapon.CultivationItems[0];
            (uint weaponMaterialItemId, uint weaponMaterialCount) = (starRarity, ascensionIndex) switch
            {
                (5, 1) => (weaponMaterialId - 3U, 5u),
                (5, 2) => (weaponMaterialId - 2U, 5u),
                (5, 3) => (weaponMaterialId - 2U, 9u),
                (5, 4) => (weaponMaterialId - 1U, 5u),
                (5, 5) => (weaponMaterialId - 1U, 9u),
                (5, 6) => (weaponMaterialId, 6u),

                (4, 1) => (weaponMaterialId - 3U, 3u),
                (4, 2) => (weaponMaterialId - 2U, 3u),
                (4, 3) => (weaponMaterialId - 2U, 6u),
                (4, 4) => (weaponMaterialId - 1U, 3u),
                (4, 5) => (weaponMaterialId - 1U, 6u),
                (4, 6) => (weaponMaterialId, 4u),

                (3, 1) => (weaponMaterialId - 3U, 2u),
                (3, 2) => (weaponMaterialId - 2U, 2u),
                (3, 3) => (weaponMaterialId - 2U, 4u),
                (3, 4) => (weaponMaterialId - 1U, 2u),
                (3, 5) => (weaponMaterialId - 1U, 4u),
                (3, 6) => (weaponMaterialId, 3u),

                // 绿蓝紫 绿蓝 白绿
                (2, 1) => (weaponMaterialId - 2U, 1u),
                (2, 2) => (weaponMaterialId - 1U, 1u),
                (2, 3) => (weaponMaterialId - 1U, 3u),
                (2, 4) => (weaponMaterialId, 1u),

                (1, 1) => (weaponMaterialId - 2U, 1u),
                (1, 2) => (weaponMaterialId - 1U, 1u),
                (1, 3) => (weaponMaterialId - 1U, 2u),
                (1, 4) => (weaponMaterialId, 1u),

                _ => throw HutaoException.NotSupported(),
            };
            AddOrUpdateItem(items, weaponMaterialItemId, weaponMaterialCount);

            // 添加A怪物材料
            uint monsterMaterialA = weapon.CultivationItems[1];
            (uint monsterAItemId, uint monsterACount) = (starRarity, ascensionIndex) switch
            {
                (5, 1) => (monsterMaterialA - 2U, 5u),
                (5, 2) => (monsterMaterialA - 2U, 18u),
                (5, 3) => (monsterMaterialA - 1U, 9u),
                (5, 4) => (monsterMaterialA - 1U, 18u),
                (5, 5) => (monsterMaterialA, 14u),
                (5, 6) => (monsterMaterialA, 27u),

                (4, 1) => (monsterMaterialA - 2U, 3u),
                (4, 2) => (monsterMaterialA - 2U, 12u),
                (4, 3) => (monsterMaterialA - 1U, 6u),
                (4, 4) => (monsterMaterialA - 1U, 12u),
                (4, 5) => (monsterMaterialA, 9u),
                (4, 6) => (monsterMaterialA, 18u),

                (3, 1) => (monsterMaterialA - 2U, 2u),
                (3, 2) => (monsterMaterialA - 2U, 8u),
                (3, 3) => (monsterMaterialA - 1U, 4u),
                (3, 4) => (monsterMaterialA - 1U, 8u),
                (3, 5) => (monsterMaterialA, 6u),
                (3, 6) => (monsterMaterialA, 12u),

                (2, 1) => (monsterMaterialA - 1U, 1u),
                (2, 2) => (monsterMaterialA - 1U, 5u),
                (2, 3) => (monsterMaterialA, 3u),
                (2, 4) => (monsterMaterialA, 5u),

                (1, 1) => (monsterMaterialA - 1U, 1u),
                (1, 2) => (monsterMaterialA - 1U, 4u),
                (1, 3) => (monsterMaterialA, 2u),
                (1, 4) => (monsterMaterialA, 4u),

                _ => throw HutaoException.NotSupported(),
            };
            AddOrUpdateItem(items, monsterAItemId, monsterACount);

            // 添加B怪物材料
            uint monsterMaterialB = weapon.CultivationItems[2];
            (uint monsterBItemId, uint monsterBCount) = (starRarity, ascensionIndex) switch
            {
                (5, 1) => (monsterMaterialB - 2U, 3u),
                (5, 2) => (monsterMaterialB - 2U, 12u),
                (5, 3) => (monsterMaterialB - 1U, 9u),
                (5, 4) => (monsterMaterialB - 1U, 14u),
                (5, 5) => (monsterMaterialB, 9u), (5, 6) => (monsterMaterialB, 18u),

                (4, 1) => (monsterMaterialB - 2U, 2u),
                (4, 2) => (monsterMaterialB - 2U, 8u),
                (4, 3) => (monsterMaterialB - 1U, 6u),
                (4, 4) => (monsterMaterialB - 1U, 9u),
                (4, 5) => (monsterMaterialB, 6u),
                (4, 6) => (monsterMaterialB, 12u),

                (3, 1) => (monsterMaterialB - 2U, 1u),
                (3, 2) => (monsterMaterialB - 2U, 5u),
                (3, 3) => (monsterMaterialB - 1U, 4u),
                (3, 4) => (monsterMaterialB - 1U, 6u),
                (3, 5) => (monsterMaterialB, 4u),
                (3, 6) => (monsterMaterialB, 8u),

                (2, 1) => (monsterMaterialB - 1U, 1u),
                (2, 2) => (monsterMaterialB - 1U, 4u),
                (2, 3) => (monsterMaterialB, 3u),
                (2, 4) => (monsterMaterialB, 4u),

                (1, 1) => (monsterMaterialB - 1U, 1u),
                (1, 2) => (monsterMaterialB - 1U, 2u),
                (1, 3) => (monsterMaterialB, 2u),
                (1, 4) => (monsterMaterialB, 3u),

                _ => throw HutaoException.NotSupported(),
            };
            AddOrUpdateItem(items, monsterBItemId, monsterBCount);
        }

        return (totalMora, items);
    }

    #endregion

    private static void AddOrUpdateItem(Dictionary<uint, uint> items, uint id, uint count)
    {
        if (count is 0)
        {
            return;
        }

        if (items.TryGetValue(id, out uint existingCount))
        {
            items[id] = existingCount + count;
        }
        else
        {
            items[id] = count;
        }
    }

    private static BatchConsumption? CreateBatchConsumption(Dictionary<uint, uint> itemCounts)
    {
        if (itemCounts.Count is 0)
        {
            return default;
        }

        Item[] items = new Item[itemCounts.Count];
        int index = 0;

        foreach ((uint id, uint count) in itemCounts)
        {
            items[index++] = new Item
            {
                Id = id,
                Num = count,
            };
        }

        return BatchConsumption.CreateForOffline(ImmutableArray.Create(items));
    }
}