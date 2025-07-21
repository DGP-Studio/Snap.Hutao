// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Intrinsic.Frozen;
using Snap.Hutao.ViewModel.Cultivation;
using System.Text.RegularExpressions;

namespace Snap.Hutao.Model.Metadata.Item;

internal sealed class Material : DisplayItem
{
    public const uint Mora = 202U;                      // 摩拉
    public const uint WanderersAdvice = 104001U;        // 流浪者的经验
    public const uint AdventurersExperience = 104002U;  // 冒险家的经验
    public const uint HeroesWit = 104003U;              // 大英雄的经验
    public const uint MysticEnhancementOre = 104013U;   // 精锻用魔矿
    public const uint AgnidusAgateSliver = 104111U;     // 燃愿玛瑙碎屑
    public const uint VarunadaLazuriteSliver = 104121U; // 涤净青金碎屑
    public const uint NagadusEmeraldSliver = 104131U;   // 生长碧翡碎屑
    public const uint VajradaAmethystSliver = 104141U;  // 最胜紫晶碎屑
    public const uint VayudaTurquoiseSliver = 104151U;  // 自在松石碎屑
    public const uint ShivadaJadeSliver = 104161U;      // 哀叙冰玉碎屑
    public const uint PrithivaTopazSliver = 104171U;    // 坚牢黄玉碎屑
    public const uint CrownOfInsight = 104319U;         // 智识之冕

    public static readonly Material Default = new()
    {
        Name = "？？？",
        Id = default,
        RankLevel = default,
        ItemType = default,
        Icon = default!,
        Description = "？？？",
        TypeDescription = "？？？",
    };

    public MaterialType MaterialType { get; init; }

    public string? EffectDescription { get; init; }

    public bool IsInventoryItem()
    {
        // 原质
        if (Id == 112001U)
        {
            return false;
        }

        // 摩拉
        if (Id == Mora)
        {
            return true;
        }

        if (TypeDescription is null)
        {
            return false;
        }

        if (Regex.IsMatch(TypeDescription, SHRegex.ModelMetadataMaterialLocalSpecialtyRegex))
        {
            return true;
        }

        // Character and Weapon Enhancement Material // 怪物掉落
        // Character EXP Material                    // 经验书
        // Character Ascension Material              // 元素晶石
        // Character Talent Material                 // 天赋本
        // Character Level-Up Material               // 40体BOSS/周本掉落
        // Weapon Enhancement Material               // 魔矿
        // Weapon Ascension Material                 // 武器本
        return IntrinsicFrozen.MaterialTypeDescriptions.Contains(TypeDescription);
    }

    public bool IsResinItem()
    {
        if (Id == Mora || Id == WanderersAdvice || Id == AdventurersExperience || Id == HeroesWit)
        {
            return true;
        }

        // 无类型 智识之冕
        if (TypeDescription is null || Id == CrownOfInsight)
        {
            return false;
        }

        return IntrinsicFrozen.ResinMaterialTypeDescriptions.Contains(TypeDescription);
    }

    public bool IsItemOfToday(in TimeSpan offset, bool treatSundayAsTrue = false)
    {
        return (DateTimeOffset.Now.ToOffset(offset) - new TimeSpan(4, 0, 0)).DayOfWeek switch
        {
            DayOfWeek.Monday or DayOfWeek.Thursday => MaterialIds.MondayThursdayItems.Contains(Id),
            DayOfWeek.Tuesday or DayOfWeek.Friday => MaterialIds.TuesdayFridayItems.Contains(Id),
            DayOfWeek.Wednesday or DayOfWeek.Saturday => MaterialIds.WednesdaySaturdayItems.Contains(Id),
            _ => treatSundayAsTrue && (MaterialIds.MondayThursdayItems.Contains(Id) || MaterialIds.TuesdayFridayItems.Contains(Id) || MaterialIds.WednesdaySaturdayItems.Contains(Id)),
        };
    }

    public DaysOfWeek GetDaysOfWeek()
    {
        return MaterialIds.GetDaysOfWeek(Id);
    }
}