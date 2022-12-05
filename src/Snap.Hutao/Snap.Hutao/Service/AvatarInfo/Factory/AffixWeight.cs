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
    /// <param name="hp">大生命</param>
    /// <param name="atk">大攻击</param>
    /// <param name="def">大防御</param>
    /// <param name="cr">暴击率</param>
    /// <param name="ch">暴击伤害</param>
    /// <param name="em">元素精通</param>
    /// <param name="ce">充能效率</param>
    /// <param name="ha">治疗加成</param>
    /// <param name="name">名称</param>
    public AffixWeight(int avatarId, double hp, double atk, double def, double cr, double ch, double em, double ce, double ha, string name = "通用")
    {
        AvatarId = avatarId;
        Name = name;

        this[FightProperty.FIGHT_PROP_HP_PERCENT] = hp;
        this[FightProperty.FIGHT_PROP_ATTACK_PERCENT] = atk;
        this[FightProperty.FIGHT_PROP_DEFENSE_PERCENT] = def;
        this[FightProperty.FIGHT_PROP_CRITICAL] = cr;
        this[FightProperty.FIGHT_PROP_CRITICAL_HURT] = ch;
        this[FightProperty.FIGHT_PROP_ELEMENT_MASTERY] = em;
        this[FightProperty.FIGHT_PROP_CHARGE_EFFICIENCY] = ce;
        this[FightProperty.FIGHT_PROP_HEAL_ADD] = ha;
    }

    /// <summary>
    /// 角色Id
    /// </summary>
    public AvatarId AvatarId { get; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; }
}