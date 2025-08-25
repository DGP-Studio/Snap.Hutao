// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Metadata.Tower;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.Web.Hoyolab;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.SpiralAbyss;

internal sealed partial class SpiralAbyssView : IEntityAccess<SpiralAbyssEntry?>, IPropertyValuesProvider
{
    private SpiralAbyssView(SpiralAbyssEntry entity, SpiralAbyssMetadataContext context)
        : this(entity, context.IdTowerScheduleMap[entity.ScheduleId], context)
    {
    }

    private SpiralAbyssView(SpiralAbyssEntry entity, TowerSchedule towerSchedule, SpiralAbyssMetadataContext context)
        : this(towerSchedule, context)
    {
        Entity = entity;

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

        TimeSpan offset = PlayerUid.GetRegionTimeZoneUtcOffsetForUid(entity.Uid);
        foreach (Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyssFloor webFloor in spiralAbyss.Floors)
        {
            // Ignoring floor 1 - 8 here
            if (Floors.Source.SingleOrDefault(f => f.IndexValue == webFloor.Index) is { } floor)
            {
                floor.Attach(webFloor, offset, context);
            }
        }
    }

    private SpiralAbyssView(TowerSchedule towerSchedule, SpiralAbyssMetadataContext context)
    {
        ScheduleId = towerSchedule.Id;
        FormattedTime = $"{towerSchedule.Open:yyyy.MM.dd HH:mm} - {towerSchedule.Close:yyyy.MM.dd HH:mm}";

        BlessingName = towerSchedule.BuffName;
        Blessings = towerSchedule.Descriptions;
        Floors = towerSchedule.FloorIds.Select(id => FloorView.Create(context.IdTowerFloorMap[id], context)).Reverse().AsAdvancedCollectionView();
    }

    public uint ScheduleId { get; }

    public string Schedule { get => SH.FormatModelEntitySpiralAbyssSchedule(ScheduleId); }

    public string FormattedTime { get; }

    public bool Engaged { get; }

    public SpiralAbyssEntry? Entity { get; }

    public string BlessingName { get; }

    public ImmutableArray<string> Blessings { get; }

    public int TotalBattleTimes { get; }

    public int TotalStar { get; }

    public string MaxFloor { get; } = default!;

    public ImmutableArray<RankAvatar> Reveals { get; } = [];

    public IAdvancedCollectionView<FloorView> Floors { get; }

    public RankAvatar? Defeat { get; }

    public RankAvatar? Damage { get; }

    public RankAvatar? TakeDamage { get; }

    public RankAvatar? NormalSkill { get; }

    public RankAvatar? EnergySkill { get; }

    public static SpiralAbyssView From(SpiralAbyssEntry entity, SpiralAbyssMetadataContext context)
    {
        return new(entity, context);
    }

    public static SpiralAbyssView From(SpiralAbyssEntry? entity, TowerSchedule meta, SpiralAbyssMetadataContext context)
    {
        return entity is not null ? new(entity, meta, context) : new(meta, context);
    }

    private static ImmutableArray<RankAvatar> ToRankAvatars(ImmutableArray<Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyssRank> ranks, SpiralAbyssMetadataContext context)
    {
        return [.. ranks.Where(r => r.AvatarId != 0U).Select(r => new RankAvatar(r.Value, context.IdAvatarMap[r.AvatarId]))];
    }

    private static RankAvatar? ToRankAvatar(ImmutableArray<Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyssRank> ranks, SpiralAbyssMetadataContext context)
    {
        return ranks.Where(r => r.AvatarId != 0U).Select(r => new RankAvatar(r.Value, context.IdAvatarMap[r.AvatarId])).SingleOrDefault();
    }
}