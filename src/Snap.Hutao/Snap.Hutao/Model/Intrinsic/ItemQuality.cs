// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Intrinsic;

/// <summary>
/// 稀有度
/// </summary>
[HighQuality]
[Localization]
internal enum ItemQuality
{
    /// <summary>
    /// 无
    /// </summary>
    QUALITY_NONE = 0,

    /// <summary>
    /// 一星
    /// </summary>
    [LocalizationKey("ModelIntrinsicItemQualityWhite")]
    QUALITY_WHITE = 1,

    /// <summary>
    /// 二星
    /// </summary>
    [LocalizationKey("ModelIntrinsicItemQualityGreen")]
    QUALITY_GREEN = 2,

    /// <summary>
    /// 三星
    /// </summary>
    [LocalizationKey("ModelIntrinsicItemQualityBlue")]
    QUALITY_BLUE = 3,

    /// <summary>
    /// 四星
    /// </summary>
    [LocalizationKey("ModelIntrinsicItemQualityPurple")]
    QUALITY_PURPLE = 4,

    /// <summary>
    /// 五星
    /// </summary>
    [LocalizationKey("ModelIntrinsicItemQualityOrange")]
    QUALITY_ORANGE = 5,

    /// <summary>
    /// 限定五星
    /// </summary>
    [LocalizationKey("ModelIntrinsicItemQualityRed")]
    QUALITY_ORANGE_SP = 105,
}