// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Tower;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.ViewModel.SpiralAbyss;

/// <summary>
/// 深渊视图
/// </summary>
[HighQuality]
internal sealed class SpiralAbyssView : IEntityOnly<SpiralAbyssEntry?>,
    IMappingFrom<SpiralAbyssView, SpiralAbyssEntry, SpiralAbyssMetadataContext>
{
    private readonly SpiralAbyssEntry? entity;

    /// <summary>
    /// 构造一个新的深渊视图
    /// </summary>
    /// <param name="entity">实体</param>
    /// <param name="idAvatarMap">Id角色映射</param>
    private SpiralAbyssView(SpiralAbyssEntry entity, SpiralAbyssMetadataContext context)
        : this(context.IdScheduleMap[(uint)entity.ScheduleId], context)
    {
        this.entity = entity;

        Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss? spiralAbyss = entity.SpiralAbyss;
        TotalBattleTimes = spiralAbyss.TotalBattleTimes;
        TotalStar = spiralAbyss.TotalStar;
        MaxFloor = spiralAbyss.MaxFloor;
        Reveals = spiralAbyss.RevealRank.SelectList(r => new RankAvatar(r.Value, context.IdAvatarMap[r.AvatarId]));
        Defeat = spiralAbyss.DefeatRank.Select(r => new RankAvatar(r.Value, context.IdAvatarMap[r.AvatarId])).SingleOrDefault();
        Damage = spiralAbyss.DamageRank.Select(r => new RankAvatar(r.Value, context.IdAvatarMap[r.AvatarId])).SingleOrDefault();
        TakeDamage = spiralAbyss.TakeDamageRank.Select(r => new RankAvatar(r.Value, context.IdAvatarMap[r.AvatarId])).SingleOrDefault();
        NormalSkill = spiralAbyss.NormalSkillRank.Select(r => new RankAvatar(r.Value, context.IdAvatarMap[r.AvatarId])).SingleOrDefault();
        EnergySkill = spiralAbyss.EnergySkillRank.Select(r => new RankAvatar(r.Value, context.IdAvatarMap[r.AvatarId])).SingleOrDefault();
        Floors = spiralAbyss.Floors.Select(f => new FloorView(f, context.IdAvatarMap)).Reverse().ToList();
    }

    private SpiralAbyssView(TowerSchedule towerSchedule, SpiralAbyssMetadataContext context)
    {
        TimeFormatted = $"{towerSchedule.Open:yyyy.MM.dd HH:mm} - {towerSchedule.Close:yyyy.MM.dd HH:mm}";

        BlessingName = towerSchedule.BuffName;
        Blessings = towerSchedule.Descriptions;
    }

    public SpiralAbyssEntry? Entity { get => entity; }

    public string TimeFormatted { get; }

    public string BlessingName { get; }

    public List<string> Blessings { get; }

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
}