// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace Snap.Hutao.Core.Windowing.Backdrop;

internal sealed class TransparentBackdrop : SystemBackdrop, IDisposable, IBackdropNeedEraseBackground
{
    private readonly object compositorLock = new();

    private Color tintColor;
    private Windows.UI.Composition.CompositionColorBrush? brush;
    private Windows.UI.Composition.Compositor? compositor;

    public TransparentBackdrop()
        : this(Colors.Transparent)
    {
    }

    public TransparentBackdrop(Color tintColor)
    {
        this.tintColor = tintColor;
    }

    internal Windows.UI.Composition.Compositor Compositor
    {
        get
        {
            if (compositor is null)
            {
                lock (compositorLock)
                {
                    if (compositor is null)
                    {
                        DispatcherQueue.EnsureSystemDispatcherQueue();
                        compositor = new Windows.UI.Composition.Compositor();
                    }
                }
            }

            return compositor;
        }
    }

    public void Dispose()
    {
        compositor?.Dispose();
    }

    protected override void OnTargetConnected(ICompositionSupportsSystemBackdrop connectedTarget, XamlRoot xamlRoot)
    {
        brush ??= Compositor.CreateColorBrush(tintColor);
        connectedTarget.SystemBackdrop = brush;
    }

    protected override void OnTargetDisconnected(ICompositionSupportsSystemBackdrop disconnectedTarget)
    {
        disconnectedTarget.SystemBackdrop = null;
    }
}