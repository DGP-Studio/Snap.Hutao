// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.SpiralAbyss.Post;

internal sealed class SimpleSpiralAbyss
{
    public SimpleSpiralAbyss(Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss spiralAbyss)
    {
        ScheduleId = spiralAbyss.ScheduleId;
        TotalBattleTimes = spiralAbyss.TotalBattleTimes;
        TotalWinTimes = spiralAbyss.TotalWinTimes;
        Damage = SimpleRank.FromRank(spiralAbyss.DamageRank.SingleOrDefault());
        TakeDamage = SimpleRank.FromRank(spiralAbyss.TakeDamageRank.SingleOrDefault());
        Floors = spiralAbyss.Floors.Select(f => new SimpleFloor(f));
    }

    public uint ScheduleId { get; set; }

    public int TotalBattleTimes { get; set; }

    public int TotalWinTimes { get; set; }

    public SimpleRank? Damage { get; set; }

    public SimpleRank? TakeDamage { get; set; }

    public IEnumerable<SimpleFloor> Floors { get; set; }
}