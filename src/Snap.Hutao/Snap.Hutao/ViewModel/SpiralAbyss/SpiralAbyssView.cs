// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Metadata.Tower;
using Snap.Hutao.UI.Xaml.Data;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.SpiralAbyss;

internal sealed partial class SpiralAbyssView : IEntityAccess<SpiralAbyssEntry?>, IAdvancedCollectionViewItem
{
    private readonly SpiralAbyssEntry? entity;

    private SpiralAbyssView(SpiralAbyssEntry entity, SpiralAbyssMetadataContext context)
        : this(context.IdTowerScheduleMap[entity.ScheduleId], context)
    {
        this.entity = entity;

        Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss spiralAbyss = entity.SpiralAbyss;
        TotalBattleTimes = spiralAbyss.TotalBattleTimes;
        TotalStar = spiralAbyss.TotalStar;
        MaxFloor = spiralAbyss.MaxFloor;
        Reveals = ToRankAvatars(spiralAbyss.RevealRank, context);
        Defeat = ToRankAvatar(spiralAbyss.DefeatRank, context);
        Damage = ToRankAvatar(spiralAbyss.DamageRank, context);
        TakeDamage = ToRankAvatar(spiralAbyss.TakeDamageRank, context);
        NormalSkill = ToRankAvatar(spiralAbyss.NormalSkillRank, context);
        EnergySkill = ToRankAvatar(spiralAbyss.EnergySkillRank, context);
        Engaged = true;

        foreach (Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyssFloor webFloor in spiralAbyss.Floors)
        {
            // Ignoring floor 1 - 8 here
            if (Floors.SingleOrDefault(f => f.IndexValue == webFloor.Index) is { } floor)
            {
                floor.WithSpiralAbyssFloor(webFloor, context);
            }
        }
    }

    private SpiralAbyssView(TowerSchedule towerSchedule, SpiralAbyssMetadataContext context)
    {
        ScheduleId = towerSchedule.Id;
        TimeFormatted = $"{towerSchedule.Open:yyyy.MM.dd HH:mm} - {towerSchedule.Close:yyyy.MM.dd HH:mm}";

        BlessingName = towerSchedule.BuffName;
        Blessings = towerSchedule.Descriptions;
        Floors = towerSchedule.FloorIds.Select(id => FloorView.From(context.IdTowerFloorMap[id], context)).Reverse().ToList();
    }

    public uint ScheduleId { get; }

    public string Schedule { get => SH.FormatModelEntitySpiralAbyssScheduleFormat(ScheduleId); }

    public SpiralAbyssEntry? Entity { get => entity; }

    public string TimeFormatted { get; }

    public string BlessingName { get; }

    public ImmutableArray<string> Blessings { get; }

    public bool Engaged { get; }

    public int TotalBattleTimes { get; }

    public int TotalStar { get; }

    public string MaxFloor { get; } = default!;

    public List<RankAvatar> Reveals { get; } = default!;

    public RankAvatar? Defeat { get; }

    public RankAvatar? Damage { get; }

    public RankAvatar? TakeDamage { get; }

    public RankAvatar? NormalSkill { get; }

    public RankAvatar? EnergySkill { get; }

    public List<FloorView> Floors { get; }

    public static SpiralAbyssView From(SpiralAbyssEntry entity, SpiralAbyssMetadataContext context)
    {
        return new(entity, context);
    }

    public static SpiralAbyssView From(SpiralAbyssEntry? entity, TowerSchedule meta, SpiralAbyssMetadataContext context)
    {
        return entity is not null ? new(entity, context) : new(meta, context);
    }

    private static List<RankAvatar> ToRankAvatars(List<Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyssRank> ranks, SpiralAbyssMetadataContext context)
    {
        return ranks.Where(r => r.AvatarId != 0U).Select(r => new RankAvatar(r.Value, context.IdAvatarMap[r.AvatarId])).ToList();
    }

    private static RankAvatar? ToRankAvatar(List<Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyssRank> ranks, SpiralAbyssMetadataContext context)
    {
        return ranks.Where(r => r.AvatarId != 0U).Select(r => new RankAvatar(r.Value, context.IdAvatarMap[r.AvatarId])).SingleOrDefault();
    }
}