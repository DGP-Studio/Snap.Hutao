// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata;

/// <summary>
/// 材料
/// </summary>
public class Material
{
    private static readonly ImmutableHashSet<MaterialId> MondayThursdayItems = new HashSet<MaterialId>
    {
        104301, 104302, 104303, // 「自由」
        104310, 104311, 104312, // 「繁荣」
        104320, 104321, 104322, // 「浮世」
        104329, 104330, 104331, // 「诤言」
        114001, 114002, 114003, 114004, // 高塔孤王
        114013, 114014, 114015, 114016, // 孤云寒林
        114025, 114026, 114027, 114028, // 远海夷地
        114037, 114038, 114039, 114040, // 谧林涓露
    }.ToImmutableHashSet();

    private static readonly ImmutableHashSet<MaterialId> TuesdayFridayItems = new HashSet<MaterialId>
    {
        104304, 104305, 104306, // 「抗争」
        104313, 104314, 104315, // 「勤劳」
        104323, 104324, 104325, // 「风雅」
        104332, 104333, 104334, // 「巧思」
        114005, 114006, 114007, 114008, // 凛风奔狼
        114017, 114018, 114019, 114020, // 雾海云间
        114029, 114030, 114031, 114032, // 鸣神御灵
        114041, 114042, 114043, 114044, // 绿洲花园
    }.ToImmutableHashSet();

    private static readonly ImmutableHashSet<MaterialId> WednesdaySaturdayItems = new HashSet<MaterialId>
    {
        104307, 104308, 104309, // 「诗文」
        104316, 104317, 104318, // 「黄金」
        104326, 104327, 104328, // 「天光」
        104335, 104336, 104337, // 「笃行」
        114009, 114010, 114011, 114012, // 狮牙斗士
        114021, 114022, 114023, 114024, // 漆黑陨铁
        114033, 114034, 114035, 114036, // 今昔剧画
        114045, 114046, 114047, 114048, // 谧林涓露
    }.ToImmutableHashSet();

    /// <summary>
    /// 物品Id
    /// </summary>
    public MaterialId Id { get; set; }

    /// <summary>
    /// 等级
    /// </summary>
    public ItemQuality RankLevel { get; set; }

    /// <summary>
    /// 物品类型
    /// </summary>
    public ItemType ItemType { get; set; }

    /// <summary>
    /// 材料类型
    /// </summary>
    public MaterialType MaterialType { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    public string Icon { get; set; } = default!;

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; } = default!;

    /// <summary>
    /// 类型描述
    /// </summary>
    public string TypeDescription { get; set; } = default!;

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
        if (Id == 112001)
        {
            return false;
        }

        // 摩拉
        if (Id == 202)
        {
            return true;
        }

        if (TypeDescription.EndsWith("区域特产"))
        {
            return true;
        }

        return TypeDescription switch
        {
            "角色经验素材" => true,
            "角色培养素材" => true,
            "天赋培养素材" => true,
            "武器强化素材" => true,
            "武器突破素材" => true,
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
            DayOfWeek.Monday or DayOfWeek.Thursday => MondayThursdayItems.Contains(Id),
            DayOfWeek.Tuesday or DayOfWeek.Friday => TuesdayFridayItems.Contains(Id),
            DayOfWeek.Wednesday or DayOfWeek.Saturday => WednesdaySaturdayItems.Contains(Id),
            _ => false,
        };
    }

    /// <summary>
    /// 获取物品对应的 DaysOfWeek
    /// </summary>
    /// <returns>DaysOfWeek</returns>
    public Binding.Cultivation.DaysOfWeek GetDaysOfWeek()
    {
        if (MondayThursdayItems.Contains(Id))
        {
            return Binding.Cultivation.DaysOfWeek.MondayAndThursday;
        }

        if (TuesdayFridayItems.Contains(Id))
        {
            return Binding.Cultivation.DaysOfWeek.TuesdayAndFriday;
        }

        if (WednesdaySaturdayItems.Contains(Id))
        {
            return Binding.Cultivation.DaysOfWeek.WednesdayAndSaturday;
        }

        return Binding.Cultivation.DaysOfWeek.Any;
    }
}