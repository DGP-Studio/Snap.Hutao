// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Intrinsic;

/// <summary>
/// 从属地区
/// </summary>
public enum AssociationType
{
    /// <summary>
    /// 无
    /// </summary>
    ASSOC_TYPE_NONE,

    /// <summary>
    /// 蒙德
    /// </summary>
    [Description("蒙德")]
    ASSOC_TYPE_MONDSTADT,

    /// <summary>
    /// 璃月
    /// </summary>
    [Description("璃月")]
    ASSOC_TYPE_LIYUE,

    /// <summary>
    /// 主角
    /// </summary>
    ASSOC_TYPE_MAINACTOR,

    /// <summary>
    /// 愚人众
    /// </summary>
    [Description("愚人众")]
    ASSOC_TYPE_FATUI,

    /// <summary>
    /// 稻妻
    /// </summary>
    [Description("稻妻")]
    ASSOC_TYPE_INAZUMA,

    /// <summary>
    /// 游侠
    /// </summary>
    [Description("游侠")]
    ASSOC_TYPE_RANGER,

    /// <summary>
    /// 须弥
    /// </summary>
    [Description("须弥")]
    ASSOC_TYPE_SUMERU,

    /// <summary>
    /// 枫丹
    /// </summary>
    [Description("枫丹")]
    ASSOC_TYPE_FONTAINE,

    /// <summary>
    /// 纳塔
    /// </summary>
    [Description("纳塔")]
    ASSOC_TYPE_NATLAN,

    /// <summary>
    /// 至冬
    /// </summary>
    [Description("至冬")]
    ASSOC_TYPE_SNEZHNAYA,
}