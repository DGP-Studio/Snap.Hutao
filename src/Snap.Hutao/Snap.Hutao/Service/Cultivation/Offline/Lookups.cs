// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Item;
using System.Collections.Frozen;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Cultivation.Offline;

internal static class Lookups
{
    public const int PurpleExpBookPerItemExp = 20000;
    public const int PurpleWeaponOrePerItemExp = 10000;

    public const int PurpleExpBookPerItemMora = 4000;
    public const int PurpleWeaponOrePerItemMora = 1000;

    // 突破等级
    public static readonly ImmutableArray<int> AscensionLevels = [20, 40, 50, 60, 70, 80];
    public static readonly FrozenDictionary<int, int> AvatarAscensionIndices = WinRTAdaptive.ToFrozenDictionary(
    [
        KeyValuePair.Create(20, 0),
        KeyValuePair.Create(40, 1),
        KeyValuePair.Create(50, 2),
        KeyValuePair.Create(60, 3),
        KeyValuePair.Create(70, 4),
        KeyValuePair.Create(80, 5),
    ]);

    // 角色等级突破相关
    public static readonly ImmutableArray<int> AvatarAscensionMoraCosts = [20000, 40000, 60000, 80000, 100000, 120000];
    public static readonly ImmutableArray<uint> AvatarBossMaterialCounts = [0, 2, 4, 8, 12, 20];
    public static readonly ImmutableArray<uint> AvatarSpecialtyCounts = [3, 10, 20, 30, 45, 60];

    // 角色天赋升级相关
    public static readonly ImmutableArray<int> TalentMoraCosts = [0, 12500, 17500, 25000, 30000, 37500, 120000, 260000, 450000, 700000];
    public static readonly ImmutableArray<uint> WeeklyBossCounts = [0, 0, 0, 0, 0, 0, 1, 1, 2, 2];

    // 角色突破素材（元素晶石）基础 ID 映射
    public static readonly FrozenDictionary<ElementType, uint> ElementGemBaseIds = WinRTAdaptive.ToFrozenDictionary(
    [
        KeyValuePair.Create(ElementType.Fire, MaterialIds.AgnidusAgateSliver),
        KeyValuePair.Create(ElementType.Water, MaterialIds.VarunadaLazuriteSliver),
        KeyValuePair.Create(ElementType.Grass, MaterialIds.NagadusEmeraldSliver),
        KeyValuePair.Create(ElementType.Electric, MaterialIds.VajradaAmethystSliver),
        KeyValuePair.Create(ElementType.Wind, MaterialIds.VayudaTurquoiseSliver),
        KeyValuePair.Create(ElementType.Ice, MaterialIds.ShivadaJadeSliver),
        KeyValuePair.Create(ElementType.Rock, MaterialIds.PrithivaTopazSliver),
    ]);

    // 角色突破素材（元素石） 消耗查找表 [突破次数-1] -> (品质偏移, 数量)
    public static readonly ImmutableArray<QualityOffsetCount> AvatarGemConsumption =
    [
        (0U, 1U), // 突破1次
        (1U, 3U), // 突破2次
        (1U, 6U), // 突破3次
        (2U, 3U), // 突破4次
        (2U, 6U), // 突破5次
        (3U, 6U), // 突破6次
    ];

    // 角色突破怪物材料 消耗查找表 [突破次数-1] -> (品质偏移, 数量)
    public static readonly ImmutableArray<QualityOffsetCount> AvatarMonsterMaterialConsumption =
    [
        (2U, 03U), // 突破1次
        (2U, 15U), // 突破2次
        (1U, 12U), // 突破3次
        (1U, 18U), // 突破4次
        (0U, 12U), // 突破5次
        (0U, 24U), // 突破6次
    ];

    // 角色天赋素材 消耗查找表 [等级-1] -> (品质偏移, 数量)
    public static readonly ImmutableArray<QualityOffsetCount> TalentBookConsumption =
    [
        (0U, 00U), // 等级0 (padding)
        (2U, 03U), // 等级1
        (1U, 02U), // 等级2
        (1U, 04U), // 等级3
        (1U, 06U), // 等级4
        (1U, 09U), // 等级5
        (0U, 04U), // 等级6
        (0U, 06U), // 等级7
        (0U, 12U), // 等级8
        (0U, 16U), // 等级9
    ];

    // 角色天赋怪物材料 消耗查找表 [等级-1] -> (品质偏移, 数量)
    public static readonly ImmutableArray<QualityOffsetCount> TalentMonsterMaterialConsumption =
    [
        (0U, 00U), // 等级0 (padding)
        (2U, 06U), // 等级1
        (1U, 03U), // 等级2
        (1U, 04U), // 等级3
        (1U, 06U), // 等级4
        (1U, 09U), // 等级5
        (0U, 04U), // 等级6
        (0U, 06U), // 等级7
        (0U, 09U), // 等级8
        (0U, 12U), // 等级9
    ];

