// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media;
using System.Runtime.CompilerServices;
using WinRT;

namespace Snap.Hutao.UI.Xaml.Media.Backdrop;

internal sealed partial class SystemBackdropDesktopWindowXamlSourceAccess : SystemBackdrop
{
    public SystemBackdropDesktopWindowXamlSourceAccess(SystemBackdrop? systemBackdrop)
    {
        InnerBackdrop = systemBackdrop;
    }

    public DesktopWindowXamlSource? DesktopWindowXamlSource
    {
        get; private set;
    }

    public SystemBackdrop? InnerBackdrop { get; }

    protected override void OnTargetConnected(ICompositionSupportsSystemBackdrop target, XamlRoot xamlRoot)
    {
        DesktopWindowXamlSource = DesktopWindowXamlSource.FromAbi(target.As<IInspectable>().ThisPtr);
        if (InnerBackdrop is not null)
        {
            ProtectedOnTargetConnected(InnerBackdrop, target, xamlRoot);
        }
    }

    protected override void OnTargetDisconnected(ICompositionSupportsSystemBackdrop target)
    {
        DesktopWindowXamlSource = null;
        if (InnerBackdrop is not null)
        {
            ProtectedOnTargetDisconnected(InnerBackdrop, target);
        }
    }

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = nameof(OnTargetConnected))]
    private static extern void ProtectedOnTargetConnected(SystemBackdrop systemBackdrop, ICompositionSupportsSystemBackdrop target, XamlRoot xamlRoot);

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = nameof(OnTargetDisconnected))]
    private static extern void ProtectedOnTargetDisconnected(SystemBackdrop systemBackdrop, ICompositionSupportsSystemBackdrop target);
}