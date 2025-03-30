// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.AvatarProperty;
using System.Collections.Frozen;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.AvatarInfo.Factory.Builder;

internal static class AvatarViewBuilderSkillExtension
{
    public static TBuilder SetSkills<TBuilder>(this TBuilder builder, ImmutableArray<ProudSkill> proudSkills, FrozenDictionary<SkillId, SkillLevel> skillLevels, FrozenDictionary<SkillId, SkillLevel> extraLevels)
        where TBuilder : class, IAvatarViewBuilder
    {
        return builder.SetSkills(CreateSkills(proudSkills, skillLevels, extraLevels));
    }

    public static TBuilder SetSkills<TBuilder>(this TBuilder builder, ImmutableArray<SkillView> skills)
        where TBuilder : class, IAvatarViewBuilder
    {
        return builder.Configure(b => b.View.Skills = skills);
    }

    private static ImmutableArray<SkillView> CreateSkills(ImmutableArray<ProudSkill> proudSkills, FrozenDictionary<SkillId, SkillLevel> skillLevels, FrozenDictionary<SkillId, SkillLevel> extraLevels)
    {
        if (skillLevels is { Count: 0 })
        {
            return [];
        }

        SkillState state = new(skillLevels, extraLevels);

        return proudSkills.SelectAsArray(
            static (proudSkill, state) => new SkillViewBuilder()
                .SetName(proudSkill.Name)
                .SetIcon(SkillIconConverter.IconNameToUri(proudSkill.Icon))
                .SetDescription(proudSkill.Description)
                .SetGroupId(proudSkill.GroupId)
                .SetLevel(LevelFormat.Format(state.NonExtraLeveledSkills[proudSkill.Id], state.ExtraLevels.GetValueOrDefault(proudSkill.Id)))
                .SetLevelNumber(state.NonExtraLeveledSkills[proudSkill.Id])
                .SetInfo(DescriptionsParametersDescriptor.Convert(proudSkill.Proud, state.SkillLevels[proudSkill.Id]))
                .View,
            state);
    }

    private sealed class SkillState
    {
        public SkillState(FrozenDictionary<SkillId, SkillLevel> skillLevels, FrozenDictionary<SkillId, SkillLevel> extraLevels)
        {
            SkillLevels = skillLevels;
            ExtraLevels = extraLevels;

            // Parameters has to be IDictionary<,> to avoid using IEnumerable<,> in the constructor
            Dictionary<SkillId, SkillLevel> nonExtraLeveledSkills = new(skillLevels);
            foreach ((SkillId skillId, SkillLevel extraLevel) in extraLevels)
            {
                nonExtraLeveledSkills.DecreaseByValue(skillId, extraLevel);
            }

            NonExtraLeveledSkills = nonExtraLeveledSkills;
        }

        public IReadOnlyDictionary<SkillId, SkillLevel> SkillLevels { get; }

        public IReadOnlyDictionary<SkillId, SkillLevel> ExtraLevels { get; }

        public IReadOnlyDictionary<SkillId, SkillLevel> NonExtraLeveledSkills { get; }
    }
}