    // 武器突破摩拉消耗查找表
    public static readonly QualityIndexedImmutableArray<ImmutableArray<int>> WeaponAscensionMoraCosts =
    [
        [00000, 00000, 00000, 00000, 00000, 00000], // QUALITY_NONE (padding)
        [00000, 05000, 05000, 10000, 00000, 00000], // QUALITY_WHITE
        [05000, 05000, 10000, 15000, 00000, 00000], // QUALITY_GREEN
        [05000, 10000, 15000, 20000, 25000, 30000], // QUALITY_BLUE
        [05000, 15000, 20000, 30000, 35000, 45000], // QUALITY_PURPLE
        [10000, 20000, 30000, 45000, 55000, 65000], // QUALITY_ORANGE
        [10000, 20000, 30000, 45000, 55000, 65000], // QUALITY_ORANGE_SP
    ];

    // 武器突破素材消耗的查找表
    public static readonly QualityIndexedImmutableArray<ImmutableArray<QualityOffsetCount>> WeaponMaterialConsumption =
    [
        [(0U, 0U), (0U, 0U), (0U, 0U), (0U, 0U), (0U, 0U), (0U, 0U)], // QUALITY_NONE (padding)
        [(2U, 1U), (1U, 1U), (1U, 2U), (0U, 1U), (0U, 0U), (0U, 0U)], // QUALITY_WHITE
        [(2U, 1U), (1U, 1U), (1U, 3U), (0U, 1U), (0U, 0U), (0U, 0U)], // QUALITY_GREEN
        [(3U, 2U), (2U, 2U), (2U, 4U), (1U, 2U), (1U, 4U), (0U, 3U)], // QUALITY_BLUE
        [(3U, 3U), (2U, 3U), (2U, 6U), (1U, 3U), (1U, 6U), (0U, 4U)], // QUALITY_PURPLE
        [(3U, 5U), (2U, 5U), (2U, 9U), (1U, 5U), (1U, 9U), (0U, 6U)], // QUALITY_ORANGE
        [(3U, 5U), (2U, 5U), (2U, 9U), (1U, 5U), (1U, 9U), (0U, 6U)], // QUALITY_ORANGE_SP
    ];

    // 武器精英怪物材料消耗查找表
    public static readonly QualityIndexedImmutableArray<ImmutableArray<QualityOffsetCount>> WeaponMonsterMaterialAConsumption =
    [
        [(0U, 0U), (0U, 00U), (0U, 0U), (0U, 00U), (0U, 00U), (0U, 00U)], // QUALITY_NONE (padding)
        [(1U, 1U), (1U, 04U), (0U, 2U), (0U, 04U), (0U, 00U), (0U, 00U)], // QUALITY_WHITE
        [(1U, 1U), (1U, 05U), (0U, 3U), (0U, 05U), (0U, 00U), (0U, 00U)], // QUALITY_GREEN
        [(2U, 2U), (2U, 08U), (1U, 4U), (1U, 08U), (0U, 06U), (0U, 12U)], // QUALITY_BLUE
        [(2U, 3U), (2U, 12U), (1U, 6U), (1U, 12U), (0U, 09U), (0U, 18U)], // QUALITY_PURPLE
        [(2U, 5U), (2U, 18U), (1U, 9U), (1U, 18U), (0U, 14U), (0U, 27U)], // QUALITY_ORANGE
        [(2U, 5U), (2U, 18U), (1U, 9U), (1U, 18U), (0U, 14U), (0U, 27U)], // QUALITY_ORANGE_SP
    ];

    // 武器普通怪物材料消耗查找表
    public static readonly QualityIndexedImmutableArray<ImmutableArray<QualityOffsetCount>> WeaponMonsterMaterialBConsumption =
    [
        [(0U, 0U), (0U, 00U), (0U, 0U), (0U, 00U), (0U, 0U), (0U, 00U)], // QUALITY_NONE
        [(1U, 1U), (1U, 02U), (0U, 2U), (0U, 03U), (0U, 0U), (0U, 00U)], // QUALITY_WHITE
        [(1U, 1U), (1U, 04U), (0U, 3U), (0U, 04U), (0U, 0U), (0U, 00U)], // QUALITY_GREEN
        [(2U, 1U), (2U, 05U), (1U, 4U), (1U, 06U), (0U, 4U), (0U, 08U)], // QUALITY_BLUE
        [(2U, 2U), (2U, 08U), (1U, 6U), (1U, 09U), (0U, 6U), (0U, 12U)], // QUALITY_PURPLE
        [(2U, 3U), (2U, 12U), (1U, 9U), (1U, 14U), (0U, 9U), (0U, 18U)], // QUALITY_ORANGE
        [(2U, 3U), (2U, 12U), (1U, 9U), (1U, 14U), (0U, 9U), (0U, 18U)], // QUALITY_ORANGE_SP
    ];
}