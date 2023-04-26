// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Annotation;
using Snap.Hutao.ViewModel.AvatarProperty;

namespace Snap.Hutao.Service.AvatarInfo.Factory;

/// <summary>
/// 简述战斗属性帮助类
/// </summary>
[HighQuality]
internal static class SummaryFightPropertyMapHelper
{
    /// <summary>
    /// 创建角色属性
    /// </summary>
    /// <param name="fightPropMap">属性映射</param>
    /// <returns>列表</returns>
    public static List<AvatarProperty> CreateAvatarProperties(Dictionary<FightProperty, double> fightPropMap)
    {
        if (fightPropMap == null)
        {
            return new();
        }

        AvatarProperty hpProp = GetHpProperty(fightPropMap);
        AvatarProperty atkProp = GetAtkProperty(fightPropMap);
        AvatarProperty defProp = GetDefProperty(fightPropMap);

        double em = fightPropMap.GetValueOrDefault(FightProperty.FIGHT_PROP_ELEMENT_MASTERY);             // 28
        AvatarProperty emProp = Model.Metadata.Converter.PropertiesParametersDescriptor.FormatAvatarProperty(
            FightProperty.FIGHT_PROP_ELEMENT_MASTERY, SH.ServiceAvatarInfoPropertyEM, FormatMethod.Integer, em);

        double critRate = fightPropMap.GetValueOrDefault(FightProperty.FIGHT_PROP_CRITICAL);              // 20
        AvatarProperty critRateProp = Model.Metadata.Converter.PropertiesParametersDescriptor.FormatAvatarProperty(
            FightProperty.FIGHT_PROP_CRITICAL, SH.ServiceAvatarInfoPropertyCR, FormatMethod.Percent, critRate);

        double critDMG = fightPropMap.GetValueOrDefault(FightProperty.FIGHT_PROP_CRITICAL_HURT);          // 22
        AvatarProperty critDMGProp = Model.Metadata.Converter.PropertiesParametersDescriptor.FormatAvatarProperty(
            FightProperty.FIGHT_PROP_CRITICAL_HURT, SH.ServiceAvatarInfoPropertyCDmg, FormatMethod.Percent, critDMG);

        double chargeEff = fightPropMap.GetValueOrDefault(FightProperty.FIGHT_PROP_CHARGE_EFFICIENCY);    // 23
        AvatarProperty chargeEffProp = Model.Metadata.Converter.PropertiesParametersDescriptor.FormatAvatarProperty(
            FightProperty.FIGHT_PROP_CHARGE_EFFICIENCY, SH.ServiceAvatarInfoPropertyCE, FormatMethod.Percent, chargeEff);

        List<AvatarProperty> properties = new(9) { hpProp, atkProp, defProp, emProp, critRateProp, critDMGProp, chargeEffProp };

        // 元素伤害
        FightProperty bonusProperty = GetBonusFightProperty(fightPropMap);
        if (bonusProperty != FightProperty.FIGHT_PROP_NONE)
        {
            double value = fightPropMap[bonusProperty];
            if (value > 0)
            {
                AvatarProperty bonusProp = Model.Metadata.Converter.PropertiesParametersDescriptor.FormatAvatarProperty(
                    bonusProperty, bonusProperty.GetLocalizedDescription(), FormatMethod.Percent, value);
                properties.Add(bonusProp);
            }
        }

        // 物伤
        if (fightPropMap.TryGetValue(FightProperty.FIGHT_PROP_PHYSICAL_ADD_HURT, out double addValue))
        {
            if (addValue > 0)
            {
                string description = FightProperty.FIGHT_PROP_PHYSICAL_ADD_HURT.GetLocalizedDescription();
                AvatarProperty physicalBonusProp = Model.Metadata.Converter.PropertiesParametersDescriptor.FormatAvatarProperty(
                    FightProperty.FIGHT_PROP_PHYSICAL_ADD_HURT, description, FormatMethod.Percent, addValue);
                properties.Add(physicalBonusProp);
            }
        }

        return properties;
    }

