// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Windows.Foundation;

namespace Snap.Hutao.UI.Xaml.Control.Layout;

internal sealed class WrapItem
{
    public WrapItem(int index)
    {
        Index = index;
    }

    public static Point EmptyPosition { get; } = new(float.NegativeInfinity, float.NegativeInfinity);

    public int Index { get; }

    public Size Size { get; set; } = Size.Empty;

    public Point Position { get; set; } = EmptyPosition;

    public UIElement? Element { get; set; }
}
