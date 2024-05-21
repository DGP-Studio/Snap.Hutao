// Licensed to the .NET Fou// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Windows.Foundation;

namespace Snap.Hutao.Control.Layout;

internal sealed class WrapItem
{
    public WrapItem(int index)
    {
        Index = index;
    }

    public int Index { get; }

    public Size? Size { get; set; }

    public Point? Position { get; set; }

    public UIElement? Element { get; set; }
}
