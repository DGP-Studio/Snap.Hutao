// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Extension;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Binding.AvatarProperty;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Annotation;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Converter;
using ModelPlayerInfo = Snap.Hutao.Web.Enka.Model.PlayerInfo;

namespace Snap.Hutao.Service.AvatarInfo.Factory;

/// <summary>
/// 简述帮助类
/// </summary>
internal static class SummaryHelper
{
    /// <summary>
    /// 创建玩家对象
    /// </summary>
    /// <param name="playerInfo">玩家信息</param>
    /// <returns>玩家对象</returns>
    public static Player CreatePlayer(ModelPlayerInfo playerInfo)
    {
        return new()
        {
            Nickname = playerInfo.Nickname,
            Level = playerInfo.Level,
            Signature = playerInfo.Signature,
            FinishAchievementNumber = playerInfo.FinishAchievementNum,
            SipralAbyssFloorLevel = $"{playerInfo.TowerFloorIndex} - {playerInfo.TowerLevelIndex}",
        };
    }

    /// <summary>
    /// 创建命之座
    /// </summary>
    /// <param name="talentIds">激活的命座列表</param>
    /// <param name="talents">全部命座</param>
    /// <returns>命之座</returns>
    public static List<Constellation> CreateConstellations(IList<int>? talentIds, IList<SkillBase> talents)
    {
        return talents.Select(talent => new Constellation()
        {
            Name = talent.Name,
            Icon = SkillIconConverter.IconNameToUri(talent.Icon),
            Description = talent.Description,
            IsActivated = talentIds?.Contains(talent.Id) ?? false,
        }).ToList();
    }

    /// <summary>
    /// 创建技能组
    /// </summary>
    /// <param name="skillLevelMap">技能等级映射</param>
    /// <param name="proudSkillExtraLevelMap">额外提升等级映射</param>
    /// <param name="proudSkills">技能列表</param>
    /// <returns>技能</returns>
    public static List<Skill> CreateSkills(IDictionary<string, int> skillLevelMap, IDictionary<string, int>? proudSkillExtraLevelMap, IEnumerable<ProudableSkill> proudSkills)
    {
        if (skillLevelMap == null)
        {
            return new();
        }

        Dictionary<string, int> skillExtraLeveledMap = new(skillLevelMap);

        if (proudSkillExtraLevelMap != null)
        {
            foreach ((string skillGroupId, int extraLevel) in proudSkillExtraLevelMap)
            {
                int skillGroupIdInt32 = int.Parse(skillGroupId);
                int skillId = proudSkills.Single(p => p.GroupId == skillGroupIdInt32).Id;

                skillExtraLeveledMap.Increase(skillId.ToString(), extraLevel);
            }
        }

        List<Skill> skills = new();

        foreach (ProudableSkill proudableSkill in proudSkills)
        {
            Skill skill = new()
            {
                Name = proudableSkill.Name,
                Icon = SkillIconConverter.IconNameToUri(proudableSkill.Icon),
                Description = proudableSkill.Description,

                GroupId = proudableSkill.GroupId,
                LevelNumber = skillLevelMap[proudableSkill.Id.ToString()],
                Info = DescParamDescriptor.Convert(proudableSkill.Proud, skillExtraLeveledMap[proudableSkill.Id.ToString()]),
            };

            skills.Add(skill);
        }

        return skills;
    }

