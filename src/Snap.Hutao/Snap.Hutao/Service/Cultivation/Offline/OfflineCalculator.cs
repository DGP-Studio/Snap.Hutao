// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Service.AvatarInfo.Factory;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CalculateConsumption = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.Consumption;
using MetadataAvatar = Snap.Hutao.Model.Metadata.Avatar.Avatar;
using MetadataWeapon = Snap.Hutao.Model.Metadata.Weapon.Weapon;

namespace Snap.Hutao.Service.Cultivation.Offline;

internal static class OfflineCalculator
{
    private const int PurpleExpBookExp = 20000;
    private const int PurpleWeaponExp = 10000;

    private const int PurpleExpBookMoraPer = 4000;
    private const int PurpleWeaponExpMoraPer = 1000;

    // 突破等级
    private static readonly ImmutableArray<int> AscensionLevels = [20, 40, 50, 60, 70, 80];

    // 角色等级突破相关
    private static readonly ImmutableArray<int> AvatarAscensionMoraCosts = [20000, 40000, 60000, 80000, 100000, 120000];
    private static readonly ImmutableArray<uint> AvatarBossMaterialCounts = [0, 2, 4, 8, 12, 20];
    private static readonly ImmutableArray<uint> AvatarSpecialtyCounts = [3, 10, 20, 30, 45, 60];

    // 角色天赋升级相关
    private static readonly ImmutableArray<int> TalentMoraCosts = [0, 12500, 17500, 25000, 30000, 37500, 120000, 260000, 450000, 700000];
    private static readonly ImmutableArray<uint> WeeklyBossCounts = [0, 0, 0, 0, 0, 0, 1, 1, 2, 2];

    // 角色突破素材（元素石）基础 ID 映射
    private static readonly FrozenDictionary<ElementType, uint> ElementGemBaseIds = FrozenDictionary.ToFrozenDictionary(
    [
        KeyValuePair.Create(ElementType.Fire, Material.AgnidusAgateSliver),
        KeyValuePair.Create(ElementType.Water, Material.VarunadaLazuriteSliver),
        KeyValuePair.Create(ElementType.Grass, Material.NagadusEmeraldSliver),
        KeyValuePair.Create(ElementType.Electric, Material.VajradaAmethystSliver),
        KeyValuePair.Create(ElementType.Wind, Material.VayudaTurquoiseSliver),
        KeyValuePair.Create(ElementType.Ice, Material.ShivadaJadeSliver),
        KeyValuePair.Create(ElementType.Rock, Material.PrithivaTopazSliver),
    ]);

    // 角色突破素材（元素石） 消耗查找表 [突破次数-1] -> (品质偏移, 数量)
    private static readonly ImmutableArray<QualityOffsetCount> AvatarGemConsumption =
    [
        new(0u, 1u), // 突破1次
        new(1u, 3u), // 突破2次
        new(1u, 6u), // 突破3次
        new(2u, 3u), // 突破4次
        new(2u, 6u), // 突破5次
        new(3u, 6u), // 突破6次
    ];

    // 角色突破怪物材料 消耗查找表 [突破次数-1] -> (品质偏移, 数量)
    private static readonly ImmutableArray<QualityOffsetCount> AvatarMonsterMaterialConsumption =
    [
        new(2u, 3u), // 突破1次
        new(2u, 15u), // 突破2次
        new(1u, 12u), // 突破3次
        new(1u, 18u), // 突破4次
        new(0u, 12u), // 突破5次
        new(0u, 24u), // 突破6次
    ];

    // 角色天赋素材 消耗查找表 [等级-1] -> (品质偏移, 数量)
    private static readonly ImmutableArray<QualityOffsetCount> TalentBookConsumption =
    [
        new(0, 0u), // 等级0 (不使用)
        new(2u, 3u), // 等级1
        new(1u, 2u), // 等级2
        new(1u, 4u), // 等级3
        new(1u, 6u), // 等级4
        new(1u, 9u), // 等级5
        new(0u, 4u), // 等级6
        new(0u, 6u), // 等级7
        new(0u, 12u), // 等级8
        new(0u, 16u), // 等级9
    ];

