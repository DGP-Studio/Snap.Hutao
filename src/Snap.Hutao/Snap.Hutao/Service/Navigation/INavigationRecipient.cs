// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Navigation;

/// <summary>
/// 导航消息接收器
/// 用于通知异步导航完成
/// </summary>
[HighQuality]
internal interface INavigationRecipient
{
    /// <summary>
    /// 异步接收导航消息
    /// 在此方法结束后才会通知导航服务导航完成
    /// </summary>
    /// <param name="data">导航数据</param>
    /// <returns>接收处理结果是否成功</returns>
    ValueTask<bool> ReceiveAsync(INavigationData data);
}