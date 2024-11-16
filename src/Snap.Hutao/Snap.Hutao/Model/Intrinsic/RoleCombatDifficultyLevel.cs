// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Intrinsic;

[Localization]
internal enum RoleCombatDifficultyLevel
{
    None = 0,

    [LocalizationKey(nameof(SH.ModelIntrinsicRoleCombatDifficultyLevelEasy))]
    Easy = 1,

    [LocalizationKey(nameof(SH.ModelIntrinsicRoleCombatDifficultyLevelNormal))]
    Normal = 2,

    [LocalizationKey(nameof(SH.ModelIntrinsicRoleCombatDifficultyLevelHard))]
    Hard = 3,

    [LocalizationKey(nameof(SH.ModelIntrinsicRoleCombatDifficultyLevelVisionary))]
    Visionary = 4,
}