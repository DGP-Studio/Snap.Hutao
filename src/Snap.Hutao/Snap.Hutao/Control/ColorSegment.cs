// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.UI;

namespace Snap.Hutao.Control;

internal sealed class ColorSegment : IColorSegment
{
    public ColorSegment(Color color, double value)
    {
        Color = color;
        Value = value;
    }

    public Color Color { get; set; }

    public double Value { get; set; }
}