    private static AvatarProperty GetHpProperty(Dictionary<FightProperty, double> fightPropMap)
    {
        double baseHp = fightPropMap.GetValueOrDefault(FightProperty.FIGHT_PROP_BASE_HP);                 // 1
        double hp = fightPropMap.GetValueOrDefault(FightProperty.FIGHT_PROP_HP);                          // 2
        double hpPercent = fightPropMap.GetValueOrDefault(FightProperty.FIGHT_PROP_HP_PERCENT);           // 3
        double hpAdd = hp + (baseHp * hpPercent);
        AvatarProperty hpProp = Model.Metadata.Converter.PropertiesParametersDescriptor.FormatAvatarProperty(
            FightProperty.FIGHT_PROP_MAX_HP, SH.ServiceAvatarInfoPropertyHp, FormatMethod.Integer, baseHp, hpAdd);
        return hpProp;
    }

    private static AvatarProperty GetAtkProperty(Dictionary<FightProperty, double> fightPropMap)
    {
        double baseAtk = fightPropMap.GetValueOrDefault(FightProperty.FIGHT_PROP_BASE_ATTACK);            // 4
        double atk = fightPropMap.GetValueOrDefault(FightProperty.FIGHT_PROP_ATTACK);                     // 5
        double atkPrecent = fightPropMap.GetValueOrDefault(FightProperty.FIGHT_PROP_ATTACK_PERCENT);      // 6
        double atkAdd = atk + (baseAtk * atkPrecent);
        AvatarProperty atkProp = Model.Metadata.Converter.PropertiesParametersDescriptor.FormatAvatarProperty(
            FightProperty.FIGHT_PROP_CUR_ATTACK, SH.ServiceAvatarInfoPropertyAtk, FormatMethod.Integer, baseAtk, atkAdd);
        return atkProp;
    }

    private static AvatarProperty GetDefProperty(Dictionary<FightProperty, double> fightPropMap)
    {
        double baseDef = fightPropMap.GetValueOrDefault(FightProperty.FIGHT_PROP_BASE_DEFENSE);           // 7
        double def = fightPropMap.GetValueOrDefault(FightProperty.FIGHT_PROP_DEFENSE);                    // 8
        double defPercent = fightPropMap.GetValueOrDefault(FightProperty.FIGHT_PROP_DEFENSE_PERCENT);     // 9
        double defAdd = def + (baseDef * defPercent);
        AvatarProperty defProp = Model.Metadata.Converter.PropertiesParametersDescriptor.FormatAvatarProperty(
            FightProperty.FIGHT_PROP_CUR_DEFENSE, SH.ServiceAvatarInfoPropertyDef, FormatMethod.Integer, baseDef, defAdd);
        return defProp;
    }

    private static FightProperty GetBonusFightProperty(IDictionary<FightProperty, double> fightPropMap)
    {
        if (fightPropMap.ContainsKey(FightProperty.FIGHT_PROP_MAX_FIRE_ENERGY))
        {
            return FightProperty.FIGHT_PROP_FIRE_ADD_HURT; // 70 40
        }

        if (fightPropMap.ContainsKey(FightProperty.FIGHT_PROP_MAX_ELEC_ENERGY))
        {
            return FightProperty.FIGHT_PROP_ELEC_ADD_HURT; // 71 41
        }

        if (fightPropMap.ContainsKey(FightProperty.FIGHT_PROP_MAX_WATER_ENERGY))
        {
            return FightProperty.FIGHT_PROP_WATER_ADD_HURT; // 72 42
        }

        if (fightPropMap.ContainsKey(FightProperty.FIGHT_PROP_MAX_GRASS_ENERGY))
        {
            return FightProperty.FIGHT_PROP_GRASS_ADD_HURT; // 73 43
        }

        if (fightPropMap.ContainsKey(FightProperty.FIGHT_PROP_MAX_WIND_ENERGY))
        {
            return FightProperty.FIGHT_PROP_WIND_ADD_HURT; // 74 44
        }

        if (fightPropMap.ContainsKey(FightProperty.FIGHT_PROP_MAX_ICE_ENERGY))
        {
            return FightProperty.FIGHT_PROP_ICE_ADD_HURT; // 75 46
        }

        if (fightPropMap.ContainsKey(FightProperty.FIGHT_PROP_MAX_ROCK_ENERGY))
        {
            return FightProperty.FIGHT_PROP_ROCK_ADD_HURT; // 76 45
        }

        return FightProperty.FIGHT_PROP_NONE;
    }
}