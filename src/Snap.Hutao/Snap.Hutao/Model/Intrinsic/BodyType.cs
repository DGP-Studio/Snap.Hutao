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
    [LocalizationKey("ModelIntrinsicBodyTypeBoy")]
    BODY_BOY,

    /// <summary>
    /// 女孩
    /// </summary>
    [LocalizationKey("ModelIntrinsicBodyTypeGirl")]
    BODY_GIRL,

    /// <summary>
    /// 成女
    /// </summary>
    [LocalizationKey("ModelIntrinsicBodyTypeLady")]
    BODY_LADY,

    /// <summary>
    /// 成男
    /// </summary>
    [LocalizationKey("ModelIntrinsicBodyTypeMale")]
    BODY_MALE,

    /// <summary>
    /// 萝莉
    /// </summary>
    [LocalizationKey("ModelIntrinsicBodyTypeLoli")]
    BODY_LOLI,
}