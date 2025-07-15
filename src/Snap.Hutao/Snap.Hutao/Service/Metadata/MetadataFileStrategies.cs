// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Metadata;

internal static class MetadataFileStrategies
{
    public static readonly MetadataFileStrategy Achievement = new("Achievement");
    public static readonly MetadataFileStrategy AchievementGoal = new("AchievementGoal");
    public static readonly MetadataFileStrategy Avatar = new("Avatar", true);
    public static readonly MetadataFileStrategy AvatarCurve = new("AvatarCurve");
    public static readonly MetadataFileStrategy AvatarPromote = new("AvatarPromote");
    public static readonly MetadataFileStrategy Chapter = new("Chapter");
    public static readonly MetadataFileStrategy Combine = new("Combine");
    public static readonly MetadataFileStrategy DisplayItem = new("DisplayItem");
    public static readonly MetadataFileStrategy GachaEvent = new("GachaEvent");
    public static readonly MetadataFileStrategy HardChallengeSchedule = new("HardChallengeSchedule");
    public static readonly MetadataFileStrategy HyperLinkName = new("HyperLinkName");
    public static readonly MetadataFileStrategy Material = new("Material");
    public static readonly MetadataFileStrategy Monster = new("Monster");
    public static readonly MetadataFileStrategy MonsterCurve = new("MonsterCurve");
    public static readonly MetadataFileStrategy ProfilePicture = new("ProfilePicture");
    public static readonly MetadataFileStrategy Reliquary = new("Reliquary");
    public static readonly MetadataFileStrategy ReliquaryMainAffix = new("ReliquaryMainAffix");
    public static readonly MetadataFileStrategy ReliquaryMainAffixLevel = new("ReliquaryMainAffixLevel");
    public static readonly MetadataFileStrategy ReliquarySet = new("ReliquarySet");
    public static readonly MetadataFileStrategy ReliquarySubAffix = new("ReliquarySubAffix");
    public static readonly MetadataFileStrategy RoleCombatSchedule = new("RoleCombatSchedule");
    public static readonly MetadataFileStrategy TowerFloor = new("TowerFloor");
    public static readonly MetadataFileStrategy TowerLevel = new("TowerLevel");
    public static readonly MetadataFileStrategy TowerSchedule = new("TowerSchedule");
    public static readonly MetadataFileStrategy Weapon = new("Weapon");
    public static readonly MetadataFileStrategy WeaponCurve = new("WeaponCurve");
    public static readonly MetadataFileStrategy WeaponPromote = new("WeaponPromote");
}