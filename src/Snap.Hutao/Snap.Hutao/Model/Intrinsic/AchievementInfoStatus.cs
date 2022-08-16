// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Intrinsic;

/// <summary>
/// 成就信息状态
/// https://github.com/Grasscutters/Grasscutter/blob/development/proto/AchievementInfo.proto
/// </summary>
public enum AchievementInfoStatus
{
    /// <summary>
    /// 非法值
    /// </summary>
    ACHIEVEMENT_INVALID = 0,

    /// <summary>
    /// 未完成
    /// </summary>
    ACHIEVEMENT_UNFINISHED = 1,

    /// <summary>
    /// 已完成
    /// </summary>
    ACHIEVEMENT_FINISHED = 2,

    /// <summary>
    /// 奖励已领取
    /// </summary>
    ACHIEVEMENT_POINT_TAKEN = 3,
}