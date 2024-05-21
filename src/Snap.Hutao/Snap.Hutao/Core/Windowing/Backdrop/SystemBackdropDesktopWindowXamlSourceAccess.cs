// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media;
using System.Runtime.CompilerServices;
using WinRT;

namespace Snap.Hutao.Core.Windowing.Backdrop;

internal sealed class SystemBackdropDesktopWindowXamlSourceAccess : SystemBackdrop
{
    private readonly SystemBackdrop? innerBackdrop;

    public SystemBackdropDesktopWindowXamlSourceAccess(SystemBackdrop? systemBackdrop)
    {
        innerBackdrop = systemBackdrop;
    }

    public DesktopWindowXamlSource? DesktopWindowXamlSource
    {
        get; private set;
    }

    protected override void OnTargetConnected(ICompositionSupportsSystemBackdrop target, XamlRoot xamlRoot)
    {
        DesktopWindowXamlSource = DesktopWindowXamlSource.FromAbi(target.As<IInspectable>().ThisPtr);
        if (innerBackdrop is not null)
        {
            ProtectedOnTargetConnected(innerBackdrop, target, xamlRoot);
        }
    }

    protected override void OnTargetDisconnected(ICompositionSupportsSystemBackdrop target)
    {
        DesktopWindowXamlSource = null;
        if (innerBackdrop is not null)
        {
            ProtectedOnTargetDisconnected(innerBackdrop, target);
        }
    }

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = nameof(OnTargetConnected))]
    private static extern void ProtectedOnTargetConnected(SystemBackdrop systemBackdrop, ICompositionSupportsSystemBackdrop target, XamlRoot xamlRoot);

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = nameof(OnTargetDisconnected))]
    private static extern void ProtectedOnTargetDisconnected(SystemBackdrop systemBackdrop, ICompositionSupportsSystemBackdrop target);
}