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

        // 摩拉
        if (Id == 202U)
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
        // 摩拉 大英雄的经验
        if (Id == 202U || Id == 104003U)
        {
            return true;
        }

        // 无类型 智识之冕
        if (TypeDescription is null || Id == 104319U)
        {
            return false;
        }

        return IntrinsicFrozen.ResinMaterialTypeDescriptions.Contains(TypeDescription);
    }

    public bool IsTodaysItem(in TimeSpan offset, bool treatSundayAsTrue = false)
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