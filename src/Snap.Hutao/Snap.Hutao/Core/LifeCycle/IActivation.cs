// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Windows.AppLifecycle;

namespace Snap.Hutao.Core.LifeCycle;

/// <summary>
/// 激活
/// </summary>
internal interface IActivation
{
    /// <summary>
    /// 响应激活事件
    /// 激活事件一般不会在UI线程上触发
    /// </summary>
    /// <param name="sender">发送方</param>
    /// <param name="args">激活参数</param>
    void Activate(object? sender, AppActivationArguments args);

    /// <summary>
    /// 使用当前 App 实例初始化激活
    /// </summary>
    /// <param name="appInstance">App 实例</param>
    void InitializeWith(AppInstance appInstance);

    /// <summary>
    /// 无转发触发激活事件
    /// </summary>
    /// <param name="sender">发送方</param>
    /// <param name="args">激活参数</param>
    void NonRedirectToActivate(object? sender, AppActivationArguments args);
}