// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Threading;
using Snap.Hutao.Model.Binding.AvatarProperty;
using Snap.Hutao.Web.Hoyolab;

namespace Snap.Hutao.Service.AvatarInfo;

/// <summary>
/// 角色信息服务
/// </summary>
internal interface IAvatarInfoService
{
    /// <summary>
    /// 异步获取总览数据
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="refreshOption">刷新选项</param>
    /// <param name="token">取消令牌</param>
    /// <returns>总览数据</returns>
    Task<ValueResult<RefreshResult, Summary?>> GetSummaryAsync(PlayerUid uid, RefreshOption refreshOption, CancellationToken token = default);
}