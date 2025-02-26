// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;

namespace Snap.Hutao;

internal static class SentryTelemetry
{
    public static IDisposable Initialize()
    {
        IDisposable sentry = SentrySdk.Init(options =>
        {
            options.Dsn = "https://1a1151ce5ac4e7f1536edf085bd483ec@sentry.snapgenshin.com/2";
#if DEBUG
            options.Debug = true;
#endif
            options.AutoSessionTracking = true;
            options.IsGlobalModeEnabled = true;
            options.Release = HutaoRuntime.Version.ToString();
            options.Environment = GetBuildEnvironment();

            // Use our own exception handling
            options.DisableWinUiUnhandledExceptionIntegration();
        });

        SentrySdk.ConfigureScope(scope =>
        {
            scope.User = new SentryUser()
            {
                Id = HutaoRuntime.DeviceId,
            };
        });

        return sentry;
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