// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Intrinsic;

[ExtendedEnum]
internal enum QualityType
{
    /// <summary>
    /// 无
    /// </summary>
    QUALITY_NONE = 0,

    [LocalizationKey(nameof(SH.ModelIntrinsicItemQualityWhite))]
    QUALITY_WHITE = 1,

    [LocalizationKey(nameof(SH.ModelIntrinsicItemQualityGreen))]
    QUALITY_GREEN = 2,

    [LocalizationKey(nameof(SH.ModelIntrinsicItemQualityBlue))]
    QUALITY_BLUE = 3,

    [LocalizationKey(nameof(SH.ModelIntrinsicItemQualityPurple))]
    QUALITY_PURPLE = 4,

    [LocalizationKey(nameof(SH.ModelIntrinsicItemQualityOrange))]
    QUALITY_ORANGE = 5,

    [LocalizationKey(nameof(SH.ModelIntrinsicItemQualityRed))]
    QUALITY_ORANGE_SP = 105,
}