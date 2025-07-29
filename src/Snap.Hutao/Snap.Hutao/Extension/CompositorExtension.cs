// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using System.Numerics;

namespace Snap.Hutao.Extension;

internal static class CompositorExtension
{
    public static RectangleClip CreateRectangleClip(this Compositor compositor, Vector2 size, CornerRadius cornerRadius)
    {
        float topLeft = (float)cornerRadius.TopLeft;
        float topRight = (float)cornerRadius.TopRight;
        float bottomLeft = (float)cornerRadius.BottomLeft;
        float bottomRight = (float)cornerRadius.BottomRight;
        return compositor.CreateRectangleClip(
            0,
            0,
            size.X,
            size.Y,
            new(topLeft, topLeft),
            new(topRight, topRight),
            new(bottomRight, bottomRight),
            new(bottomLeft, bottomLeft));
    }
}