    // 角色天赋怪物材料 消耗查找表 [等级-1] -> (品质偏移, 数量)
    private static readonly ImmutableArray<QualityOffsetCount> TalentMonsterMaterialConsumption =
    [
        new(0, 0u), // 等级0 (不使用)
        new(2u, 6u), // 等级1
        new(1u, 3u), // 等级2
        new(1u, 4u), // 等级3
        new(1u, 6u), // 等级4
        new(1u, 9u), // 等级5
        new(0u, 4u), // 等级6
        new(0u, 6u), // 等级7
        new(0u, 9u), // 等级8
        new(0u, 12u), // 等级9
    ];

    // 武器突破摩拉消耗查找表
    private static readonly FrozenDictionary<QualityType, ImmutableArray<int>> WeaponAscensionMoraCosts = FrozenDictionary.ToFrozenDictionary(
    [
        KeyValuePair.Create(QualityType.QUALITY_WHITE, ImmutableArray.Create(0, 5000, 5000, 10000, 0, 0)),
        KeyValuePair.Create(QualityType.QUALITY_GREEN, ImmutableArray.Create(5000, 5000, 10000, 15000, 0, 0)),
        KeyValuePair.Create(QualityType.QUALITY_BLUE, ImmutableArray.Create(5000, 10000, 15000, 20000, 25000, 30000)),
        KeyValuePair.Create(QualityType.QUALITY_PURPLE, ImmutableArray.Create(5000, 15000, 20000, 30000, 35000, 45000)),
        KeyValuePair.Create(QualityType.QUALITY_ORANGE, ImmutableArray.Create(10000, 20000, 30000, 45000, 55000, 65000)),
    ]);

    // 武器突破素材消耗的查找表
    private static readonly FrozenDictionary<QualityType, ImmutableArray<QualityOffsetCount>> WeaponMaterialConsumption = FrozenDictionary.ToFrozenDictionary(
    [
        KeyValuePair.Create(QualityType.QUALITY_WHITE, ImmutableArray.Create<QualityOffsetCount>(new(2u, 1u), new(1u, 1u), new(1u, 2u), new(0u, 1u), new(0u, 0u), new(0u, 0u))),
        KeyValuePair.Create(QualityType.QUALITY_GREEN, ImmutableArray.Create<QualityOffsetCount>(new(2u, 1u), new(1u, 1u), new(1u, 3u), new(0u, 1u), new(0u, 0u), new(0u, 0u))),
        KeyValuePair.Create(QualityType.QUALITY_BLUE, ImmutableArray.Create<QualityOffsetCount>(new(3u, 2u), new(2u, 2u), new(2u, 4u), new(1u, 2u), new(1u, 4u), new(0u, 3u))),
        KeyValuePair.Create(QualityType.QUALITY_PURPLE, ImmutableArray.Create<QualityOffsetCount>(new(3u, 3u), new(2u, 3u), new(2u, 6u), new(1u, 3u), new(1u, 6u), new(0u, 4u))),
        KeyValuePair.Create(QualityType.QUALITY_ORANGE, ImmutableArray.Create<QualityOffsetCount>(new(3u, 5u), new(2u, 5u), new(2u, 9u), new(1u, 5u), new(1u, 9u), new(0u, 6u))),
    ]);

    // 武器精英怪物材料消耗查找表
    private static readonly FrozenDictionary<QualityType, ImmutableArray<QualityOffsetCount>> WeaponMonsterMaterialAConsumption = FrozenDictionary.ToFrozenDictionary(
    [
        KeyValuePair.Create(QualityType.QUALITY_WHITE, ImmutableArray.Create<QualityOffsetCount>(new(1u, 1u), new(1u, 4u), new(0u, 2u), new(0u, 4u), new(0u, 0u), new(0u, 0u))),
        KeyValuePair.Create(QualityType.QUALITY_GREEN, ImmutableArray.Create<QualityOffsetCount>(new(1u, 1u), new(1u, 5u), new(0u, 3u), new(0u, 5u), new(0u, 0u), new(0u, 0u))),
        KeyValuePair.Create(QualityType.QUALITY_BLUE, ImmutableArray.Create<QualityOffsetCount>(new(2u, 2u), new(2u, 8u), new(1u, 4u), new(1u, 8u), new(0u, 6u), new(0u, 12u))),
        KeyValuePair.Create(QualityType.QUALITY_PURPLE, ImmutableArray.Create<QualityOffsetCount>(new(2u, 3u), new(2u, 12u), new(1u, 6u), new(1u, 12u), new(0u, 9u), new(0u, 18u))),
        KeyValuePair.Create(QualityType.QUALITY_ORANGE, ImmutableArray.Create<QualityOffsetCount>(new(2u, 5u), new(2u, 18u), new(1u, 9u), new(1u, 18u), new(0u, 14u), new(0u, 27u))),
    ]);

