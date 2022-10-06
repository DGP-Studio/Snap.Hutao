// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Core.Logging;
using System.Diagnostics;
using System.IO;

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

    private void OnAppUnhandledException(object? sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        // string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        // string fileName = $"ex-{DateTimeOffset.Now:yyyyMMddHHmmssffff}.txt";
        // File.WriteAllText(Path.Combine(path, fileName), $"{e.Exception}\r\n{e.Exception.StackTrace}");
        logger.LogError(EventIds.UnhandledException, e.Exception, "未经处理的异常");

        foreach (ILoggerProvider provider in Ioc.Default.GetRequiredService<IEnumerable<ILoggerProvider>>())
        {
            provider.Dispose();
        }
    }

    private void OnXamlBindingFailed(object? sender, BindingFailedEventArgs e)
    {
        logger.LogCritical(EventIds.XamlBindingError, "XAML绑定失败: {message}", e.Message);
    }
}
