// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.AvatarProperty;
using Snap.Hutao.ViewModel.User;

namespace Snap.Hutao.Service.AvatarInfo;

/// <summary>
/// 角色信息服务
/// </summary>
[HighQuality]
internal interface IAvatarInfoService
{
    /// <summary>
    /// 异步获取总览数据
    /// </summary>
    /// <param name="userAndUid">uid</param>
    /// <param name="refreshOption">刷新选项</param>
    /// <param name="token">取消令牌</param>
    /// <returns>总览数据</returns>
    Task<ValueResult<RefreshResult, Summary?>> GetSummaryAsync(UserAndUid userAndUid, RefreshOption refreshOption, CancellationToken token = default);
}