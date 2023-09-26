// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Intrinsic;

/// <summary>
/// 成就信息状态
/// </summary>
[HighQuality]
internal enum AchievementStatus
{
    /// <summary>
    /// 未识别
    /// </summary>
    STATUS_UNRECOGNIZED = -1,

    /// <summary>
    /// 不使用的成就
    /// </summary>
    STATUS_INVALID = 0,

    /// <summary>
    /// 未完成
    /// </summary>
    STATUS_UNFINISHED = 1,

    /// <summary>
    /// 已完成
    /// </summary>
    STATUS_FINISHED = 2,

    /// <summary>
    /// 奖励已领取
    /// </summary>
    STATUS_REWARD_TAKEN = 3,
}