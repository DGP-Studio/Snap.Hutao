// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Metadata.Tower;

namespace Snap.Hutao.ViewModel.SpiralAbyss;

/// <summary>
/// 深渊视图
/// </summary>
[HighQuality]
internal sealed class SpiralAbyssView : IEntityOnly<SpiralAbyssEntry?>,
    IMappingFrom<SpiralAbyssView, SpiralAbyssEntry, SpiralAbyssMetadataContext>,
    IMappingFrom<SpiralAbyssView, SpiralAbyssEntry?, TowerSchedule, SpiralAbyssMetadataContext>
{
    private readonly SpiralAbyssEntry? entity;

    private SpiralAbyssView(SpiralAbyssEntry entity, SpiralAbyssMetadataContext context)
        : this(context.IdScheduleMap[entity.ScheduleId], context)
    {
        this.entity = entity;

        Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss? spiralAbyss = entity.SpiralAbyss;
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

        foreach (Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.Floor webFloor in spiralAbyss.Floors)
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
        Floors = towerSchedule.FloorIds.Select(id => FloorView.From(context.IdFloorMap[id], context)).Reverse().ToList();
    }

    public uint ScheduleId { get; }

    /// <summary>
    /// 视图 中使用的计划 Id 字符串
    /// </summary>
    public string Schedule { get => SH.ModelEntitySpiralAbyssScheduleFormat.Format(ScheduleId); }

    public SpiralAbyssEntry? Entity { get => entity; }

    public string TimeFormatted { get; }

    public string BlessingName { get; }

    public List<string> Blessings { get; }

    public bool Engaged { get; }

    /// <summary>
    /// 战斗次数
    /// </summary>
    public int TotalBattleTimes { get; }

    /// <summary>
    /// 共获得渊星
    /// </summary>
    public int TotalStar { get; }

    /// <summary>
    /// 最深抵达
    /// </summary>
    public string MaxFloor { get; } = default!;

    /// <summary>
    /// 出战次数
    /// </summary>
    public List<RankAvatar> Reveals { get; } = default!;

    /// <summary>
    /// 击破次数
    /// </summary>
    public RankAvatar? Defeat { get; }

    /// <summary>
    /// 最强一击
    /// </summary>
    public RankAvatar? Damage { get; }

    /// <summary>
    /// 承受伤害
    /// </summary>
    public RankAvatar? TakeDamage { get; }

    /// <summary>
    /// 元素战技
    /// </summary>
    public RankAvatar? NormalSkill { get; }

    /// <summary>
    /// 元素爆发
    /// </summary>
    public RankAvatar? EnergySkill { get; }

    /// <summary>
    /// 层信息
    /// </summary>
    public List<FloorView> Floors { get; }

    public static SpiralAbyssView From(SpiralAbyssEntry entity, SpiralAbyssMetadataContext context)
    {
        return new(entity, context);
    }

    public static SpiralAbyssView From(SpiralAbyssEntry? entity, TowerSchedule meta, SpiralAbyssMetadataContext context)
    {
        if (entity is not null)
        {
            return new(entity, context);
        }
        else
        {
            return new(meta, context);
        }
    }

    private static List<RankAvatar> ToRankAvatars(List<Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.Rank> ranks, SpiralAbyssMetadataContext context)
    {
        return ranks.Where(r => r.AvatarId != 0U).Select(r => new RankAvatar(r.Value, context.IdAvatarMap[r.AvatarId])).ToList();
    }

    private static RankAvatar? ToRankAvatar(List<Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.Rank> ranks, SpiralAbyssMetadataContext context)
    {
        return ranks.Where(r => r.AvatarId != 0U).Select(r => new RankAvatar(r.Value, context.IdAvatarMap[r.AvatarId])).SingleOrDefault();
    }
}