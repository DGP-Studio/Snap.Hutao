// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.Cultivation;
using System.Collections.Frozen;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata.Item;

internal static class MaterialIds
{
    private static readonly ImmutableArray<RotationalMaterialIdEntry> Entries =
    [
        new(DaysOfWeek.MondayAndThursday, 104301U, 104302U, 104303U), // 「自由」
        new(DaysOfWeek.MondayAndThursday, 104310U, 104311U, 104312U), // 「繁荣」
        new(DaysOfWeek.MondayAndThursday, 104320U, 104321U, 104322U), // 「浮世」
        new(DaysOfWeek.MondayAndThursday, 104329U, 104330U, 104331U), // 「诤言」
        new(DaysOfWeek.MondayAndThursday, 104338U, 104339U, 104340U), // 「公平」
        new(DaysOfWeek.MondayAndThursday, 104347U, 104348U, 104349U), // 「角逐」
        new(DaysOfWeek.TuesdayAndFriday, 104304U, 104305U, 104306U), // 「抗争」
        new(DaysOfWeek.TuesdayAndFriday, 104313U, 104314U, 104315U), // 「勤劳」
        new(DaysOfWeek.TuesdayAndFriday, 104323U, 104324U, 104325U), // 「风雅」
        new(DaysOfWeek.TuesdayAndFriday, 104332U, 104333U, 104334U), // 「巧思」
        new(DaysOfWeek.TuesdayAndFriday, 104341U, 104342U, 104343U), // 「正义」
        new(DaysOfWeek.TuesdayAndFriday, 104350U, 104351U, 104352U), // 「焚燔」
        new(DaysOfWeek.WednesdayAndSaturday, 104307U, 104308U, 104309U), // 「诗文」
        new(DaysOfWeek.WednesdayAndSaturday, 104316U, 104317U, 104318U), // 「黄金」
        new(DaysOfWeek.WednesdayAndSaturday, 104326U, 104327U, 104328U), // 「天光」
        new(DaysOfWeek.WednesdayAndSaturday, 104335U, 104336U, 104337U), // 「笃行」
        new(DaysOfWeek.WednesdayAndSaturday, 104344U, 104345U, 104346U), // 「秩序」
        new(DaysOfWeek.WednesdayAndSaturday, 104353U, 104354U, 104355U), // 「纷争」
        new(DaysOfWeek.MondayAndThursday, 114001U, 114002U, 114003U, 114004U), // 高塔孤王
        new(DaysOfWeek.MondayAndThursday, 114013U, 114014U, 114015U, 114016U), // 孤云寒林
        new(DaysOfWeek.MondayAndThursday, 114025U, 114026U, 114027U, 114028U), // 远海夷地
        new(DaysOfWeek.MondayAndThursday, 114037U, 114038U, 114039U, 114040U), // 谧林涓露
        new(DaysOfWeek.MondayAndThursday, 114049U, 114050U, 114051U, 114052U), // 悠古弦音
        new(DaysOfWeek.MondayAndThursday, 114061U, 114062U, 114063U, 114064U), // 贡祭炽心
        new(DaysOfWeek.TuesdayAndFriday, 114005U, 114006U, 114007U, 114008U), // 凛风奔狼
        new(DaysOfWeek.TuesdayAndFriday, 114017U, 114018U, 114019U, 114020U), // 雾海云间
        new(DaysOfWeek.TuesdayAndFriday, 114029U, 114030U, 114031U, 114032U), // 鸣神御灵
        new(DaysOfWeek.TuesdayAndFriday, 114041U, 114042U, 114043U, 114044U), // 绿洲花园
        new(DaysOfWeek.TuesdayAndFriday, 114053U, 114054U, 114055U, 114056U), // 纯圣露滴
        new(DaysOfWeek.TuesdayAndFriday, 114065U, 114066U, 114067U, 114068U), // 谵妄圣主
        new(DaysOfWeek.WednesdayAndSaturday, 114009U, 114010U, 114011U, 114012U), // 狮牙斗士
        new(DaysOfWeek.WednesdayAndSaturday, 114021U, 114022U, 114023U, 114024U), // 漆黑陨铁
        new(DaysOfWeek.WednesdayAndSaturday, 114033U, 114034U, 114035U, 114036U), // 今昔剧画
        new(DaysOfWeek.WednesdayAndSaturday, 114045U, 114046U, 114047U, 114048U), // 谧林涓露
        new(DaysOfWeek.WednesdayAndSaturday, 114057U, 114058U, 114059U, 114060U), // 无垢之海
        new(DaysOfWeek.WednesdayAndSaturday, 114069U, 114070U, 114071U, 114072U), // 神合秘烟
    ];

    public static FrozenSet<MaterialId> MondayThursdayItems { get; } = [.. Entries.Where(entry => entry.DaysOfWeek is DaysOfWeek.MondayAndThursday).SelectMany(entry => entry.Enumerate())];

    public static FrozenSet<RotationalMaterialIdEntry> MondayThursdayEntries { get; } = [.. Entries.Where(entry => entry.DaysOfWeek is DaysOfWeek.MondayAndThursday)];

    public static FrozenSet<MaterialId> TuesdayFridayItems { get; } = [.. Entries.Where(entry => entry.DaysOfWeek is DaysOfWeek.TuesdayAndFriday).SelectMany(entry => entry.Enumerate())];

    public static FrozenSet<RotationalMaterialIdEntry> TuesdayFridayEntries { get; } = [.. Entries.Where(entry => entry.DaysOfWeek is DaysOfWeek.TuesdayAndFriday)];

    public static FrozenSet<MaterialId> WednesdaySaturdayItems { get; } = [.. Entries.Where(entry => entry.DaysOfWeek is DaysOfWeek.WednesdayAndSaturday).SelectMany(entry => entry.Enumerate())];

    public static FrozenSet<RotationalMaterialIdEntry> WednesdaySaturdayEntries { get; } = [.. Entries.Where(entry => entry.DaysOfWeek is DaysOfWeek.WednesdayAndSaturday)];

    public static DaysOfWeek GetDaysOfWeek(MaterialId id)
    {
        if (MondayThursdayItems.Contains(id))
        {
            return DaysOfWeek.MondayAndThursday;
        }

        if (TuesdayFridayItems.Contains(id))
        {
            return DaysOfWeek.TuesdayAndFriday;
        }

        if (WednesdaySaturdayItems.Contains(id))
        {
            return DaysOfWeek.WednesdayAndSaturday;
        }

        return DaysOfWeek.Any;
    }
}