// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Service.AvatarInfo.Factory;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.InteropServices;
using CalculateConsumption = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.Consumption;
using MetadataAvatar = Snap.Hutao.Model.Metadata.Avatar.Avatar;
using MetadataWeapon = Snap.Hutao.Model.Metadata.Weapon.Weapon;
using static Snap.Hutao.Service.Cultivation.Offline.Lookups;

namespace Snap.Hutao.Service.Cultivation.Offline;

internal static class OfflineCalculator
{
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
        foreach (AvatarPromotionDelta delta in deltas)
        {
            ArgumentNullException.ThrowIfNull(delta.Weapon);

            if (context.GetAvatar(delta.AvatarId) is not { } avatar || context.GetWeapon(delta.Weapon.Id) is not { } weapon)
            {
                continue;
            }

            CalculateConsumption consumption = new()
            {
                // Since we will merge skill consume into avatar consume when saving to db
                // We just calculate them altogether.
                AvatarConsume = CalculateAvatarConsumption(delta, avatar),
                AvatarSkillConsume = [],
                WeaponConsume = CalculateWeaponConsumption(delta, weapon),
            };

            consumptions.Add(consumption);
        }

        return BatchConsumption.CreateForBatch(consumptions.ToImmutable());
    }

    private static ImmutableArray<Item> CalculateAvatarConsumption(AvatarPromotionDelta delta, MetadataAvatar avatar)
    {
        Dictionary<uint, uint> itemCounts = [];

        // 计算角色经验
        if (delta.AvatarLevelCurrent < delta.AvatarLevelTarget)
        {
            CalculateAvatarExperience(delta, itemCounts);
        }

        // 计算角色突破
        if (delta.AvatarLevelCurrent <= delta.AvatarLevelTarget)
        {
            CalculateAvatarAscension(delta, avatar, itemCounts);
        }

        // 计算天赋升级
        if (delta.SkillList.Length > 0)
        {
            CalculateAvatarTalents(delta, avatar, itemCounts);
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

        // 计算武器经验
        if (delta.Weapon.LevelCurrent < delta.Weapon.LevelTarget)
        {
            CalculateWeaponExperience(weapon.RankLevel, delta.Weapon, itemCounts);
        }

        // 计算武器突破
        if (delta.Weapon.LevelCurrent <= delta.Weapon.LevelTarget)
        {
            CalculateWeaponAscension(delta.Weapon, weapon, itemCounts);
        }

        return ConvertToItems(itemCounts);
    }

    private static void CalculateAvatarExperience(AvatarPromotionDelta delta, Dictionary<uint, uint> itemCounts)
    {
        int requiredExp = AvatarLevelExperience.CalculateTotalExperience((int)delta.AvatarLevelCurrent, (int)delta.AvatarLevelTarget);
        uint expItemCount = (uint)Math.Ceiling((double)requiredExp / PurpleExpBookPerItemExp);
        AddOrUpdateItem(itemCounts, MaterialIds.Mora, expItemCount * PurpleExpBookPerItemMora);
        AddOrUpdateItem(itemCounts, MaterialIds.HeroesWit, expItemCount);
    }

    private static void CalculateAvatarAscension(AvatarPromotionDelta delta, MetadataAvatar avatar, Dictionary<uint, uint> itemCounts)
    {
        ImmutableArray<int> requiredAscensions = Ascension.GetRequiredLevels(AscensionLevels, delta);
        if (requiredAscensions.IsEmpty)
        {
            return;
        }

        foreach (int requiredAscension in requiredAscensions)
        {
            int ascensionIndex = AvatarAscensionIndices[requiredAscension];

            // 添加摩拉消耗
            AddOrUpdateItem(itemCounts, MaterialIds.Mora, (uint)AvatarAscensionMoraCosts[ascensionIndex]);

            // 添加元素宝石 根据突破等级选择宝石品质和数量
            ElementType element = ElementNameIconConverter.ElementNameToElementType(avatar.FetterInfo.Vision);
            AddOrUpdateItem(itemCounts, AvatarGemConsumption[ascensionIndex].Positive(ElementGemBaseIds[element]));

            // 添加 BOSS 材料
            AddOrUpdateItem(itemCounts, avatar.CultivationItems[1], AvatarBossMaterialCounts[ascensionIndex]);

            // 添加特产材料
            AddOrUpdateItem(itemCounts, avatar.CultivationItems[2], AvatarSpecialtyCounts[ascensionIndex]);

            // 添加怪物材料
            AddOrUpdateItem(itemCounts, AvatarMonsterMaterialConsumption[ascensionIndex].Negative(avatar.CultivationItems[3]));
        }
    }

    private static void CalculateAvatarTalents(AvatarPromotionDelta delta, MetadataAvatar avatar, Dictionary<uint, uint> itemCounts)
    {
        foreach (PromotionDelta skill in delta.SkillList)
        {
            if (skill.LevelCurrent >= skill.LevelTarget)
            {
                continue;
            }

            CalculateAvatarTalent(skill, avatar, itemCounts);
        }
    }

    private static void CalculateAvatarTalent(PromotionDelta delta, MetadataAvatar avatar, Dictionary<uint, uint> itemCounts)
    {
        uint currentLevel = delta.LevelCurrent;
        uint targetLevel = delta.LevelTarget;

        if (currentLevel >= targetLevel || currentLevel >= 10 || targetLevel > 10)
        {
            return;
        }

        foreach (int upgradeIndex in Enumerable.Range((int)currentLevel, (int)(targetLevel - currentLevel)))
        {
            // 添加摩拉消耗
            AddOrUpdateItem(itemCounts, MaterialIds.Mora, (uint)TalentMoraCosts[upgradeIndex]);

            // 添加天赋书
            AddOrUpdateItem(itemCounts, TalentBookConsumption[upgradeIndex].Negative(avatar.CultivationItems[4]));

            // 添加怪物材料
            AddOrUpdateItem(itemCounts, TalentMonsterMaterialConsumption[upgradeIndex].Negative(avatar.CultivationItems[3]));

            // 添加周常BOSS材料
            AddOrUpdateItem(itemCounts, avatar.CultivationItems[5], WeeklyBossCounts[upgradeIndex]);

            // 添加智识之冠
            if (upgradeIndex is 9)
            {
                AddOrUpdateItem(itemCounts, MaterialIds.CrownOfInsight, 1U);
            }
        }
    }

    private static void CalculateWeaponExperience(QualityType quality, PromotionDelta delta, Dictionary<uint, uint> itemCounts)
    {
        int requiredExp = WeaponLevelExperience.CalculateTotalExperience(quality, (int)delta.LevelCurrent, (int)delta.LevelTarget);
        uint expItemCount = (uint)Math.Ceiling((double)requiredExp / PurpleWeaponOrePerItemExp);
        AddOrUpdateItem(itemCounts, MaterialIds.Mora, expItemCount * PurpleWeaponOrePerItemMora);
        AddOrUpdateItem(itemCounts, MaterialIds.MysticEnhancementOre, expItemCount);
    }

    private static void CalculateWeaponAscension(PromotionDelta delta, MetadataWeapon weapon, Dictionary<uint, uint> itemCounts)
    {
        QualityType quality = weapon.RankLevel;

        ImmutableArray<int> requiredAscensions = Ascension.GetRequiredLevels(AscensionLevels, delta);
        Debug.Assert(weapon.RankLevel >= QualityType.QUALITY_BLUE || !requiredAscensions.Contains(80));

        if (requiredAscensions.IsEmpty)
        {
            return;
        }

        foreach (int requiredAscension in requiredAscensions)
        {
            int ascensionIndex = AscensionLevels.IndexOf(requiredAscension);

            // 添加摩拉消耗
            AddOrUpdateItem(itemCounts, MaterialIds.Mora, (uint)WeaponAscensionMoraCosts[quality][ascensionIndex]);

            // 添加武器突破材料
            AddOrUpdateItem(itemCounts, WeaponMaterialConsumption[quality][ascensionIndex].Negative(weapon.CultivationItems[0]));

            // 添加A怪物材料
            AddOrUpdateItem(itemCounts, WeaponMonsterMaterialAConsumption[quality][ascensionIndex].Negative(weapon.CultivationItems[1]));

            // 添加B怪物材料
            AddOrUpdateItem(itemCounts, WeaponMonsterMaterialBConsumption[quality][ascensionIndex].Negative(weapon.CultivationItems[2]));
        }
    }

    private static void AddOrUpdateItem(Dictionary<uint, uint> items, uint id, uint count)
    {
        if (count is 0)
        {
            return;
        }

        CollectionsMarshal.GetValueRefOrAddDefault(items, id, out _) += count;
    }

    private static void AddOrUpdateItem(Dictionary<uint, uint> items, (uint Id, uint Count) tuple)
    {
        AddOrUpdateItem(items, tuple.Id, tuple.Count);
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