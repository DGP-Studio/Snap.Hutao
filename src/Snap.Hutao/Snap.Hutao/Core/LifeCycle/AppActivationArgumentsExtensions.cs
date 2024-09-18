// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;
using Windows.ApplicationModel.Activation;

namespace Snap.Hutao.Core.LifeCycle;

internal static class AppActivationArgumentsExtensions
{
    public static bool TryGetProtocolActivatedUri(this AppActivationArguments activatedEventArgs, [NotNullWhen(true)] out Uri? uri)
    {
        uri = null;
        if (activatedEventArgs.Data is not IProtocolActivatedEventArgs protocolArgs)
        {
            return false;
        }

        uri = protocolArgs.Uri;
        return true;
    }

    public static bool TryGetLaunchActivatedArguments(this AppActivationArguments activatedEventArgs, [NotNullWhen(true)] out string? arguments)
    {
        arguments = null;

        if (activatedEventArgs.Data is not ILaunchActivatedEventArgs launchArgs)
        {
            return false;
        }

        arguments = launchArgs.Arguments.Trim();
        return true;
    }

    public static bool TryGetAppNotificationActivatedArguments(this AppActivationArguments activatedEventArgs, out string? argument, [NotNullWhen(true)] out IDictionary<string, string>? arguments, [NotNullWhen(true)] out IDictionary<string, string>? userInput)
    {
        argument = null;
        arguments = null;
        userInput = null;

        if (activatedEventArgs.Data is not AppNotificationActivatedEventArgs appNotificationArgs)
        {
            return false;
        }

        argument = appNotificationArgs.Argument;
        arguments = appNotificationArgs.Arguments;
        userInput = appNotificationArgs.UserInput;
        return true;
    }
}