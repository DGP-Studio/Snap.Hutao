// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Navigation;

/// <summary>
/// 导航消息接收器
/// </summary>
public interface INavigationRecipient
{
    /// <summary>
    /// 异步接收消息
    /// </summary>
    /// <param name="data">导航数据</param>
    /// <returns>接收处理结果是否成功</returns>
    Task<bool> ReceiveAsync(INavigationData data);
}