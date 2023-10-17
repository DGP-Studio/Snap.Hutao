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
        talentIds ??= new();

        return talents.SelectList(talent => new ConstellationView()
        {
            Name = talent.Name,
            Icon = SkillIconConverter.IconNameToUri(talent.Icon),
            Description = talent.Description,
            IsActivated = talentIds.Contains(talent.Id),
        });
    }

    /// <summary>
    /// 创建技能组
    /// </summary>
    /// <param name="skillLevelMap">技能等级映射</param>
    /// <param name="proudSkillExtraLevelMap">额外提升等级映射</param>
    /// <param name="proudSkills">技能列表</param>
    /// <returns>技能</returns>
    public static List<SkillView> CreateSkills(Dictionary<SkillId, SkillLevel>? skillLevelMap, Dictionary<SkillGroupId, SkillLevel>? proudSkillExtraLevelMap, List<ProudableSkill> proudSkills)
    {
        if (skillLevelMap.IsNullOrEmpty())
        {
            return new();
        }

        Dictionary<SkillId, SkillLevel> skillExtraLeveledMap = new(skillLevelMap);

        if (proudSkillExtraLevelMap is not null)
        {
            foreach ((SkillGroupId groupId, SkillLevel extraLevel) in proudSkillExtraLevelMap)
            {
                skillExtraLeveledMap.IncreaseValue(proudSkills.Single(p => p.GroupId == groupId).Id, extraLevel);
            }
        }

        return proudSkills.SelectList(proudableSkill =>
        {
            SkillId skillId = proudableSkill.Id;

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
    public static ReliquarySubAffixId GetAffixMaxId(in ReliquarySubAffixId appendId)
    {
        // axxxxx -> a
        uint value = appendId / 100000U;

        // a -> m
        uint max = value switch
        {
            1 => 2,
            2 => 3,
            3 or 4 or 5 => 4,
            _ => throw Must.NeverHappen(),
        };

        // axxxxb -> axxxx -> axxxx0 -> axxxxm
        return ((appendId / 10) * 10) + max;
    }

    /// <summary>
    /// 获取百分比属性副词条分数
    /// </summary>
    /// <param name="appendId">id</param>
    /// <returns>分数</returns>
    public static float GetPercentSubAffixScore(in ReliquarySubAffixId appendId)
    {
        // 圣遗物相同类型副词条强化档位一共为 4/3/2 档
        // 五星 为 70% 80% 90% 100%
        // 四星 为 70% 80% 90% 100%
        // 三星 为 70% 80% 90% 100%
        // 二星 为 70%   85%   100%
        // 二星 为 80%         100%
        // 通过计算与最大属性的 Id 差来决定当前副词条的强化档位
        uint maxId = GetAffixMaxId(appendId);
        uint delta = maxId - appendId;

        return (maxId / 100000, delta) switch
        {
            (5 or 4 or 3, 0) => 100F,
            (5 or 4 or 3, 1) => 90F,
            (5 or 4 or 3, 2) => 80F,
            (5 or 4 or 3, 3) => 70F,

            (2, 0) => 100F,
            (2, 1) => 85F,
            (2, 2) => 70F,

            (1, 0) => 100F,
            (1, 1) => 80F,

            _ => throw Must.NeverHappen($"Unexpected AppendId: {appendId.Value} Delta: {delta}"),
        };
    }

    /// <summary>
    /// 获取双爆评分
    /// </summary>
    /// <param name="fightPropMap">属性</param>
    /// <returns>评分</returns>
    public static float ScoreCrit(Dictionary<FightProperty, float>? fightPropMap)
    {
        if (fightPropMap.IsNullOrEmpty())
        {
            return 0F;
        }

        float cr = fightPropMap[FightProperty.FIGHT_PROP_CRITICAL];
        float cd = fightPropMap[FightProperty.FIGHT_PROP_CRITICAL_HURT];

        return 100 * ((cr * 2) + cd);
    }
}