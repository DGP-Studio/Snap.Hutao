// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO.Http.Proxy;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.Logging;

internal static class LoggerFactoryExtension
{
    private static Version? WindowsVersion
    {
        get
        {
            if (field is null)
            {
                using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
                {
                    if (key is not null)
                    {
                        object? major = key.GetValue("CurrentMajorVersionNumber");
                        object? minor = key.GetValue("CurrentMinorVersionNumber");
                        object? build = key.GetValue("CurrentBuildNumber");
                        object? revision = key.GetValue("UBR");
                        field = new($"{major}", $"{minor}", $"{build}", $"{revision}");
                    }
                    else
                    {
                        field = new Version("0", "0", "0", "0");
                    }
                }
            }

            return field;
        }
    }

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
            options.HttpProxy = HttpProxyUsingSystemProxy.Instance;

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
            options.Release = $"{HutaoRuntime.Version}";
            options.Environment = GetBuildEnvironment();

            // Suppress logs to generate events and breadcrumbs
            options.MinimumBreadcrumbLevel = LogLevel.None;
            options.MinimumEventLevel = LogLevel.None;

            options.ProfilesSampleRate = 1.0D;
            options.TracesSampleRate = 1.0D;

            // Use our own exception handling
            options.DisableWinUiUnhandledExceptionIntegration();

            options.ConfigureScope(scope =>
            {
                scope.User = new()
                {
                    Id = HutaoRuntime.DeviceId,
                };

                scope.SetTag("elevated", HutaoRuntime.IsProcessElevated ? "yes" : "no");
                scope.SetWebView2Version();
            });

            options.AddExceptionProcessor(new SentryExceptionProcessor());

            options.SetBeforeSend(@event =>
            {
                if (@event.Exception is { } exception)
                {
                    if (ExceptionFingerprint.TryGetFingerprint(exception, out string? fingerprint))
                    {
                        @event.SetFingerprint("{{ default }}", fingerprint);
                    }
                }

                Sentry.Protocol.OperatingSystem operatingSystem = @event.Contexts.OperatingSystem;
                operatingSystem.Build = WindowsVersion?.Build;
                operatingSystem.Name = "Windows";
                operatingSystem.Version = WindowsVersion?.ToString();

                return @event;
            });
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SetWebView2Version(this Scope scope)
    {
        WebView2Version webView2Version = HutaoRuntime.WebView2Version;
        Dictionary<string, object> webView2 = new()
        {
            ["Supported"] = webView2Version.Supported,
        };

        if (webView2Version.Supported)
        {
            webView2["Version"] = webView2Version.Version;
        }

        scope.Contexts["WebView2"] = webView2;
    }

    private sealed class Version
    {
        public Version(string? major, string? minor, string? build, string? revision)
        {
            Major = major;
            Minor = minor;
            Build = build;
            Revision = revision;
        }

        public string? Major { get; }

        public string? Minor { get; }

        public string? Build { get; }

        public string? Revision { get; }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Build}.{Revision}";
        }
    }
}