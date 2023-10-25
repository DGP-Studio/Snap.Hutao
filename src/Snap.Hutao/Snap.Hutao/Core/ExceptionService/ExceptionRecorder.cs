// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.Core.ExceptionService;

/// <summary>
/// 异常记录器
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class ExceptionRecorder
{
    private readonly ILogger<ExceptionRecorder> logger;
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// 记录应用程序异常
    /// </summary>
    /// <param name="app">应用程序</param>
    public void Record(Application app)
    {
        app.UnhandledException += OnAppUnhandledException;
        app.DebugSettings.BindingFailed += OnXamlBindingFailed;
        app.DebugSettings.XamlResourceReferenceFailed += OnXamlResourceReferenceFailed;
    }

    [SuppressMessage("", "CA2012")]
    private void OnAppUnhandledException(object? sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        ValueTask<string?> task = serviceProvider
            .GetRequiredService<Web.Hutao.Log.HomaLogUploadClient>()
            .UploadLogAsync(e.Exception);

        if (!task.IsCompleted)
        {
            task.GetAwaiter().GetResult();
        }

        logger.LogError("未经处理的全局异常:\r\n{Detail}", ExceptionFormat.Format(e.Exception));
    }

    private void OnXamlBindingFailed(object? sender, BindingFailedEventArgs e)
    {
        logger.LogCritical("XAML 绑定失败:{Message}", e.Message);
    }

    private void OnXamlResourceReferenceFailed(DebugSettings sender, XamlResourceReferenceFailedEventArgs e)
    {
        logger.LogCritical("XAML 资源引用失败:{Message}", e.Message);
    }
}