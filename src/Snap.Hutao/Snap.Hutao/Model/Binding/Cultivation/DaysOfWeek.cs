// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Binding.Cultivation;

/// <summary>
/// 游戏内星期中的天
/// </summary>
[HighQuality]
internal enum DaysOfWeek
{
    /// <summary>
    /// 任意
    /// </summary>
    Any,

    /// <summary>
    /// 周一/周四/周日
    /// </summary>
    [LocalizationKey("ModelBindingCultivationDaysOfWeek14")]
    MondayAndThursday,

    /// <summary>
    /// 周二/周五/周日
    /// </summary>
    [LocalizationKey("ModelBindingCultivationDaysOfWeek25")]
    TuesdayAndFriday,

    /// <summary>
    /// 周三/周六/周日
    /// </summary>
    [LocalizationKey("ModelBindingCultivationDaysOfWeek36")]
    WednesdayAndSaturday,
}