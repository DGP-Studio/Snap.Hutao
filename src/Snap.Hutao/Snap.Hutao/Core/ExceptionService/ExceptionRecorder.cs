// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using System.Collections;
using System.Text;

namespace Snap.Hutao.Core.ExceptionService;

/// <summary>
/// 异常记录器
/// </summary>
[HighQuality]
internal sealed class ExceptionRecorder
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
#if RELEASE
#pragma warning disable VSTHRD002
        Ioc.Default.GetRequiredService<Web.Hutao.HomaLogUploadClient>().UploadLogAsync(e.Exception).GetAwaiter().GetResult();
#pragma warning restore VSTHRD002
#endif
        StringBuilder dataDetailBuilder = new();
        foreach (DictionaryEntry entry in e.Exception.Data)
        {
            string key = $"{entry.Key}";
            string value = $"{entry.Value}";

            dataDetailBuilder.Append(key).Append(':').Append(value).Append("\r\n");
        }

        logger.LogError(e.Exception, "未经处理的异常\r\n{detail}", dataDetailBuilder.ToString());
    }

    private void OnXamlBindingFailed(object? sender, BindingFailedEventArgs e)
    {
        logger.LogCritical("XAML绑定失败: {message}", e.Message);
    }
}