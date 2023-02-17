// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.Service.Abstraction;

/// <summary>
/// 消息条服务
/// </summary>
[HighQuality]
internal interface IInfoBarService
{
    /// <summary>
    /// 显示错误消息
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="delay">关闭延迟</param>
    void Error(string message, int delay = 60000);

    /// <summary>
    /// 显示错误消息
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="message">消息</param>
    /// <param name="delay">关闭延迟</param>
    void Error(string title, string message, int delay = 60000);

    /// <summary>
    /// 显示错误消息
    /// </summary>
    /// <param name="exception">异常</param>
    /// <param name="delay">关闭延迟</param>
    void Error(Exception exception, int delay = 60000);

    /// <summary>
    /// 显示错误消息
    /// </summary>
    /// <param name="exception">异常</param>
    /// <param name="title">标题</param>
    /// <param name="delay">关闭延迟</param>
    void Error(Exception exception, string title, int delay = 60000);

    /// <summary>
    /// 显示提示信息
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="delay">关闭延迟</param>
    void Information(string message, int delay = 5000);

    /// <summary>
    /// 显示提示信息
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="message">消息</param>
    /// <param name="delay">关闭延迟</param>
    void Information(string title, string message, int delay = 5000);

    /// <summary>
    /// 使用指定的 <see cref="StackPanel"/> 初始化服务
    /// </summary>
    /// <param name="container">信息条的目标容器</param>
    void Initialize(StackPanel container);

    /// <summary>
    /// 显示成功信息
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="delay">关闭延迟</param>
    void Success(string message, int delay = 5000);

    /// <summary>
    /// 显示成功信息
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="message">消息</param>
    /// <param name="delay">关闭延迟</param>
    void Success(string title, string message, int delay = 5000);

    /// <summary>
    /// 异步等待加载完成
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>任务</returns>
    Task WaitInitializationAsync(CancellationToken token = default);

    /// <summary>
    /// 显示警告信息
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="delay">关闭延迟</param>
    void Warning(string message, int delay = 30000);

    /// <summary>
    /// 显示警告信息
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="message">消息</param>
    /// <param name="delay">关闭延迟</param>
    void Warning(string title, string message, int delay = 30000);
}
