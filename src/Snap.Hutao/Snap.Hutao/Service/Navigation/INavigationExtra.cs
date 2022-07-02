// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Navigation;

/// <summary>
/// 为 <see cref="NavigationExtra"/> 提供抽象接口
/// </summary>
public interface INavigationExtra
{
    /// <summary>
    /// 数据
    /// </summary>
    object? Data { get; set; }

    /// <summary>
    /// 通知导航服务导航已经结束
    /// </summary>
    void NotifyNavigationCompleted();

    /// <summary>
    /// 通知导航服务导航异常
    /// </summary>
    /// <param name="exception">异常</param>
    void NotifyNavigationException(Exception exception);
}