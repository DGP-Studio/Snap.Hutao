// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Binding.SpiralAbyss;

/// <summary>
/// 深渊视图
/// </summary>
public class SpiralAbyssView
{
    /// <summary>
    /// 构造一个新的深渊视图
    /// </summary>
    /// <param name="spiralAbyss">深渊信息</param>
    /// <param name="idAvatarMap">Id角色映射</param>
    public SpiralAbyssView(Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss spiralAbyss, Dictionary<AvatarId, Metadata.Avatar.Avatar> idAvatarMap)
    {
        Schedule = $"第 {spiralAbyss.ScheduleId} 期";
        TotalBattleTimes = spiralAbyss.TotalBattleTimes;
        TotalStar = spiralAbyss.TotalStar;
        MaxFloor = spiralAbyss.MaxFloor;
        Reveals = spiralAbyss.RevealRank.Select(r => new RankAvatar(r.Value, r.AvatarId, idAvatarMap)).ToList();
        Defeat = spiralAbyss.DefeatRank.Select(r => new RankAvatar(r.Value, r.AvatarId, idAvatarMap)).SingleOrDefault();
        Damage = spiralAbyss.DamageRank.Select(r => new RankAvatar(r.Value, r.AvatarId, idAvatarMap)).SingleOrDefault();
        TakeDamage = spiralAbyss.TakeDamageRank.Select(r => new RankAvatar(r.Value, r.AvatarId, idAvatarMap)).SingleOrDefault();
        NormalSkill = spiralAbyss.NormalSkillRank.Select(r => new RankAvatar(r.Value, r.AvatarId, idAvatarMap)).SingleOrDefault();
        EnergySkill = spiralAbyss.EnergySkillRank.Select(r => new RankAvatar(r.Value, r.AvatarId, idAvatarMap)).SingleOrDefault();
        Floors = spiralAbyss.Floors.Select(f => new FloorView(f, idAvatarMap)).ToList();
    }

    /// <summary>
    /// 期
    /// </summary>
    public string Schedule { get; set; }

    /// <summary>
    /// 战斗次数
    /// </summary>
    public int TotalBattleTimes { get; set; }

    /// <summary>
    /// 共获得渊星
    /// </summary>
    public int TotalStar { get; set; }

    /// <summary>
    /// 最深抵达
    /// </summary>
    public string MaxFloor { get; set; }

    /// <summary>
    /// 出战次数
    /// </summary>
    public List<RankAvatar> Reveals { get; set; }

    /// <summary>
    /// 击破次数
    /// </summary>
    public RankAvatar? Defeat { get; set; }

    /// <summary>
    /// 最强一击
    /// </summary>
    public RankAvatar? Damage { get; set; }

    /// <summary>
    /// 承受伤害
    /// </summary>
    public RankAvatar? TakeDamage { get; set; }

    /// <summary>
    /// 元素战技
    /// </summary>
    public RankAvatar? NormalSkill { get; set; }

    /// <summary>
    /// 元素爆发
    /// </summary>
    public RankAvatar? EnergySkill { get; set; }

    /// <summary>
    /// 层信息
    /// </summary>
    public List<FloorView> Floors { get; set; }
}