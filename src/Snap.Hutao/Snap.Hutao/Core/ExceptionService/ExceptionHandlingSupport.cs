// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.UI.Xaml.View.Window;
using System.Diagnostics;

namespace Snap.Hutao.Core.ExceptionService;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class ExceptionHandlingSupport
{
    private readonly ILogger<ExceptionHandlingSupport> logger;
    private readonly IServiceProvider serviceProvider;

    public static void Initialize(IServiceProvider serviceProvider, Application app)
    {
        serviceProvider.GetRequiredService<ExceptionHandlingSupport>().Attach(app);
    }

    private void Attach(Application app)
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
        string message = ExceptionFormat.Format(e.Exception);
        logger.LogError("Unhandled Exception:\r\n{Detail}", message);

        XamlApplicationLifetime.Exiting = true;

        if (e.Exception is null)
        {
            return;
        }

        // https://github.com/getsentry/sentry-dotnet/blob/main/src/Sentry/Integrations/WinUIUnhandledExceptionIntegration.cs
        e.Exception.SetSentryMechanism("Microsoft.UI.Xaml.UnhandledException", handled: false);
        e.Handled = true;

        SentrySdk.CaptureException(e.Exception);

        serviceProvider
            .GetRequiredService<ITaskContext>()
            .BeginInvokeOnMainThread(() => ExceptionWindow.Show(message));
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