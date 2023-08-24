// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata.Item;

/// <summary>
/// 材料表
/// </summary>
internal static class Materials
{
    private static readonly ImmutableHashSet<MaterialId> MondayThursdayItemsInner = new HashSet<MaterialId>
    {
        104301U, 104302U, 104303U, // 「自由」
        104310U, 104311U, 104312U, // 「繁荣」
        104320U, 104321U, 104322U, // 「浮世」
        104329U, 104330U, 104331U, // 「诤言」
        104338U, 104339U, 104340U, // 「公平」
        114001U, 114002U, 114003U, 114004U, // 高塔孤王
        114013U, 114014U, 114015U, 114016U, // 孤云寒林
        114025U, 114026U, 114027U, 114028U, // 远海夷地
        114037U, 114038U, 114039U, 114040U, // 谧林涓露
        114049U, 114050U, 114051U, 114052U, // 悠古弦音
    }.ToImmutableHashSet();

    private static readonly ImmutableHashSet<MaterialId> TuesdayFridayItemsInner = new HashSet<MaterialId>
    {
        104304U, 104305U, 104306U, // 「抗争」
        104313U, 104314U, 104315U, // 「勤劳」
        104323U, 104324U, 104325U, // 「风雅」
        104332U, 104333U, 104334U, // 「巧思」
        104341U, 104342U, 104343U, // 「正义」
        114005U, 114006U, 114007U, 114008U, // 凛风奔狼
        114017U, 114018U, 114019U, 114020U, // 雾海云间
        114029U, 114030U, 114031U, 114032U, // 鸣神御灵
        114041U, 114042U, 114043U, 114044U, // 绿洲花园
        114053U, 114054U, 114055U, 114056U, // 纯圣露滴
    }.ToImmutableHashSet();

    private static readonly ImmutableHashSet<MaterialId> WednesdaySaturdayItemsInner = new HashSet<MaterialId>
    {
        104307U, 104308U, 104309U, // 「诗文」
        104316U, 104317U, 104318U, // 「黄金」
        104326U, 104327U, 104328U, // 「天光」
        104335U, 104336U, 104337U, // 「笃行」
        104344U, 104345U, 104346U, // 「秩序」
        114009U, 114010U, 114011U, 114012U, // 狮牙斗士
        114021U, 114022U, 114023U, 114024U, // 漆黑陨铁
        114033U, 114034U, 114035U, 114036U, // 今昔剧画
        114045U, 114046U, 114047U, 114048U, // 谧林涓露
        114057U, 114058U, 114059U, 114060U, // 无垢之海
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