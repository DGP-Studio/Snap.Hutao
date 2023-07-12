// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Intrinsic;

/// <summary>
/// 体型
/// </summary>
[HighQuality]
[Localization]
internal enum BodyType
{
    /// <summary>
    /// 无
    /// </summary>
    BODY_NONE,

    /// <summary>
    /// 男孩
    /// </summary>
    [LocalizationKey(nameof(SH.ModelIntrinsicBodyTypeBoy))]
    BODY_BOY,

    /// <summary>
    /// 女孩
    /// </summary>
    [LocalizationKey(nameof(SH.ModelIntrinsicBodyTypeGirl))]
    BODY_GIRL,

    /// <summary>
    /// 成女
    /// </summary>
    [LocalizationKey(nameof(SH.ModelIntrinsicBodyTypeLady))]
    BODY_LADY,

    /// <summary>
    /// 成男
    /// </summary>
    [LocalizationKey(nameof(SH.ModelIntrinsicBodyTypeMale))]
    BODY_MALE,

    /// <summary>
    /// 萝莉
    /// </summary>
    [LocalizationKey(nameof(SH.ModelIntrinsicBodyTypeLoli))]
    BODY_LOLI,
}