// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.HardChallenge;

[ExtendedEnum]
internal enum HardChallengeBestAvatarType
{
    None,

    /// <summary>
    /// UI_ACTIVITY_LEYLINEC_RECORD_ONEATTACK
    /// </summary>
    [LocalizationKey(nameof(SH.WebHardChallengeBestAvatarTypeOneAttack))]
    OneAttack = 1,

    /// <summary>
    /// UI_ACTIVITY_LEYLINEC_RECORD_ALLATTACK
    /// </summary>
    [LocalizationKey(nameof(SH.WebHardChallengeBestAvatarTypeAllAttack))]
    AllAttack = 2,
}