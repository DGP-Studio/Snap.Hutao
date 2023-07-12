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
    [LocalizationKey(nameof(SH.ModelIntrinsicAssociationTypeMondstadt))]
    ASSOC_TYPE_MONDSTADT,

    /// <summary>
    /// 璃月
    /// </summary>
    [LocalizationKey(nameof(SH.ModelIntrinsicAssociationTypeLiyue))]
    ASSOC_TYPE_LIYUE,

    /// <summary>
    /// 主角
    /// </summary>
    ASSOC_TYPE_MAINACTOR,

    /// <summary>
    /// 愚人众
    /// </summary>
    [LocalizationKey(nameof(SH.ModelIntrinsicAssociationTypeFatui))]
    ASSOC_TYPE_FATUI,

    /// <summary>
    /// 稻妻
    /// </summary>
    [LocalizationKey(nameof(SH.ModelIntrinsicAssociationTypeInazuma))]
    ASSOC_TYPE_INAZUMA,

    /// <summary>
    /// 游侠
    /// </summary>
    [LocalizationKey(nameof(SH.ModelIntrinsicAssociationTypeRanger))]
    ASSOC_TYPE_RANGER,

    /// <summary>
    /// 须弥
    /// </summary>
    [LocalizationKey(nameof(SH.ModelIntrinsicAssociationTypeSumeru))]
    ASSOC_TYPE_SUMERU,

    /// <summary>
    /// 枫丹
    /// </summary>
    [LocalizationKey(nameof(SH.ModelIntrinsicAssociationTypeFontaine))]
    ASSOC_TYPE_FONTAINE,

    /// <summary>
    /// 纳塔
    /// </summary>
    [LocalizationKey(nameof(SH.ModelIntrinsicAssociationTypeNatlan))]
    ASSOC_TYPE_NATLAN,

    /// <summary>
    /// 至冬
    /// </summary>
    [LocalizationKey(nameof(SH.ModelIntrinsicAssociationTypeSnezhnaya))]
    ASSOC_TYPE_SNEZHNAYA,
}