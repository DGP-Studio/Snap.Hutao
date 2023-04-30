// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.Core.ExceptionService;

/// <summary>
/// 异常记录器
/// </summary>
[HighQuality]
[Injection(InjectAs.Singleton)]
internal sealed class ExceptionRecorder
{
    private readonly ILogger<ExceptionRecorder> logger;
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// 构造一个新的异常记录器
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="logger">日志器</param>
    public ExceptionRecorder(IServiceProvider serviceProvider)
    {
        logger = serviceProvider.GetRequiredService<ILogger<ExceptionRecorder>>();
        this.serviceProvider = serviceProvider;
    }

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

    private void OnAppUnhandledException(object? sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
#if RELEASE
#pragma warning disable VSTHRD002
        serviceProvider
            .GetRequiredService<Web.Hutao.HomaLogUploadClient>()
            .UploadLogAsync(serviceProvider, e.Exception)
            .GetAwaiter()
            .GetResult();
#pragma warning restore VSTHRD002
#endif

        logger.LogError("未经处理的全局异常:\r\n{detail}", ExceptionFormat.Format(e.Exception));
    }

    private void OnXamlBindingFailed(object? sender, BindingFailedEventArgs e)
    {
        logger.LogCritical("XAML 绑定失败:{message}", e.Message);
    }

    private void OnXamlResourceReferenceFailed(DebugSettings sender, XamlResourceReferenceFailedEventArgs e)
    {
        logger.LogCritical("XAML 资源引用失败:{message}", e.Message);
    }
}