// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Service.AvatarInfo.Factory;

/// <summary>
/// 词条权重
/// </summary>
[HighQuality]
internal sealed class AffixWeight : Dictionary<FightProperty, float>
{
    /// <summary>
    /// 构造一个新的词条权重
    /// </summary>
    /// <param name="avatarId">角色Id</param>
    /// <param name="hp">大生命</param>
    /// <param name="atk">大攻击</param>
    /// <param name="def">大防御</param>
    /// <param name="crit">暴击率</param>
    /// <param name="critHurt">暴击伤害</param>
    /// <param name="mastery">元素精通</param>
    /// <param name="charge">充能效率</param>
    /// <param name="heal">治疗加成</param>
    /// <param name="name">名称</param>
    public AffixWeight(
        int avatarId,
        float hp,
        float atk,
        float def,
        float crit,
        float critHurt,
        float mastery,
        float charge,
        float heal,
        string name = "通用")
    {
        AvatarId = avatarId;
        Name = name;

        this[FightProperty.FIGHT_PROP_HP_PERCENT] = hp;
        this[FightProperty.FIGHT_PROP_ATTACK_PERCENT] = atk;
        this[FightProperty.FIGHT_PROP_DEFENSE_PERCENT] = def;
        this[FightProperty.FIGHT_PROP_CRITICAL] = crit;
        this[FightProperty.FIGHT_PROP_CRITICAL_HURT] = critHurt;
        this[FightProperty.FIGHT_PROP_ELEMENT_MASTERY] = mastery;
        this[FightProperty.FIGHT_PROP_CHARGE_EFFICIENCY] = charge;
        this[FightProperty.FIGHT_PROP_HEAL_ADD] = heal;
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
    public AffixWeight Anemo(float value = 100)
    {
        this[FightProperty.FIGHT_PROP_WIND_ADD_HURT] = value;
        return this;
    }

    /// <summary>
    /// 冰元素伤害加成
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>链式调用对象</returns>
    public AffixWeight Cryo(float value = 100)
    {
        this[FightProperty.FIGHT_PROP_ICE_ADD_HURT] = value;
        return this;
    }

    /// <summary>
    /// 草元素伤害加成
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>链式调用对象</returns>
    public AffixWeight Dendro(float value = 100)
    {
        this[FightProperty.FIGHT_PROP_GRASS_ADD_HURT] = value;
        return this;
    }

    /// <summary>
    /// 雷元素伤害加成
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>链式调用对象</returns>
    public AffixWeight Electro(float value = 100)
    {
        this[FightProperty.FIGHT_PROP_ELEC_ADD_HURT] = value;
        return this;
    }

    /// <summary>
    /// 岩元素伤害加成
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>链式调用对象</returns>
    public AffixWeight Geo(float value = 100)
    {
        this[FightProperty.FIGHT_PROP_ROCK_ADD_HURT] = value;
        return this;
    }

    /// <summary>
    /// 水元素伤害加成
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>链式调用对象</returns>
    public AffixWeight Hydro(float value = 100)
    {
        this[FightProperty.FIGHT_PROP_WATER_ADD_HURT] = value;
        return this;
    }

    /// <summary>
    /// 火元素伤害加成
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>链式调用对象</returns>
    public AffixWeight Pyro(float value = 100)
    {
        this[FightProperty.FIGHT_PROP_FIRE_ADD_HURT] = value;
        return this;
    }

    /// <summary>
    /// 物理伤害伤害加成
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>链式调用对象</returns>
    public AffixWeight Physical(float value = 100)
    {
        this[FightProperty.FIGHT_PROP_PHYSICAL_ADD_HURT] = value;
        return this;
    }
}