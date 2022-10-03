// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Extension;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Binding.AvatarProperty;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Annotation;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Web.Enka.Model;
using MetadataAvatar = Snap.Hutao.Model.Metadata.Avatar.Avatar;
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
    /// <param name="avatar">角色</param>
    /// <returns>玩家对象</returns>
    public static Player CreatePlayer(ModelPlayerInfo playerInfo, MetadataAvatar avatar)
    {
        return new()
        {
            Nickname = playerInfo.Nickname,
            Level = playerInfo.Level,
            Signature = playerInfo.Signature,
            FinishAchievementNumber = playerInfo.FinishAchievementNum,
            SipralAbyssFloorLevel = $"{playerInfo.TowerFloorIndex} - {playerInfo.TowerLevelIndex}",
            ProfilePicture = AvatarIconConverter.IconNameToUri(GetIconName(playerInfo.ProfilePicture, avatar)),
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
        List<Constellation> constellations = new();

        foreach (SkillBase talent in talents)
        {
            Constellation constellation = new()
            {
                Name = talent.Name,
                Icon = SkillIconConverter.IconNameToUri(talent.Icon),
                Description = talent.Description,
                IsActiviated = talentIds?.Contains(talent.Id) ?? false,
            };

            constellations.Add(constellation);
        }

        return constellations;
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
        Dictionary<string, int> skillLevelMapCopy = new(skillLevelMap);

        if (proudSkillExtraLevelMap != null)
        {
            foreach ((string skillGroupId, int extraLevel) in proudSkillExtraLevelMap)
            {
                int skillGroupIdInt32 = int.Parse(skillGroupId);
                int skillId = proudSkills.Single(p => p.GroupId == skillGroupIdInt32).Id;

                skillLevelMapCopy.Increase($"{skillId}", extraLevel);
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
                Info = DescParamDescriptor.Convert(proudableSkill.Proud, skillLevelMapCopy[$"{proudableSkill.Id}"]),
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
        List<Pair2<string, string, string?>> properties;

        double baseHp = fightPropMap.GetValueOrDefault(FightProperty.FIGHT_PROP_BASE_HP);                 // 1
        double hp = fightPropMap.GetValueOrDefault(FightProperty.FIGHT_PROP_HP);                          // 2
        double hpPercent = fightPropMap.GetValueOrDefault(FightProperty.FIGHT_PROP_HP_PERCENT);           // 3
        double hpAdd = hp + (baseHp * hpPercent);
        double maxHp = baseHp + hpAdd;
        Pair2<string, string, string?> hpPair2 = new("生命值", FormatValue(FormatMethod.Integer, maxHp), $"[+{FormatValue(FormatMethod.Integer, hpAdd)}]");

        double baseAtk = fightPropMap.GetValueOrDefault(FightProperty.FIGHT_PROP_BASE_ATTACK);            // 4
        double atk = fightPropMap.GetValueOrDefault(FightProperty.FIGHT_PROP_ATTACK);                     // 5
        double atkPrecent = fightPropMap.GetValueOrDefault(FightProperty.FIGHT_PROP_ATTACK_PERCENT);      // 6
        double atkAdd = atk + (baseAtk * atkPrecent);
        double maxAtk = baseAtk + atkAdd;
        Pair2<string, string, string?> atkPair2 = new("攻击力", FormatValue(FormatMethod.Integer, maxAtk), $"[+{FormatValue(FormatMethod.Integer, atkAdd)}]");

        double baseDef = fightPropMap.GetValueOrDefault(FightProperty.FIGHT_PROP_BASE_DEFENSE);           // 7
        double def = fightPropMap.GetValueOrDefault(FightProperty.FIGHT_PROP_DEFENSE);                    // 8
        double defPercent = fightPropMap.GetValueOrDefault(FightProperty.FIGHT_PROP_DEFENSE_PERCENT);     // 9
        double defAdd = def + (baseDef * defPercent);
        double maxDef = baseDef + defPercent;
        Pair2<string, string, string?> defPair2 = new("防御力", FormatValue(FormatMethod.Integer, maxDef), $"[+{FormatValue(FormatMethod.Integer, defAdd)}]");

        double em = fightPropMap.GetValueOrDefault(FightProperty.FIGHT_PROP_ELEMENT_MASTERY);             // 28
        Pair2<string, string, string?> emPair2 = new("元素精通", FormatValue(FormatMethod.Integer, em), null);

        double critRate = fightPropMap.GetValueOrDefault(FightProperty.FIGHT_PROP_CRITICAL);              // 20
        Pair2<string, string, string?> critRatePair2 = new("暴击率", FormatValue(FormatMethod.Percent, critRate), null);

        double critDMG = fightPropMap.GetValueOrDefault(FightProperty.FIGHT_PROP_CRITICAL_HURT);          // 22
        Pair2<string, string, string?> critDMGPair2 = new("暴击伤害", FormatValue(FormatMethod.Percent, critDMG), null);

        double chargeEff = fightPropMap.GetValueOrDefault(FightProperty.FIGHT_PROP_CHARGE_EFFICIENCY);    // 23
        Pair2<string, string, string?> chargeEffPair2 = new("元素充能效率", FormatValue(FormatMethod.Percent, chargeEff), null);

        properties = new() { hpPair2, atkPair2, defPair2, emPair2, critRatePair2, critDMGPair2, chargeEffPair2 };

        FightProperty bonusProperty = GetBonusFightProperty(fightPropMap);
        if (bonusProperty != FightProperty.FIGHT_PROP_NONE)
        {
            double value = fightPropMap[bonusProperty];
            Pair2<string, string, string?> bonusPair2 = new(bonusProperty.GetDescription(), FormatValue(FormatMethod.Percent, value), null);
            properties.Add(bonusPair2);
        }

        return properties;
    }

    private static string FormatValue(FormatMethod method, double value)
    {
        return method switch
        {
            FormatMethod.Integer => Math.Round((double)value, MidpointRounding.AwayFromZero).ToString(),
            FormatMethod.Percent => string.Format("{0:P1}", value),
            _ => value.ToString(),
        };
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

        // 物伤
        if (fightPropMap.ContainsKey(FightProperty.FIGHT_PROP_PHYSICAL_ADD_HURT))
        {
            return FightProperty.FIGHT_PROP_PHYSICAL_ADD_HURT;
        }

        return FightProperty.FIGHT_PROP_NONE;
    }

    private static string GetIconName(ProfilePicture profilePicture, MetadataAvatar avatar)
    {
        if (profilePicture.CostumeId != null)
        {
            return avatar.Costumes.Single(c => c.Id == profilePicture.CostumeId).Icon ?? string.Empty;
        }

        return avatar.Icon;
    }
}