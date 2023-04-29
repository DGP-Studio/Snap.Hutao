// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.ViewModel.AvatarProperty;

namespace Snap.Hutao.Service.AvatarInfo.Factory;

/// <summary>
/// 简述战斗属性帮助类
/// </summary>
[HighQuality]
internal static class SummaryAvatarProperties
{
    /// <summary>
    /// 创建角色属性
    /// </summary>
    /// <param name="fightPropMap">属性映射</param>
    /// <returns>列表</returns>
    public static List<AvatarProperty> Create(Dictionary<FightProperty, float> fightPropMap)
    {
        if (fightPropMap == null)
        {
            return new();
        }

        AvatarProperty hpProp = ToAvatarProperty(FightProperty.FIGHT_PROP_BASE_HP, fightPropMap);
        AvatarProperty atkProp = ToAvatarProperty(FightProperty.FIGHT_PROP_BASE_ATTACK, fightPropMap);
        AvatarProperty defProp = ToAvatarProperty(FightProperty.FIGHT_PROP_BASE_DEFENSE, fightPropMap);
        AvatarProperty emProp = ToAvatarProperty(FightProperty.FIGHT_PROP_ELEMENT_MASTERY, fightPropMap);
        AvatarProperty critRateProp = ToAvatarProperty(FightProperty.FIGHT_PROP_CRITICAL, fightPropMap);
        AvatarProperty critDMGProp = ToAvatarProperty(FightProperty.FIGHT_PROP_CRITICAL_HURT, fightPropMap);
        AvatarProperty chargeEffProp = ToAvatarProperty(FightProperty.FIGHT_PROP_CHARGE_EFFICIENCY, fightPropMap);

        List<AvatarProperty> properties = new(9) { hpProp, atkProp, defProp, emProp, critRateProp, critDMGProp, chargeEffProp };

        // 元素伤害
        if (TryGetBonusFightProperty(fightPropMap, out FightProperty bonusProperty))
        {
            float value = fightPropMap[bonusProperty];
            if (value > 0)
            {
                properties.Add(FightPropertyFormat.ToAvatarProperty(bonusProperty, value));
            }
        }

        // 物伤 可以和其他元素伤害并存，所以分别判断
        if (fightPropMap.TryGetValue(FightProperty.FIGHT_PROP_PHYSICAL_ADD_HURT, out float addValue))
        {
            if (addValue > 0)
            {
                properties.Add(FightPropertyFormat.ToAvatarProperty(FightProperty.FIGHT_PROP_PHYSICAL_ADD_HURT, addValue));
            }
        }

        return properties;
    }

    private static AvatarProperty ToAvatarProperty(FightProperty baseProp, Dictionary<FightProperty, float> fightPropMap)
    {
        // 1 2 3 2000
        // 4 5 6 2001
        // 7 8 9 2002
        float baseHp = fightPropMap.GetValueOrDefault(baseProp);
        float hp = fightPropMap.GetValueOrDefault(baseProp + 1);
        float hpPercent = fightPropMap.GetValueOrDefault(baseProp + 2);
        float hpAdd = hp + (baseHp * hpPercent);

        return FightPropertyFormat.ToAvatarProperty(baseProp + 1999, baseHp, hpAdd);
    }

    private static bool TryGetBonusFightProperty(Dictionary<FightProperty, float> fightPropMap, out FightProperty value)
    {
        if (fightPropMap.ContainsKey(FightProperty.FIGHT_PROP_MAX_FIRE_ENERGY))
        {
            value = FightProperty.FIGHT_PROP_FIRE_ADD_HURT; // 70 40
            return true;
        }

        if (fightPropMap.ContainsKey(FightProperty.FIGHT_PROP_MAX_ELEC_ENERGY))
        {
            value = FightProperty.FIGHT_PROP_ELEC_ADD_HURT; // 71 41
            return true;
        }

        if (fightPropMap.ContainsKey(FightProperty.FIGHT_PROP_MAX_WATER_ENERGY))
        {
            value = FightProperty.FIGHT_PROP_WATER_ADD_HURT; // 72 42
            return true;
        }

        if (fightPropMap.ContainsKey(FightProperty.FIGHT_PROP_MAX_GRASS_ENERGY))
        {
            value = FightProperty.FIGHT_PROP_GRASS_ADD_HURT; // 73 43
            return true;
        }

        if (fightPropMap.ContainsKey(FightProperty.FIGHT_PROP_MAX_WIND_ENERGY))
        {
            value = FightProperty.FIGHT_PROP_WIND_ADD_HURT; // 74 44
            return true;
        }

        if (fightPropMap.ContainsKey(FightProperty.FIGHT_PROP_MAX_ICE_ENERGY))
        {
            value = FightProperty.FIGHT_PROP_ICE_ADD_HURT; // 75 46
            return true;
        }

        if (fightPropMap.ContainsKey(FightProperty.FIGHT_PROP_MAX_ROCK_ENERGY))
        {
            value = FightProperty.FIGHT_PROP_ROCK_ADD_HURT; // 76 45
            return true;
        }

        value = FightProperty.FIGHT_PROP_NONE;
        return false;
    }
}