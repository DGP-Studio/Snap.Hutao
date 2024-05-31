// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Primitives;
using Microsoft.Windows.AppLifecycle;

namespace Snap.Hutao.Core.LifeCycle;

internal sealed class HutaoActivationArguments
{
    public bool IsElevated { get; set; }

    public bool IsRedirectTo { get; set; }

    public bool IsToastActivated { get; set; }

    public HutaoActivationKind Kind { get; set; }

    public Uri? ProtocolActivatedUri { get; set; }

    public string? LaunchActivatedArguments { get; set; }

    public static HutaoActivationArguments FromAppActivationArguments(AppActivationArguments args, bool isRedirected = false, bool isElevated = false)
    {
        HutaoActivationArguments result = new()
        {
            IsElevated = isElevated,
            IsRedirectTo = isRedirected,
        };

        switch (args.Kind)
        {
            case ExtendedActivationKind.Launch:
                {
                    result.Kind = HutaoActivationKind.Launch;
                    if (args.TryGetLaunchActivatedArguments(out string? arguments))
                    {
                        result.LaunchActivatedArguments = arguments;

                        foreach (StringSegment segment in new StringTokenizer(arguments, [' ']))
                        {
                            if (segment.AsSpan().SequenceEqual("-ToastActivated"))
                            {
                                result.Kind = HutaoActivationKind.Toast;
                                break;
                            }
                        }
                    }

                    break;
                }

            case ExtendedActivationKind.Protocol:
                {
                    result.Kind = HutaoActivationKind.Protocol;
                    if (args.TryGetProtocolActivatedUri(out Uri? uri))
                    {
                        result.ProtocolActivatedUri = uri;
                    }

                    break;
                }
        }

        return result;
    }
}