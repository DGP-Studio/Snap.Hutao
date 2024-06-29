// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using System.Diagnostics;

namespace Snap.Hutao.Core.ExceptionService;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class ExceptionRecorder
{
    private readonly ILogger<ExceptionRecorder> logger;
    private readonly IServiceProvider serviceProvider;

    public void Record(Application app)
    {
        app.UnhandledException += OnAppUnhandledException;
        ConfigureDebugSettings(app);
    }

    [Conditional("DEBUG")]
    private void ConfigureDebugSettings(Application app)
    {
        app.DebugSettings.FailFastOnErrors = false;

        app.DebugSettings.IsBindingTracingEnabled = true;
        app.DebugSettings.BindingFailed += OnXamlBindingFailed;

        app.DebugSettings.IsXamlResourceReferenceTracingEnabled = true;
        app.DebugSettings.XamlResourceReferenceFailed += OnXamlResourceReferenceFailed;

        app.DebugSettings.LayoutCycleTracingLevel = LayoutCycleTracingLevel.High;
        app.DebugSettings.LayoutCycleDebugBreakLevel = LayoutCycleDebugBreakLevel.High;
    }

    private void OnAppUnhandledException(object? sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        logger.LogError("未经处理的全局异常:\r\n{Detail}", ExceptionFormat.Format(e.Exception));

        serviceProvider
            .GetRequiredService<Web.Hutao.Log.HutaoLogUploadClient>()
            .UploadLog(e.Exception);
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