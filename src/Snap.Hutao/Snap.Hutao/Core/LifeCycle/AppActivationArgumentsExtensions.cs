// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Primitives;
using Microsoft.Windows.AppLifecycle;
using Windows.ApplicationModel.Activation;

namespace Snap.Hutao.Core.LifeCycle;

/// <summary>
/// <see cref="AppActivationArguments"/> 扩展
/// </summary>
[HighQuality]
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

    public static bool TryGetLaunchActivatedArguments(this AppActivationArguments activatedEventArgs, [NotNullWhen(true)] out string? arguments, out bool isToastActivated)
    {
        arguments = null;
        isToastActivated = false;

        if (activatedEventArgs.Data is not ILaunchActivatedEventArgs launchArgs)
        {
            return false;
        }

        arguments = launchArgs.Arguments.Trim();
        foreach (StringSegment segment in new StringTokenizer(arguments, [' ']))
        {
            if (segment.AsSpan().SequenceEqual("----AppNotificationActivated:"))
            {
                isToastActivated = true;
                break;
            }
        }

        return true;
    }
}