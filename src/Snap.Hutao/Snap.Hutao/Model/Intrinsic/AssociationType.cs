// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Intrinsic;

/// <summary>
/// 从属地区
/// </summary>
[HighQuality]
[Localization]
internal enum AssociationType
{
    /// <summary>
    /// 无
    /// </summary>
    ASSOC_TYPE_NONE,

    /// <summary>
    /// 蒙德
    /// </summary>
    [LocalizationKey("ModelIntrinsicAssociationTypeMondstadt")]
    ASSOC_TYPE_MONDSTADT,

    /// <summary>
    /// 璃月
    /// </summary>
    [LocalizationKey("ModelIntrinsicAssociationTypeLiyue")]
    ASSOC_TYPE_LIYUE,

    /// <summary>
    /// 主角
    /// </summary>
    ASSOC_TYPE_MAINACTOR,

    /// <summary>
    /// 愚人众
    /// </summary>
    [LocalizationKey("ModelIntrinsicAssociationTypeFatui")]
    ASSOC_TYPE_FATUI,

    /// <summary>
    /// 稻妻
    /// </summary>
    [LocalizationKey("ModelIntrinsicAssociationTypeInazuma")]
    ASSOC_TYPE_INAZUMA,

    /// <summary>
    /// 游侠
    /// </summary>
    [LocalizationKey("ModelIntrinsicAssociationTypeRanger")]
    ASSOC_TYPE_RANGER,

    /// <summary>
    /// 须弥
    /// </summary>
    [LocalizationKey("ModelIntrinsicAssociationTypeSemuru")]
    ASSOC_TYPE_SUMERU,

    /// <summary>
    /// 枫丹
    /// </summary>
    [LocalizationKey("ModelIntrinsicAssociationTypeFontaine")]
    ASSOC_TYPE_FONTAINE,

    /// <summary>
    /// 纳塔
    /// </summary>
    [LocalizationKey("ModelIntrinsicAssociationTypeNatlan")]
    ASSOC_TYPE_NATLAN,

    /// <summary>
    /// 至冬
    /// </summary>
    [LocalizationKey("ModelIntrinsicAssociationTypeSnezhnaya")]
    ASSOC_TYPE_SNEZHNAYA,
}