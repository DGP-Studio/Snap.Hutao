// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Snap.Hutao.UI.Xaml.View.Window;
using Snap.Hutao.Win32;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace Snap.Hutao.Core.ExceptionService;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class ExceptionHandlingSupport
{
    private readonly ILogger<ExceptionHandlingSupport> logger;

    public static void Initialize(IServiceProvider serviceProvider, Application app)
    {
        serviceProvider.GetRequiredService<ExceptionHandlingSupport>().Attach(app);
    }

    [StackTraceHidden]
    public static Exception KillProcessOnDbException(Exception exception)
    {
        ExceptionDispatchInfo dispatch = ExceptionDispatchInfo.Capture(exception);

        if (dispatch.SourceException is DbException dbException)
        {
            throw KillProcessOnDbException(dbException);
        }

        if (dispatch.SourceException is DbUpdateException { InnerException: DbException dbEx })
        {
            throw KillProcessOnDbException(dbEx);
        }

        // In case it's not a DbException, we should preserve the original stack trace
        dispatch.Throw();

        // Should never be reached
        return dispatch.SourceException;
    }

    [StackTraceHidden]
    public static DbException KillProcessOnDbException(DbException exception)
    {
        HutaoNative.Instance.ShowErrorMessage("Warning | 警告", exception.Message);
        Process.GetCurrentProcess().Kill();
        return exception;
    }

    private static void OnAppUnhandledException(object? sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        Exception? exception = e.Exception;

        if (exception is null)
        {
            return;
        }

        Debugger.Break();
        XamlApplicationLifetime.Exiting = true;

        // https://github.com/getsentry/sentry-dotnet/blob/main/src/Sentry/Integrations/WinUIUnhandledExceptionIntegration.cs
        exception.SetSentryMechanism("Microsoft.UI.Xaml.UnhandledException", handled: false);

        SentryId id = SentrySdk.CaptureException(e.Exception);
        SentrySdk.Flush();

        // Handled has to be set to true, the control flow is returned after post
        e.Handled = true;

        if (XamlApplicationLifetime.Exited)
        {
            return;
        }

#pragma warning disable SH007
        SynchronizationContext.Current!.Post(static state => ExceptionWindow.Show((SentryId)state!), id);
#pragma warning restore SH007
    }

    private static void OnAppDomainFirstChanceException(object? sender, FirstChanceExceptionEventArgs e)
    {
        if (e.Exception is null)
        {
            return;
        }

        Exception exception = e.Exception;
        string type = TypeNameHelper.GetTypeDisplayName(exception);
        if (exception is OperationCanceledException)
        {
            return;
        }

        if (exception.TargetSite?.DeclaringType?.Assembly != typeof(App).Assembly)
        {
            return;
        }

        string a = exception.ToString();
        Debugger.Break();
    }

    private void Attach(Application app)
    {
#if DEBUG
        AppDomain.CurrentDomain.FirstChanceException += OnAppDomainFirstChanceException;
#endif

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

    private void OnXamlBindingFailed(object? sender, BindingFailedEventArgs e)
    {
        logger.LogCritical("XAML Binding Failed:{Message}", e.Message);
    }

    private void OnXamlResourceReferenceFailed(DebugSettings sender, XamlResourceReferenceFailedEventArgs e)
    {
        logger.LogCritical("XAML Resource Reference Failed:{Message}", e.Message);
    }
}