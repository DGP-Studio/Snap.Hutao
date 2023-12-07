// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

namespace Snap.Hutao.Service.Cultivation;

internal sealed class LevelInformation : IMappingFrom<LevelInformation, AvatarPromotionDelta>
{
    public uint AvatarLevelFrom { get; private set; }

    public uint AvatarLevelTo { get; private set; }

    public uint SkillALevelFrom { get; private set; }

    public uint SkillALevelTo { get; private set; }

    public uint SkillELevelFrom { get; private set; }

    public uint SkillELevelTo { get; private set; }

    public uint SkillQLevelFrom { get; private set; }

    public uint SkillQLevelTo { get; private set; }

    public uint WeaponLevelFrom { get; private set; }

    public uint WeaponLevelTo { get; private set; }

    public static LevelInformation From(AvatarPromotionDelta delta)
    {
        LevelInformation levelInformation = new();

        if (delta.AvatarId != 0U)
        {
            levelInformation.AvatarLevelFrom = delta.AvatarLevelCurrent;
            levelInformation.AvatarLevelTo = delta.AvatarLevelTarget;
        }

        if (delta.SkillList is [PromotionDelta skillA, PromotionDelta skillE, PromotionDelta skillQ, ..])
        {
            levelInformation.SkillALevelFrom = skillA.LevelCurrent;
            levelInformation.SkillALevelTo = skillA.LevelTarget;
            levelInformation.SkillELevelFrom = skillE.LevelCurrent;
            levelInformation.SkillELevelTo = skillE.LevelTarget;
            levelInformation.SkillQLevelFrom = skillQ.LevelCurrent;
            levelInformation.SkillQLevelTo = skillQ.LevelTarget;
        }

        if (delta.Weapon is { } weapon)
        {
            levelInformation.WeaponLevelFrom = weapon.LevelCurrent;
            levelInformation.WeaponLevelTo = weapon.LevelTarget;
        }

        return levelInformation;
    }
}