    // 武器普通怪物材料消耗查找表
    private static readonly FrozenDictionary<QualityType, ImmutableArray<QualityOffsetCount>> WeaponMonsterMaterialBConsumption = FrozenDictionary.ToFrozenDictionary(
    [
        KeyValuePair.Create(QualityType.QUALITY_WHITE, ImmutableArray.Create<QualityOffsetCount>(new(1u, 1u), new(1u, 2u), new(0u, 2u), new(0u, 3u), new(0u, 0u), new(0u, 0u))),
        KeyValuePair.Create(QualityType.QUALITY_GREEN, ImmutableArray.Create<QualityOffsetCount>(new(1u, 1u), new(1u, 4u), new(0u, 3u), new(0u, 4u), new(0u, 0u), new(0u, 0u))),
        KeyValuePair.Create(QualityType.QUALITY_BLUE, ImmutableArray.Create<QualityOffsetCount>(new(2u, 1u), new(2u, 5u), new(1u, 4u), new(1u, 6u), new(0u, 4u), new(0u, 8u))),
        KeyValuePair.Create(QualityType.QUALITY_PURPLE, ImmutableArray.Create<QualityOffsetCount>(new(2u, 2u), new(2u, 8u), new(1u, 6u), new(1u, 9u), new(0u, 6u), new(0u, 12u))),
        KeyValuePair.Create(QualityType.QUALITY_ORANGE, ImmutableArray.Create<QualityOffsetCount>(new(2u, 3u), new(2u, 12u), new(1u, 9u), new(1u, 14u), new(0u, 9u), new(0u, 18u))),
    ]);

    public static BatchConsumption CalculateWikiAvatarConsumption(AvatarPromotionDelta delta, MetadataAvatar avatar)
    {
        return BatchConsumption.CreateForWiki(CalculateAvatarConsumption(delta, avatar));
    }

    public static BatchConsumption CalculateWikiWeaponConsumption(AvatarPromotionDelta delta, MetadataWeapon weapon)
    {
        return BatchConsumption.CreateForWiki(CalculateWeaponConsumption(delta, weapon));
    }

    public static BatchConsumption CalculateBatchConsumption(AvatarPromotionDelta delta, SummaryFactoryMetadataContext context)
    {
        return CalculateBatchConsumption([delta], context);
    }

    public static BatchConsumption CalculateBatchConsumption(ImmutableArray<AvatarPromotionDelta> deltas, SummaryFactoryMetadataContext context)
    {
        ImmutableArray<CalculateConsumption>.Builder consumptions = ImmutableArray.CreateBuilder<CalculateConsumption>(deltas.Length);
        foreach (ref readonly AvatarPromotionDelta delta in deltas.AsSpan())
        {
            ArgumentNullException.ThrowIfNull(delta.Weapon);

            if (context.GetAvatar(delta.AvatarId) is not { } avatar || context.GetWeapon(delta.Weapon.Id) is not { } weapon)
            {
                continue;
            }

            CalculateConsumption consumption = new()
            {
                AvatarConsume = CalculateAvatarConsumption(delta, avatar),
                AvatarSkillConsume = [],
                WeaponConsume = CalculateWeaponConsumption(delta, weapon),
            };

            consumptions.Add(consumption);
        }

        return BatchConsumption.CreateForBatch(consumptions.ToImmutable());
    }

    #region Consumption