    /// <summary>
    /// 创建角色属性
    /// </summary>
    /// <param name="fightPropMap">属性映射</param>
    /// <returns>列表</returns>
    public static List<Pair2<string, string, string?>> CreateAvatarProperties(IDictionary<FightProperty, double> fightPropMap)
    {
        if (fightPropMap == null)
        {
            return new();
        }

        double baseHp = fightPropMap.GetValueOrDefault2(FightProperty.FIGHT_PROP_BASE_HP);                 // 1
        double hp = fightPropMap.GetValueOrDefault2(FightProperty.FIGHT_PROP_HP);                          // 2
        double hpPercent = fightPropMap.GetValueOrDefault2(FightProperty.FIGHT_PROP_HP_PERCENT);           // 3
        double hpAdd = hp + (baseHp * hpPercent);
        Pair2<string, string, string?> hpPair2 = PropertyInfoDescriptor.FormatIntegerPair2("生命值", FormatMethod.Integer, baseHp, hpAdd);

        double baseAtk = fightPropMap.GetValueOrDefault2(FightProperty.FIGHT_PROP_BASE_ATTACK);            // 4
        double atk = fightPropMap.GetValueOrDefault2(FightProperty.FIGHT_PROP_ATTACK);                     // 5
        double atkPrecent = fightPropMap.GetValueOrDefault2(FightProperty.FIGHT_PROP_ATTACK_PERCENT);      // 6
        double atkAdd = atk + (baseAtk * atkPrecent);
        Pair2<string, string, string?> atkPair2 = PropertyInfoDescriptor.FormatIntegerPair2("攻击力", FormatMethod.Integer, baseAtk, atkAdd);

        double baseDef = fightPropMap.GetValueOrDefault2(FightProperty.FIGHT_PROP_BASE_DEFENSE);           // 7
        double def = fightPropMap.GetValueOrDefault2(FightProperty.FIGHT_PROP_DEFENSE);                    // 8
        double defPercent = fightPropMap.GetValueOrDefault2(FightProperty.FIGHT_PROP_DEFENSE_PERCENT);     // 9
        double defAdd = def + (baseDef * defPercent);
        Pair2<string, string, string?> defPair2 = PropertyInfoDescriptor.FormatIntegerPair2("防御力", FormatMethod.Integer, baseDef, defAdd);

        double em = fightPropMap.GetValueOrDefault2(FightProperty.FIGHT_PROP_ELEMENT_MASTERY);             // 28
        Pair2<string, string, string?> emPair2 = PropertyInfoDescriptor.FormatIntegerPair2("元素精通", FormatMethod.Integer, em);

        double critRate = fightPropMap.GetValueOrDefault2(FightProperty.FIGHT_PROP_CRITICAL);              // 20
        Pair2<string, string, string?> critRatePair2 = PropertyInfoDescriptor.FormatIntegerPair2("暴击率", FormatMethod.Percent, critRate);

        double critDMG = fightPropMap.GetValueOrDefault2(FightProperty.FIGHT_PROP_CRITICAL_HURT);          // 22
        Pair2<string, string, string?> critDMGPair2 = PropertyInfoDescriptor.FormatIntegerPair2("暴击伤害", FormatMethod.Percent, critDMG);

        double chargeEff = fightPropMap.GetValueOrDefault2(FightProperty.FIGHT_PROP_CHARGE_EFFICIENCY);    // 23
        Pair2<string, string, string?> chargeEffPair2 = PropertyInfoDescriptor.FormatIntegerPair2("元素充能效率", FormatMethod.Percent, chargeEff);

        List<Pair2<string, string, string?>> properties = new() { hpPair2, atkPair2, defPair2, emPair2, critRatePair2, critDMGPair2, chargeEffPair2 };

        FightProperty bonusProperty = GetBonusFightProperty(fightPropMap);
        if (bonusProperty != FightProperty.FIGHT_PROP_NONE)
        {
            double value = fightPropMap[bonusProperty];
            if (value > 0)
            {
                Pair2<string, string, string?> bonusPair2 = new(bonusProperty.GetDescription(), PropertyInfoDescriptor.FormatValue(FormatMethod.Percent, value), null);
                properties.Add(bonusPair2);
            }
        }

        // 物伤
        if (fightPropMap.TryGetValue(FightProperty.FIGHT_PROP_PHYSICAL_ADD_HURT, out double addValue))
        {
            if (addValue > 0)
            {
                string description = FightProperty.FIGHT_PROP_PHYSICAL_ADD_HURT.GetDescription();
                Pair2<string, string, string?> physicalBonusPair2 = new(description, PropertyInfoDescriptor.FormatValue(FormatMethod.Percent, addValue), null);
                properties.Add(physicalBonusPair2);
            }
        }

        return properties;
    }

