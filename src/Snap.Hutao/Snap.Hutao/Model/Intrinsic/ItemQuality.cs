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
    QUALITY_WHITE = 1,

    /// <summary>
    /// 二星
    /// </summary>
    QUALITY_GREEN = 2,

    /// <summary>
    /// 三星
    /// </summary>
    QUALITY_BLUE = 3,

    /// <summary>
    /// 四星
    /// </summary>
    QUALITY_PURPLE = 4,

    /// <summary>
    /// 五星
    /// </summary>
    QUALITY_ORANGE = 5,

    /// <summary>
    /// 限定五星
    /// </summary>
    QUALITY_ORANGE_SP = 105,
}