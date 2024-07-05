// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace Snap.Hutao.UI.Xaml.Media.Backdrop;

internal sealed class TransparentBackdrop : SystemBackdrop, IBackdropNeedEraseBackground
{
    private object? compositorLock;

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
        get => LazyInitializer.EnsureInitialized(ref compositor, ref compositorLock, () =>
        {
            DispatcherQueue.EnsureSystemDispatcherQueue();
            return new Windows.UI.Composition.Compositor();
        });
    }

    protected override void OnTargetConnected(ICompositionSupportsSystemBackdrop target, XamlRoot xamlRoot)
    {
        base.OnTargetConnected(target, xamlRoot);

        brush ??= Compositor.CreateColorBrush(tintColor);
        target.SystemBackdrop = brush;
    }

    protected override void OnTargetDisconnected(ICompositionSupportsSystemBackdrop target)
    {
        base.OnTargetDisconnected(target);

        target.SystemBackdrop = null;

        if (compositorLock is not null)
        {
            lock (compositorLock)
            {
                compositor?.Dispose();
            }
        }
    }
}