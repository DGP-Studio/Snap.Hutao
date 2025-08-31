// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Intrinsic;

[ExtendedEnum]
internal enum HardChallengeDifficultyLevel
{
    None = 0,

    [LocalizationKey(nameof(SH.ModelIntrinsicHardChallengeDifficultyLevelNormal))]
    Normal = 1,

    [LocalizationKey(nameof(SH.ModelIntrinsicHardChallengeDifficultyLevelAdvancing))]
    Advancing = 2,

    [LocalizationKey(nameof(SH.ModelIntrinsicHardChallengeDifficultyLevelHard))]
    Hard = 3,

    [LocalizationKey(nameof(SH.ModelIntrinsicHardChallengeDifficultyLevelMenacing))]
    Menacing = 4,

    [LocalizationKey(nameof(SH.ModelIntrinsicHardChallengeDifficultyLevelFearless))]
    Fearless = 5,

    [LocalizationKey(nameof(SH.ModelIntrinsicHardChallengeDifficultyLevelDire))]
    Dire = 6,
}