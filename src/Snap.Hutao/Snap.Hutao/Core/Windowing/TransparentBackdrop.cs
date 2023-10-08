// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Media;
using Windows.Win32.System.WinRT;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Core.Windowing;

internal class TransparentBackdrop : SystemBackdrop
{
    private static readonly Lazy<Windows.UI.Composition.Compositor> LazyCompositor = new(() =>
    {
        EnsureWindowsSystemDispatcherQueueController();
        return new();
    });

    private static Windows.System.DispatcherQueueController? dispatcherQueueController;

    private static Windows.UI.Composition.Compositor Compositor => LazyCompositor.Value;

    protected override void OnTargetConnected(ICompositionSupportsSystemBackdrop connectedTarget, Microsoft.UI.Xaml.XamlRoot xamlRoot)
    {
        connectedTarget.SystemBackdrop = Compositor.CreateColorBrush(Windows.UI.Color.FromArgb(0, 255, 255, 255));
    }

    protected override void OnTargetDisconnected(ICompositionSupportsSystemBackdrop disconnectedTarget)
    {
        disconnectedTarget.SystemBackdrop = null;
    }

    private static unsafe void EnsureWindowsSystemDispatcherQueueController()
    {
        if (Windows.System.DispatcherQueue.GetForCurrentThread() is not null)
        {
            return;
        }

        if (dispatcherQueueController is null)
        {
            DispatcherQueueOptions options = new()
            {
                dwSize = (uint)sizeof(DispatcherQueueOptions),
                threadType = DISPATCHERQUEUE_THREAD_TYPE.DQTYPE_THREAD_CURRENT,
                apartmentType = DISPATCHERQUEUE_THREAD_APARTMENTTYPE.DQTAT_COM_STA,
            };

            CreateDispatcherQueueController(options, out dispatcherQueueController);
        }
    }
}