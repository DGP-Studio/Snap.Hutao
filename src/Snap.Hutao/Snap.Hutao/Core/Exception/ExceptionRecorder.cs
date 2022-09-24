// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Logging;

namespace Snap.Hutao.Core.Exception;

/// <summary>
/// 异常记录器
/// </summary>
internal class ExceptionRecorder
{
    private readonly ILogger logger;

    /// <summary>
    /// 构造一个新的异常记录器
    /// </summary>
    /// <param name="application">应用程序</param>
    /// <param name="logger">日志器</param>
    public ExceptionRecorder(Application application, ILogger logger)
    {
        this.logger = logger;

        application.UnhandledException += OnAppUnhandledException;
        application.DebugSettings.BindingFailed += OnXamlBindingFailed;
    }

    /// <summary>
    /// 当应用程序未经处理的异常引发时调用
    /// </summary>
    /// <param name="sender">实例</param>
    /// <param name="e">事件参数</param>
    public void OnAppUnhandledException(object? sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        logger.LogError(EventIds.UnhandledException, e.Exception, "未经处理的异常: [HResult:{code}]", e.Exception.HResult);
    }

    /// <summary>
    /// Xaml 绑定失败时触发
    /// </summary>
    /// <param name="sender">实例</param>
    /// <param name="e">事件参数</param>
    public void OnXamlBindingFailed(object? sender, BindingFailedEventArgs e)
    {
        logger.LogCritical(EventIds.XamlBindingError, "XAML绑定失败: {message}", e.Message);
    }
}
