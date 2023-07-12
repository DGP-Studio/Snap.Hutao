// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.Cultivation;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata.Item;

/// <summary>
/// 材料
/// </summary>
internal sealed class Material : DisplayItem
{
    public static readonly Material Default = new()
    {
        Name = "？？？",
    };

    /// <summary>
    /// 材料类型
    /// </summary>
    public MaterialType MaterialType { get; set; }

    /// <summary>
    /// 效果描述
    /// </summary>
    public string? EffectDescription { get; set; }

    /// <summary>
    /// 判断是否为物品栏物品
    /// </summary>
    /// <returns>是否为物品栏物品</returns>
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

        // TODO: support non-CHS
        if (TypeDescription.EndsWith("区域特产"))
        {
            return true;
        }

        // TODO: support non-CHS
        return TypeDescription switch
        {
            "角色与武器培养素材" => true, // 怪物掉落
            "角色经验素材" => true,      // 经验书
            "角色突破素材" => true,      // 元素晶石
            "角色天赋素材" => true,      // 天赋本
            "角色培养素材" => true,      // 40体BOSS/周本掉落
            "武器强化素材" => true,      // 魔矿
            "武器突破素材" => true,      // 武器本
            _ => false,
        };
    }

    /// <summary>
    /// 判断是否为当日物品
    /// O(1) 操作
    /// </summary>
    /// <param name="treatSundayAsTrue">星期日视为当日材料</param>
    /// <returns>是否为当日物品</returns>
    public bool IsTodaysItem(bool treatSundayAsTrue = false)
    {
        return DateTimeOffset.UtcNow.AddHours(4).DayOfWeek switch
        {
            DayOfWeek.Monday or DayOfWeek.Thursday => Materials.MondayThursdayItems.Contains(Id),
            DayOfWeek.Tuesday or DayOfWeek.Friday => Materials.TuesdayFridayItems.Contains(Id),
            DayOfWeek.Wednesday or DayOfWeek.Saturday => Materials.WednesdaySaturdayItems.Contains(Id),
            _ => false,
        };
    }

    /// <summary>
    /// 获取物品对应的 DaysOfWeek
    /// </summary>
    /// <returns>DaysOfWeek</returns>
    public DaysOfWeek GetDaysOfWeek()
    {
        if (Materials.MondayThursdayItems.Contains(Id))
        {
            return DaysOfWeek.MondayAndThursday;
        }

        if (Materials.TuesdayFridayItems.Contains(Id))
        {
            return DaysOfWeek.TuesdayAndFriday;
        }

        if (Materials.WednesdaySaturdayItems.Contains(Id))
        {
            return DaysOfWeek.WednesdayAndSaturday;
        }

        return DaysOfWeek.Any;
    }
}