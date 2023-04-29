// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Converter;

namespace Snap.Hutao.Model.Metadata.Monster;

/// <summary>
/// 怪物基本属性
/// </summary>
internal sealed class MonsterBaseValue : BaseValue
{
    /// <summary>
    /// 火抗
    /// </summary>
    public float FireSubHurt { get; set; }

    /// <summary>
    /// 草抗
    /// </summary>
    public float GrassSubHurt { get; set; }

    /// <summary>
    /// 水抗
    /// </summary>
    public float WaterSubHurt { get; set; }

    /// <summary>
    /// 雷抗
    /// </summary>
    public float ElecSubHurt { get; set; }

    /// <summary>
    /// 风抗
    /// </summary>
    public float WindSubHurt { get; set; }

    /// <summary>
    /// 冰抗
    /// </summary>
    public float IceSubHurt { get; set; }

    /// <summary>
    /// 岩抗
    /// </summary>
    public float RockSubHurt { get; set; }

    /// <summary>
    /// 物抗
    /// </summary>
    public float PhysicalSubHurt { get; set; }

    /// <summary>
    /// 抗性
    /// </summary>
    public List<NameValue<string>> SubHurts
    {
        get
        {
            return new()
            {
                FightPropertyFormat.ToNameValue(FightProperty.FIGHT_PROP_FIRE_SUB_HURT, FireSubHurt),
                FightPropertyFormat.ToNameValue(FightProperty.FIGHT_PROP_WATER_SUB_HURT, WaterSubHurt),
                FightPropertyFormat.ToNameValue(FightProperty.FIGHT_PROP_GRASS_SUB_HURT, GrassSubHurt),
                FightPropertyFormat.ToNameValue(FightProperty.FIGHT_PROP_ELEC_SUB_HURT, ElecSubHurt),
                FightPropertyFormat.ToNameValue(FightProperty.FIGHT_PROP_WIND_SUB_HURT, WindSubHurt),
                FightPropertyFormat.ToNameValue(FightProperty.FIGHT_PROP_ICE_SUB_HURT, IceSubHurt),
                FightPropertyFormat.ToNameValue(FightProperty.FIGHT_PROP_ROCK_SUB_HURT, RockSubHurt),
                FightPropertyFormat.ToNameValue(FightProperty.FIGHT_PROP_PHYSICAL_SUB_HURT, PhysicalSubHurt),
            };
        }
    }
}