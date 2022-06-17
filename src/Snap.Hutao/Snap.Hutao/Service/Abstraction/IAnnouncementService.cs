// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;
using System.Windows.Input;

namespace Snap.Hutao.Service.Abstraction;

/// <summary>
/// 公告服务
/// </summary>
public interface IAnnouncementService
{
    /// <summary>
    /// 异步获取游戏公告与活动,通常会进行缓存
    /// </summary>
    /// <param name="openAnnouncementUICommand">打开公告时触发的命令</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>公告包装器</returns>
    ValueTask<AnnouncementWrapper> GetAnnouncementsAsync(ICommand openAnnouncementUICommand, CancellationToken cancellationToken = default);
}
