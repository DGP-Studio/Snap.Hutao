// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Intrinsic;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.AvatarProperty;

/// <summary>
/// 角色属性值
/// </summary>
[HighQuality]
internal sealed class AvatarProperty : INameIcon
{
    private static readonly ImmutableDictionary<FightProperty, Uri> PropertyIcons = new Dictionary<FightProperty, Uri>()
    {
        [FightProperty.FIGHT_PROP_SKILL_CD_MINUS_RATIO] = Web.HutaoEndpoints.StaticFile("Property", "UI_Icon_CDReduce.png").ToUri(),
        [FightProperty.FIGHT_PROP_CHARGE_EFFICIENCY] = Web.HutaoEndpoints.StaticFile("Property", "UI_Icon_ChargeEfficiency.png").ToUri(),
        [FightProperty.FIGHT_PROP_CRITICAL] = Web.HutaoEndpoints.StaticFile("Property", "UI_Icon_Critical.png").ToUri(),
        [FightProperty.FIGHT_PROP_CUR_ATTACK] = Web.HutaoEndpoints.StaticFile("Property", "UI_Icon_CurAttack.png").ToUri(),
        [FightProperty.FIGHT_PROP_CUR_DEFENSE] = Web.HutaoEndpoints.StaticFile("Property", "UI_Icon_CurDefense.png").ToUri(),
        [FightProperty.FIGHT_PROP_ELEMENT_MASTERY] = Web.HutaoEndpoints.StaticFile("Property", "UI_Icon_Element.png").ToUri(),
        [FightProperty.FIGHT_PROP_ELEC_ADD_HURT] = Web.HutaoEndpoints.StaticFile("Property", "UI_Icon_Element_Electric.png").ToUri(),
        [FightProperty.FIGHT_PROP_FIRE_ADD_HURT] = Web.HutaoEndpoints.StaticFile("Property", "UI_Icon_Element_Fire.png").ToUri(),
        [FightProperty.FIGHT_PROP_GRASS_ADD_HURT] = Web.HutaoEndpoints.StaticFile("Property", "UI_Icon_Element_Grass.png").ToUri(),
        [FightProperty.FIGHT_PROP_ICE_ADD_HURT] = Web.HutaoEndpoints.StaticFile("Property", "UI_Icon_Element_Ice.png").ToUri(),
        [FightProperty.FIGHT_PROP_ROCK_ADD_HURT] = Web.HutaoEndpoints.StaticFile("Property", "UI_Icon_Element_Rock.png").ToUri(),
        [FightProperty.FIGHT_PROP_WATER_ADD_HURT] = Web.HutaoEndpoints.StaticFile("Property", "UI_Icon_Element_Water.png").ToUri(),
        [FightProperty.FIGHT_PROP_WIND_ADD_HURT] = Web.HutaoEndpoints.StaticFile("Property", "UI_Icon_Element_Wind.png").ToUri(),
        [FightProperty.FIGHT_PROP_HEAL_ADD] = Web.HutaoEndpoints.StaticFile("Property", "UI_Icon_Heal.png").ToUri(),
        [FightProperty.FIGHT_PROP_MAX_HP] = Web.HutaoEndpoints.StaticFile("Property", "UI_Icon_MaxHp.png").ToUri(),
        [FightProperty.FIGHT_PROP_PHYSICAL_ADD_HURT] = Web.HutaoEndpoints.StaticFile("Property", "UI_Icon_PhysicalAttackUp.png").ToUri(),
        [FightProperty.FIGHT_PROP_SHIELD_COST_MINUS_RATIO] = Web.HutaoEndpoints.StaticFile("Property", "UI_Icon_ShieldCostMinus.png").ToUri(),
    }.ToImmutableDictionary();

    /// <summary>
    /// 构造一个新的角色属性值
    /// </summary>
    /// <param name="property">战斗属性</param>
    /// <param name="name">名称</param>
    /// <param name="value">白字</param>
    /// <param name="addValue">绿字</param>
    public AvatarProperty(FightProperty property, string name, string value, string? addValue = null)
    {
        Name = name;
        Value = value;
        Icon = PropertyIcons.GetValueOrDefault(property)!;
        AddValue = addValue;
    }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 图标
    /// </summary>
    public Uri Icon { get; }

    /// <summary>
    /// 白字
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// 绿字
    /// </summary>
    public string? AddValue { get; }
}