// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Logging;

internal static class LoggerFactoryExtension
{
    public static ILoggingBuilder AddConsoleWindow(this ILoggingBuilder builder)
    {
        builder.Services.AddSingleton<ConsoleWindowLifeTime>();

        builder.AddSimpleConsole(options =>
        {
            options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff ";
        });

        return builder;
    }

    public static ILoggingBuilder AddSentryTelemetry(this ILoggingBuilder builder)
    {
        return builder.AddSentry(options =>
        {
            // TODO: find a way to set correct HttpProxy
#if DEBUG || IS_ALPHA_BUILD || IS_CANARY_BUILD
            // Alpha and Canary produces noisy events
            options.Dsn = "https://ec3799184191c344ca06c592cb97a464@sentry.snapgenshin.com/4";
#else
            options.Dsn = "https://1a1151ce5ac4e7f1536edf085bd483ec@sentry.snapgenshin.com/2";
#endif

#if DEBUG
            options.Debug = true;
#endif
            options.AutoSessionTracking = true;
            options.IsGlobalModeEnabled = true;
            options.Release = HutaoRuntime.Version.ToString();
            options.Environment = GetBuildEnvironment();

            // Suppress logs to generate events and breadcrumbs
            options.MinimumBreadcrumbLevel = LogLevel.None;
            options.MinimumEventLevel = LogLevel.None;

            // Suppress HTTP tracing
            options.DisableSentryHttpMessageHandler = true;

            // Use our own exception handling
            options.DisableWinUiUnhandledExceptionIntegration();

            options.ConfigureScope(scope =>
            {
                scope.User = new()
                {
                    Id = HutaoRuntime.DeviceId,
                };

                scope.SetTag("elevated", HutaoRuntime.IsProcessElevated ? "yes" : "no");
            });

            options.SetBeforeSend((@event, hint) =>
            {
                Exception? currentException = @event.Exception;
                while (currentException is not null)
                {
                    currentException.Data.Add("HResult", $"0x{currentException.HResult:X8}");
                    currentException = currentException.InnerException;
                }

                return @event;
            });
        });
    }

    private static string GetBuildEnvironment()
    {
#if DEBUG
        return "DEBUG";
#elif IS_ALPHA_BUILD
        return "ALPHA";
#elif IS_CANARY_BUILD
        return "CANARY";
#else
        return "RELEASE";
#endif
    }
}