    /// <summary>
    /// 获取副属性对应的最大属性的Id
    /// </summary>
    /// <param name="appendId">属性Id</param>
    /// <returns>最大属性Id</returns>
    public static int GetAffixMaxId(int appendId)
    {
        int value = appendId / 100000;
        int max = value switch
        {
            1 => 2,
            2 => 3,
            3 or 4 or 5 => 4,
            _ => throw Must.NeverHappen(),
        };

        return (appendId / 10 * 10) + max;
    }

    /// <summary>
    /// 获取百分比属性副词条分数
    /// </summary>
    /// <param name="appendId">id</param>
    /// <returns>分数</returns>
    public static double GetPercentSubAffixScore(int appendId)
    {
        int maxId = GetAffixMaxId(appendId);
        int delta = maxId - appendId;

        return (maxId / 100000, delta) switch
        {
            (5, 0) => 100,
            (5, 1) => 90,
            (5, 2) => 80,
            (5, 3) => 70,

            (4, 0) => 100,
            (4, 1) => 90,
            (4, 2) => 80,
            (4, 3) => 70,

            (3, 0) => 100,
            (3, 1) => 85,
            (3, 2) => 70,

            (2, 0) => 100,
            (2, 1) => 80,

            // TODO: Not quite sure why can we hit this branch.
            _ => 0,
        };
    }

    /// <summary>
    /// 获取双爆评分
    /// </summary>
    /// <param name="fightPropMap">属性</param>
    /// <returns>评分</returns>
    public static double ScoreCrit(IDictionary<FightProperty, double> fightPropMap)
    {
        if (fightPropMap == null)
        {
            return 0.0;
        }

        double cr = fightPropMap[FightProperty.FIGHT_PROP_CRITICAL];
        double cd = fightPropMap[FightProperty.FIGHT_PROP_CRITICAL_HURT];

        return 100 * ((cr * 2) + cd);
    }

    private static FightProperty GetBonusFightProperty(IDictionary<FightProperty, double> fightPropMap)
    {
        if (fightPropMap.ContainsKey(FightProperty.FIGHT_PROP_MAX_FIRE_ENERGY))
        {
            return FightProperty.FIGHT_PROP_FIRE_ADD_HURT;
        }

        if (fightPropMap.ContainsKey(FightProperty.FIGHT_PROP_MAX_ELEC_ENERGY))
        {
            return FightProperty.FIGHT_PROP_ELEC_ADD_HURT;
        }

        if (fightPropMap.ContainsKey(FightProperty.FIGHT_PROP_MAX_WATER_ENERGY))
        {
            return FightProperty.FIGHT_PROP_WATER_ADD_HURT;
        }

        if (fightPropMap.ContainsKey(FightProperty.FIGHT_PROP_MAX_GRASS_ENERGY))
        {
            return FightProperty.FIGHT_PROP_GRASS_ADD_HURT;
        }

        if (fightPropMap.ContainsKey(FightProperty.FIGHT_PROP_MAX_WIND_ENERGY))
        {
            return FightProperty.FIGHT_PROP_WIND_ADD_HURT;
        }

        if (fightPropMap.ContainsKey(FightProperty.FIGHT_PROP_MAX_ICE_ENERGY))
        {
            return FightProperty.FIGHT_PROP_ICE_ADD_HURT;
        }

        if (fightPropMap.ContainsKey(FightProperty.FIGHT_PROP_MAX_ROCK_ENERGY))
        {
            return FightProperty.FIGHT_PROP_ROCK_ADD_HURT;
        }

        return FightProperty.FIGHT_PROP_NONE;
    }
}