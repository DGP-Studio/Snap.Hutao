// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Reliquary;

/// <summary>
/// 圣遗物词条权重
/// </summary>
internal sealed class ReliquaryAffixWeight
{
    /// <summary>
    /// 角色 Id
    /// </summary>
    public AvatarId AvatarId { get; set; }

    /// <summary>
    /// 生命值
    /// </summary>
    public float HpPercent { get; set; }

    /// <summary>
    /// 攻击力
    /// </summary>
    public float AttackPercent { get; set; }

    /// <summary>
    /// 防御力
    /// </summary>
    public float DefensePercent { get; set; }

    /// <summary>
    /// 暴击率
    /// </summary>
    public float Critical { get; set; }

    /// <summary>
    /// 暴击伤害
    /// </summary>
    public float CriticalHurt { get; set; }

    /// <summary>
    /// 元素精通
    /// </summary>
    public float ElementMastery { get; set; }

    /// <summary>
    /// 元素充能效率
    /// </summary>
    public float ChargeEfficiency { get; set; }

    /// <summary>
    /// 治疗加成
    /// </summary>
    public float HealAdd { get; set; }

    /// <summary>
    /// 物理伤害加成
    /// </summary>
    public float PhysicalAddHurt { get; set; }

    /// <summary>
    /// 伤害加成
    /// </summary>
    public float AddHurt { get; set; }

    /// <summary>
    /// 元素类型
    /// </summary>
    public ElementType ElementType { get; set; }

    /// <summary>
    /// 获取权重
    /// </summary>
    /// <param name="fightProperty">属性</param>
    /// <returns>权重</returns>
    public float this[FightProperty fightProperty]
    {
        get
        {
            if ((ElementType == ElementType.Fire && fightProperty == FightProperty.FIGHT_PROP_FIRE_ADD_HURT) ||
                (ElementType == ElementType.Water && fightProperty == FightProperty.FIGHT_PROP_WATER_ADD_HURT) ||
                (ElementType == ElementType.Grass && fightProperty == FightProperty.FIGHT_PROP_GRASS_ADD_HURT) ||
                (ElementType == ElementType.Electric && fightProperty == FightProperty.FIGHT_PROP_ELEC_ADD_HURT) ||
                (ElementType == ElementType.Ice && fightProperty == FightProperty.FIGHT_PROP_ICE_ADD_HURT) ||
                (ElementType == ElementType.Wind && fightProperty == FightProperty.FIGHT_PROP_WIND_ADD_HURT) ||
                (ElementType == ElementType.Rock && fightProperty == FightProperty.FIGHT_PROP_ROCK_ADD_HURT))
            {
                return AddHurt;
            }

            return fightProperty switch
            {
                FightProperty.FIGHT_PROP_HP_PERCENT => HpPercent,
                FightProperty.FIGHT_PROP_ATTACK_PERCENT => AttackPercent,
                FightProperty.FIGHT_PROP_DEFENSE_PERCENT => DefensePercent,
                FightProperty.FIGHT_PROP_CRITICAL => Critical,
                FightProperty.FIGHT_PROP_CRITICAL_HURT => CriticalHurt,
                FightProperty.FIGHT_PROP_ELEMENT_MASTERY => ElementMastery,
                FightProperty.FIGHT_PROP_CHARGE_EFFICIENCY => ChargeEfficiency,
                FightProperty.FIGHT_PROP_HEALED_ADD => HealAdd,
                FightProperty.FIGHT_PROP_PHYSICAL_ADD_HURT => PhysicalAddHurt,
                _ => 0,
            };
        }
    }
}