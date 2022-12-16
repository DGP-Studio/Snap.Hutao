// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Intrinsic;

/// <summary>
/// 稀有度
/// https://github.com/Grasscutters/Grasscutter/blob/development/src/main/java/emu/grasscutter/game/inventory/ItemQuality.java
/// <seealso cref=""/>
/// </summary>
public enum ItemQuality
{
    /// <summary>
    /// 无
    /// </summary>
    QUALITY_NONE = 0,

    /// <summary>
    /// 一星
    /// </summary>
    [Description("一星")]
    QUALITY_WHITE = 1,

    /// <summary>
    /// 二星
    /// </summary>
    [Description("二星")]
    QUALITY_GREEN = 2,

    /// <summary>
    /// 三星
    /// </summary>
    [Description("三星")]
    QUALITY_BLUE = 3,

    /// <summary>
    /// 四星
    /// </summary>
    [Description("四星")]
    QUALITY_PURPLE = 4,

    /// <summary>
    /// 五星
    /// </summary>
    [Description("五星")]
    QUALITY_ORANGE = 5,

    /// <summary>
    /// 限定五星
    /// </summary>
    [Description("限定五星")]
    QUALITY_ORANGE_SP = 105,
}