// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace Snap.Hutao.UI.Xaml.Media.Backdrop;

internal sealed partial class TransparentBackdrop : SystemBackdrop, IBackdropNeedEraseBackground
{
    private readonly Color tintColor;

    private Windows.UI.Composition.CompositionColorBrush? brush;
    private Windows.UI.Composition.Compositor? compositor;
    private object? compositorLock;

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
        // ReSharper disable once InconsistentlySynchronizedField
        get => LazyInitializer.EnsureInitialized(ref compositor, ref compositorLock, () =>
        {
            DispatcherQueue.EnsureSystemDispatcherQueue();
            return new();
        });
    }

    protected override void OnTargetConnected(ICompositionSupportsSystemBackdrop target, XamlRoot xamlRoot)
    {
        brush ??= Compositor.CreateColorBrush(tintColor);
        target.SystemBackdrop = brush;
    }

    protected override void OnTargetDisconnected(ICompositionSupportsSystemBackdrop target)
    {
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