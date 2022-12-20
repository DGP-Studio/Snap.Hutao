// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Service.AvatarInfo.Factory;

/// <summary>
/// 词条权重
/// </summary>
internal class AffixWeight : Dictionary<FightProperty, double>
{
    /// <summary>
    /// 构造一个新的词条权重
    /// </summary>
    /// <param name="avatarId">角色Id</param>
    /// <param name="hpPercent">大生命</param>
    /// <param name="attackPercenr">大攻击</param>
    /// <param name="defensePercent">大防御</param>
    /// <param name="critical">暴击率</param>
    /// <param name="criticalHurt">暴击伤害</param>
    /// <param name="elementMastery">元素精通</param>
    /// <param name="chargeEfficiency">充能效率</param>
    /// <param name="healAdd">治疗加成</param>
    /// <param name="name">名称</param>
    public AffixWeight(
        int avatarId,
        double hpPercent,
        double attackPercenr,
        double defensePercent,
        double critical,
        double criticalHurt,
        double elementMastery,
        double chargeEfficiency,
        double healAdd,
        string name = "通用")
    {
        AvatarId = avatarId;
        Name = name;

        this[FightProperty.FIGHT_PROP_HP_PERCENT] = hpPercent;
        this[FightProperty.FIGHT_PROP_ATTACK_PERCENT] = attackPercenr;
        this[FightProperty.FIGHT_PROP_DEFENSE_PERCENT] = defensePercent;
        this[FightProperty.FIGHT_PROP_CRITICAL] = critical;
        this[FightProperty.FIGHT_PROP_CRITICAL_HURT] = criticalHurt;
        this[FightProperty.FIGHT_PROP_ELEMENT_MASTERY] = elementMastery;
        this[FightProperty.FIGHT_PROP_CHARGE_EFFICIENCY] = chargeEfficiency;
        this[FightProperty.FIGHT_PROP_HEAL_ADD] = healAdd;
    }

    /// <summary>
    /// 角色Id
    /// </summary>
    public AvatarId AvatarId { get; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 风元素伤害加成
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>链式调用对象</returns>
    public AffixWeight Anemo(double value = 100)
    {
        this[FightProperty.FIGHT_PROP_WIND_ADD_HURT] = value;
        return this;
    }

    /// <summary>
    /// 冰元素伤害加成
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>链式调用对象</returns>
    public AffixWeight Cryo(double value = 100)
    {
        this[FightProperty.FIGHT_PROP_ICE_ADD_HURT] = value;
        return this;
    }

    /// <summary>
    /// 草元素伤害加成
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>链式调用对象</returns>
    public AffixWeight Dendro(double value = 100)
    {
        this[FightProperty.FIGHT_PROP_GRASS_ADD_HURT] = value;
        return this;
    }

    /// <summary>
    /// 雷元素伤害加成
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>链式调用对象</returns>
    public AffixWeight Electro(double value = 100)
    {
        this[FightProperty.FIGHT_PROP_ELEC_ADD_HURT] = value;
        return this;
    }

    /// <summary>
    /// 岩元素伤害加成
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>链式调用对象</returns>
    public AffixWeight Geo(double value = 100)
    {
        this[FightProperty.FIGHT_PROP_ROCK_ADD_HURT] = value;
        return this;
    }

    /// <summary>
    /// 水元素伤害加成
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>链式调用对象</returns>
    public AffixWeight Hydro(double value = 100)
    {
        this[FightProperty.FIGHT_PROP_WATER_ADD_HURT] = value;
        return this;
    }

    /// <summary>
    /// 火元素伤害加成
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>链式调用对象</returns>
    public AffixWeight Pyro(double value = 100)
    {
        this[FightProperty.FIGHT_PROP_FIRE_ADD_HURT] = value;
        return this;
    }

    /// <summary>
    /// 物理伤害伤害加成
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>链式调用对象</returns>
    public AffixWeight Phyiscal(double value = 100)
    {
        this[FightProperty.FIGHT_PROP_PHYSICAL_ADD_HURT] = value;
        return this;
    }
}