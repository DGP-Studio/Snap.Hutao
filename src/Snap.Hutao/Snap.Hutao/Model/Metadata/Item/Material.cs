// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Intrinsic.Frozen;
using Snap.Hutao.ViewModel.Cultivation;
using System.Text.RegularExpressions;

namespace Snap.Hutao.Model.Metadata.Item;

internal sealed class Material : DisplayItem
{
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

        // 摩拉 无主的命星
        if (Id == MaterialIds.Mora || Id == MaterialIds.MasterlessStellaFortuna)
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

        // Character and Weapon Enhancement Material // 角色与武器培养素材 | 普通/精英怪物掉落
        // Character EXP Material                    // 角色经验素材 | 经验书
        // Character Ascension Material              // 角色突破素材 | 元素晶石
        // Character Talent Material                 // 角色天赋素材 | 天赋本
        // Character Level-Up Material               // 角色培养素材 | 40体BOSS/周本掉落
        // Weapon Enhancement Material               // 武器强化素材 | 魔矿
        // Weapon Ascension Material                 // 武器突破素材 | 武器本
        return IntrinsicFrozen.MaterialTypeDescriptions.Contains(TypeDescription);
    }

    public bool IsResinItem()
    {
        if (Id == MaterialIds.Mora || Id == MaterialIds.WanderersAdvice || Id == MaterialIds.AdventurersExperience || Id == MaterialIds.HeroesWit)
        {
            return true;
        }

        // 无类型 智识之冕
        if (TypeDescription is null || Id == MaterialIds.CrownOfInsight)
        {
            return false;
        }

        return IntrinsicFrozen.ResinMaterialTypeDescriptions.Contains(TypeDescription);
    }

    public bool IsItemOfToday(in TimeSpan offset, bool treatSundayAsTrue = false)
    {
        return (DateTimeOffset.Now.ToOffset(offset) - TimeSpan.FromHours(4)).DayOfWeek switch
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