    private static ImmutableArray<Item> CalculateAvatarConsumption(AvatarPromotionDelta delta, MetadataAvatar avatar)
    {
        Dictionary<uint, uint> itemCounts = [];
        int totalMora = 0;

        // 计算角色经验
        if (delta.AvatarLevelCurrent < delta.AvatarLevelTarget)
        {
            (int expMora, uint expBooks) = CalculateAvatarExperience(delta.AvatarLevelCurrent, delta.AvatarLevelTarget);
            totalMora += expMora;
            AddOrUpdateItem(itemCounts, Material.HeroesWit, expBooks);
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
            foreach (ref readonly PromotionDelta skill in delta.SkillList.AsSpan())
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
            AddOrUpdateItem(itemCounts, Material.Mora, (uint)totalMora);
        }

        return ConvertToItems(itemCounts);
    }

    private static ImmutableArray<Item> CalculateWeaponConsumption(AvatarPromotionDelta delta, MetadataWeapon weapon)
    {
        if (delta.Weapon is null)
        {
            return [];
        }

        Dictionary<uint, uint> itemCounts = [];
        int totalMora = 0;

        // 计算武器经验
        if (delta.Weapon.LevelCurrent < delta.Weapon.LevelTarget)
        {
            (int expMora, uint expBooks) = CalculateWeaponExperience(weapon.RankLevel, delta.Weapon.LevelCurrent, delta.Weapon.LevelTarget);
            totalMora += expMora;
            AddOrUpdateItem(itemCounts, Material.MysticEnhancementOre, expBooks);
        }

        // 计算武器突破
        if (delta.Weapon.LevelCurrent <= delta.Weapon.LevelTarget)
        {
            (int ascensionMora, Dictionary<uint, uint> ascensionItems) = CalculateWeaponAscension(
                weapon,
                delta.Weapon.LevelCurrent,
                delta.Weapon.LevelTarget,
                delta.Weapon.WeaponPromoteLevel);

            totalMora += ascensionMora;
            foreach ((uint id, uint count) in ascensionItems)
            {
                AddOrUpdateItem(itemCounts, id, count);
            }
        }

        // 添加摩拉
        if (totalMora > 0)
        {
            AddOrUpdateItem(itemCounts, Material.Mora, (uint)totalMora);
        }

        return ConvertToItems(itemCounts);
    }

    #endregion

    #region Avatar

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
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

            // This make sure (currentLevel <= ascensionLevel <= targetLevel)
            if (currentLevel > ascensionLevel || ascensionLevel > targetLevel)
            {
                continue;
            }

            // This make sure (currentLevel == ascensionLevel && currentPromoteLevel < requiredPromoteLevel) || currentLevel < ascensionLevel
            if (currentLevel == ascensionLevel && currentPromoteLevel >= requiredPromoteLevel)
            {
                continue;
            }

