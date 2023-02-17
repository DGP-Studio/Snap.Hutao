// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.AvatarInfo;

/// <summary>
/// 刷新选项
/// </summary>
[HighQuality]
internal enum RefreshOption
{
    /// <summary>
    /// 直接从数据库读取
    /// </summary>
    None,

    /// <summary>
    /// 从 Enka API 获取
    /// </summary>
    RequestFromEnkaAPI,

    /// <summary>
    /// 从 米游社 我的角色 获取
    /// </summary>
    RequestFromHoyolabGameRecord,

    /// <summary>
    /// 从 米游社 养成计算 获取
    /// </summary>
    RequestFromHoyolabCalculate,
}