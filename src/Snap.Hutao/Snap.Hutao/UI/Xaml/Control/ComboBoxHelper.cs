// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using WinRT;

namespace Snap.Hutao.UI.Xaml.Control;

[DependencyProperty("SystemBackdropWorkaround", typeof(bool), false, nameof(OnSystemBackdropWorkaroundChanged), IsAttached = true, AttachedType = typeof(Microsoft.UI.Xaml.Controls.ComboBox))]
public sealed partial class ComboBoxHelper
{
    private static void OnSystemBackdropWorkaroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is not Microsoft.UI.Xaml.Controls.ComboBox comboBox)
        {
            ActivationFactory.Get("Microsoft.UI.Content.ContentExternalBackdropLink").As<IContentExternalBackdropLinkStatics>(new("46CAC6FB-BB51-510A-958D-E0EB4160F678"));
            return;
        }
    }
}

[SuppressMessage("", "SA1201")]
[SuppressMessage("", "SYSLIB1096")]
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
[Guid("1054BF83-B35B-5FDE-8DD7-AC3BB3E6CE27")]
file unsafe interface IContentExternalBackdropLink
{
    [PreserveSig]
    HRESULT GetIids(/* Ignored */);

    [PreserveSig]
    HRESULT GetRuntimeClassName(/* Ignored */);

    [PreserveSig]
    HRESULT GetTrustLevel(/* Ignored */);

    [PreserveSig]
    HRESULT DispatcherQueue(/* Microsoft.UI.Dispatching.DispatcherQueue** */ nint* value);

    [PreserveSig]
    HRESULT ExternalBackdropBorderMode(Microsoft.UI.Composition.CompositionBorderMode* value);

    [PreserveSig]
    HRESULT ExternalBackdropBorderMode(Microsoft.UI.Composition.CompositionBorderMode value);

    [PreserveSig]
    HRESULT PlacementVisual(/* Microsoft.UI.Composition.Visual** */ nint* value);
}

[SuppressMessage("", "SA1201")]
[SuppressMessage("", "SYSLIB1096")]
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
[Guid("46CAC6FB-BB51-510A-958D-E0EB4160F678")]
file unsafe interface IContentExternalBackdropLinkStatics
{
    [PreserveSig]
    HRESULT GetIids(/* Ignored */);

    [PreserveSig]
    HRESULT GetRuntimeClassName(/* Ignored */);

    [PreserveSig]
    HRESULT GetTrustLevel(/* Ignored */);

    [PreserveSig]
    HRESULT Create(/* Microsoft.UI.Composition.Compositor* */ nint compositor, /* Microsoft.UI.Content.ContentExternalBackdropLink** */ nint* result);
}