            requiredAscensions.Add(ascensionLevel);
        }

        if (requiredAscensions.Count is 0)
        {
            return (0, []);
        }

        int totalMora = 0;
        Dictionary<uint, uint> items = [];

        foreach (ref readonly int requiredAscension in CollectionsMarshal.AsSpan(requiredAscensions))
        {
            int ascensionIndex = AscensionLevels.IndexOf(requiredAscension);

            // 添加元素宝石
            uint gemBaseId = ElementGemBaseIds[ElementNameIconConverter.ElementNameToElementType(avatar.FetterInfo.VisionBefore)];

            // 根据突破等级选择宝石品质和数量
            (uint gemOffset, uint gemCount) = AvatarGemConsumption[ascensionIndex];
            AddOrUpdateItem(items, gemBaseId + gemOffset, gemCount);

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
            (uint materialId, uint materialCount) = AvatarMonsterMaterialConsumption[ascensionIndex];
            AddOrUpdateItem(items, highestMaterialId - materialId, materialCount);
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
            return (0, []);
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
            (uint bookQualityOffset, uint bookCount) = TalentBookConsumption[upgradeIndex];
            AddOrUpdateItem(items, talentBookId - bookQualityOffset, bookCount);

            // 添加怪物材料
            (uint monsterQualityOffset, uint monsterCount) = TalentMonsterMaterialConsumption[upgradeIndex];
            AddOrUpdateItem(items, monsterMaterialId - monsterQualityOffset, monsterCount);

            // 添加周常BOSS材料
            if (upgradeIndex >= 6)
            {
                uint weeklyBossCount = WeeklyBossCounts[upgradeIndex];
                AddOrUpdateItem(items, weeklyBossMaterialId, weeklyBossCount);
            }

            // 添加智识之冠
            if (upgradeIndex is 9)
            {
                AddOrUpdateItem(items, Material.CrownOfInsight, 1u);
            }
        }

        return (totalMora, items);
    }

    #endregion

    #region Weapon

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static (int Mora, uint ExpBooks) CalculateWeaponExperience(QualityType quality, uint currentLevel, uint targetLevel)
    {
        int requiredExp = WeaponLevelExperience.CalculateTotalExperience(quality, (int)currentLevel, (int)targetLevel);
        uint expBooks = (uint)Math.Round((double)requiredExp / PurpleWeaponExp, MidpointRounding.ToPositiveInfinity);
        int mora = (int)(expBooks * PurpleWeaponExpMoraPer);

        return (mora, expBooks);
    }

    private static (int Mora, Dictionary<uint, uint> Items) CalculateWeaponAscension(MetadataWeapon weapon, uint currentLevel, uint targetLevel, uint currentPromoteLevel)
    {
        QualityType quality = weapon.RankLevel;

        List<int> requiredAscensions = [];

        for (int i = 0; i < AscensionLevels.Length; i++)
        {
            int ascensionLevel = AscensionLevels[i];
            if (ascensionLevel >= 70 && quality < QualityType.QUALITY_BLUE)
            {
                break;
            }

            uint requiredPromoteLevel = (uint)(i + 1);

            if (targetLevel >= ascensionLevel && currentLevel <= ascensionLevel)
            {
                if (currentLevel == ascensionLevel && currentPromoteLevel >= requiredPromoteLevel)
                {
                    continue;
                }

                if (currentLevel < ascensionLevel || (currentLevel == ascensionLevel && currentPromoteLevel < requiredPromoteLevel))
                {
                    requiredAscensions.Add(ascensionLevel);
                }
            }
        }

        if (requiredAscensions.Count is 0)
        {
            return (0, []);
        }

        int totalMora = 0;
        Dictionary<uint, uint> items = [];

        foreach (ref readonly int requiredAscension in CollectionsMarshal.AsSpan(requiredAscensions))
        {
            int ascensionIndex = AscensionLevels.IndexOf(requiredAscension);

            // 添加摩拉消耗
            totalMora += WeaponAscensionMoraCosts[quality][ascensionIndex];

            // 添加武器突破材料
            uint weaponMaterialId = weapon.CultivationItems[0];
            (uint materialQualityOffset, uint materialCount) = WeaponMaterialConsumption[quality][ascensionIndex];
            AddOrUpdateItem(items, weaponMaterialId - materialQualityOffset, materialCount);

            // 添加A怪物材料
            uint monsterMaterialA = weapon.CultivationItems[1];
            (uint monsterAQualityOffset, uint monsterACount) = WeaponMonsterMaterialAConsumption[quality][ascensionIndex];
            AddOrUpdateItem(items, monsterMaterialA - monsterAQualityOffset, monsterACount);

            // 添加B怪物材料
            uint monsterMaterialB = weapon.CultivationItems[2];
            (uint monsterBQualityOffset, uint monsterBCount) = WeaponMonsterMaterialBConsumption[quality][ascensionIndex];
            AddOrUpdateItem(items, monsterMaterialB - monsterBQualityOffset, monsterBCount);
        }

        return (totalMora, items);
    }

    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static void AddOrUpdateItem(Dictionary<uint, uint> items, uint id, uint count)
    {
        if (count is 0)
        {
            return;
        }

        CollectionsMarshal.GetValueRefOrAddDefault(items, id, out _) += count;
    }

    private static ImmutableArray<Item> ConvertToItems(Dictionary<uint, uint> itemCounts)
    {
        if (itemCounts.Count is 0)
        {
            return [];
        }

        ImmutableArray<Item>.Builder builder = ImmutableArray.CreateBuilder<Item>(itemCounts.Count);
        foreach ((uint id, uint count) in itemCounts)
        {
            builder.Add(new()
            {
                Id = id,
                Num = count,
            });
        }

        return builder.ToImmutable();
    }
}