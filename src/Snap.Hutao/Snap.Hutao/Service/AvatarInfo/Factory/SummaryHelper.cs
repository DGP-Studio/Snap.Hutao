// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.AvatarProperty;

namespace Snap.Hutao.Service.AvatarInfo.Factory;

/// <summary>
/// 简述帮助类
/// </summary>
[HighQuality]
internal static class SummaryHelper
{
    /// <summary>
    /// 创建命之座
    /// </summary>
    /// <param name="talents">全部命座</param>
    /// <param name="talentIds">激活的命座列表</param>
    /// <returns>命之座</returns>
    public static List<ConstellationView> CreateConstellations(List<Skill> talents, List<SkillId>? talentIds)
    {
        return talents.SelectList(talent => new ConstellationView()
        {
            Name = talent.Name,
            Icon = SkillIconConverter.IconNameToUri(talent.Icon),
            Description = talent.Description,
            IsActivated = talentIds?.Contains(talent.Id) ?? false,
        });
    }

    /// <summary>
    /// 创建技能组
    /// </summary>
    /// <param name="skillLevelMap">技能等级映射</param>
    /// <param name="proudSkillExtraLevelMap">额外提升等级映射</param>
    /// <param name="proudSkills">技能列表</param>
    /// <returns>技能</returns>
    public static List<SkillView> CreateSkills(Dictionary<string, int> skillLevelMap, Dictionary<string, int>? proudSkillExtraLevelMap, List<ProudableSkill> proudSkills)
    {
        if (skillLevelMap == null)
        {
            return new();
        }

        Dictionary<string, int> skillExtraLeveledMap = new(skillLevelMap);

        if (proudSkillExtraLevelMap != null)
        {
            foreach ((string skillGroupIdString, int extraLevel) in proudSkillExtraLevelMap)
            {
                SkillGroupId skillGroupId = int.Parse(skillGroupIdString);
                SkillId skillId = proudSkills.Single(p => p.GroupId == skillGroupId).Id;

                skillExtraLeveledMap.Increase($"{skillId.Value}", extraLevel);
            }
        }

        return proudSkills.SelectList(proudableSkill =>
        {
            string skillId = $"{proudableSkill.Id.Value}";

            return new SkillView()
            {
                Name = proudableSkill.Name,
                Icon = SkillIconConverter.IconNameToUri(proudableSkill.Icon),
                Description = proudableSkill.Description,

                GroupId = proudableSkill.GroupId,
                LevelNumber = skillLevelMap[skillId],
                Info = DescriptionsParametersDescriptor.Convert(proudableSkill.Proud, skillExtraLeveledMap[skillId]),
            };
        });
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
    public static float GetPercentSubAffixScore(int appendId)
    {
        int maxId = GetAffixMaxId(appendId);
        int delta = maxId - appendId;

        return (maxId / 100000, delta) switch
        {
            (5, 0) => 100F,
            (5, 1) => 90F,
            (5, 2) => 80F,
            (5, 3) => 70F,

            (4, 0) => 100F,
            (4, 1) => 90F,
            (4, 2) => 80F,
            (4, 3) => 70F,

            (3, 0) => 100F,
            (3, 1) => 85F,
            (3, 2) => 70F,

            (2, 0) => 100F,
            (2, 1) => 80F,

            // TODO: Not quite sure why can we hit this branch.
            _ => 0F,
        };
    }

    /// <summary>
    /// 获取双爆评分
    /// </summary>
    /// <param name="fightPropMap">属性</param>
    /// <returns>评分</returns>
    public static float ScoreCrit(Dictionary<FightProperty, float> fightPropMap)
    {
        if (fightPropMap == null)
        {
            return 0F;
        }

        float cr = fightPropMap[FightProperty.FIGHT_PROP_CRITICAL];
        float cd = fightPropMap[FightProperty.FIGHT_PROP_CRITICAL_HURT];

        return 100 * ((cr * 2) + cd);
    }
}