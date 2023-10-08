// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Notification;

/// <summary>
/// 消息条服务
/// </summary>
[HighQuality]
internal interface IInfoBarService
{
    /// <summary>
    /// 获取操作的集合
    /// </summary>
    ObservableCollection<InfoBar> Collection { get; }

    /// <summary>
    /// 显示错误消息
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="delay">关闭延迟</param>
    void Error(string message, int delay = 30000);

    /// <summary>
    /// 显示错误消息
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="message">消息</param>
    /// <param name="delay">关闭延迟</param>
    void Error(string title, string message, int delay = 30000);

    /// <summary>
    /// 显示错误消息
    /// </summary>
    /// <param name="exception">异常</param>
    /// <param name="delay">关闭延迟</param>
    void Error(Exception exception, int delay = 30000);

    /// <summary>
    /// 显示错误消息
    /// </summary>
    /// <param name="exception">异常</param>
    /// <param name="title">标题</param>
    /// <param name="delay">关闭延迟</param>
    void Error(Exception exception, string title, int delay = 30000);

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
    /// 显示警告信息
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="delay">关闭延迟</param>
    void Warning(string message, int delay = 15000);

    /// <summary>
    /// 显示警告信息
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="message">消息</param>
    /// <param name="delay">关闭延迟</param>
    void Warning(string title, string message, int delay = 15000);
}
