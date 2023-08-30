// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.UI;

namespace Snap.Hutao.Control;

internal sealed class ColorSegment
{
    public ColorSegment(Color color, double value)
    {
        Color = color;
        Value = value;
    }

    public Color Color { get; set; }

    public double Value { get; set; }
}