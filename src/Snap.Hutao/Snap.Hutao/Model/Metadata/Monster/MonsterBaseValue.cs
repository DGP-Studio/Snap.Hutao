// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

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
                Converter.PropertyDescriptor.FormatNameValue(Intrinsic.FightProperty.FIGHT_PROP_FIRE_SUB_HURT, FireSubHurt),
                Converter.PropertyDescriptor.FormatNameValue(Intrinsic.FightProperty.FIGHT_PROP_WATER_SUB_HURT, WaterSubHurt),
                Converter.PropertyDescriptor.FormatNameValue(Intrinsic.FightProperty.FIGHT_PROP_GRASS_SUB_HURT, GrassSubHurt),
                Converter.PropertyDescriptor.FormatNameValue(Intrinsic.FightProperty.FIGHT_PROP_ELEC_SUB_HURT, ElecSubHurt),
                Converter.PropertyDescriptor.FormatNameValue(Intrinsic.FightProperty.FIGHT_PROP_WIND_SUB_HURT, WindSubHurt),
                Converter.PropertyDescriptor.FormatNameValue(Intrinsic.FightProperty.FIGHT_PROP_ICE_SUB_HURT, IceSubHurt),
                Converter.PropertyDescriptor.FormatNameValue(Intrinsic.FightProperty.FIGHT_PROP_ROCK_SUB_HURT, RockSubHurt),
                Converter.PropertyDescriptor.FormatNameValue(Intrinsic.FightProperty.FIGHT_PROP_PHYSICAL_SUB_HURT, PhysicalSubHurt),
            };
        }
    }
}