// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.Cultivation;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata.Item;

/// <summary>
/// 材料表
/// </summary>
internal static class Materials
{
    private static readonly ImmutableHashSet<MaterialId> MondayThursdayItemsInner = new HashSet<MaterialId>
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

    private static readonly ImmutableHashSet<MaterialId> TuesdayFridayItemsInner = new HashSet<MaterialId>
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

    private static readonly ImmutableHashSet<MaterialId> WednesdaySaturdayItemsInner = new HashSet<MaterialId>
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
    /// 周一/周四
    /// </summary>
    public static ImmutableHashSet<MaterialId> MondayThursdayItems { get => MondayThursdayItemsInner; }

    /// <summary>
    /// 周二/周五
    /// </summary>
    public static ImmutableHashSet<MaterialId> TuesdayFridayItems { get => TuesdayFridayItemsInner; }

    /// <summary>
    /// 周三/周六
    /// </summary>
    public static ImmutableHashSet<MaterialId> WednesdaySaturdayItems { get => WednesdaySaturdayItemsInner; }
}