// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata;

/// <summary>
/// 基础数值
/// </summary>
internal class BaseValue
{
    /// <summary>
    /// 基础生命值
    /// </summary>
    public float HpBase { get; set; }

    /// <summary>
    /// 基础攻击力
    /// </summary>
    public float AttackBase { get; set; }

    /// <summary>
    /// 基础防御力
    /// </summary>
    public float DefenseBase { get; set; }

    /// <summary>
    /// 获取值
    /// </summary>
    /// <param name="fightProperty">战斗属性</param>
    /// <returns>值</returns>
    public float GetValue(FightProperty fightProperty)
    {
        return fightProperty switch
        {
            FightProperty.FIGHT_PROP_BASE_HP => HpBase,
            FightProperty.FIGHT_PROP_BASE_ATTACK => AttackBase,
            FightProperty.FIGHT_PROP_BASE_DEFENSE => DefenseBase,
            _ => 0,
        };
    }
}