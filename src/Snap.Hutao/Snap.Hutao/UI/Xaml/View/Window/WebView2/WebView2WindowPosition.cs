// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Snap.Hutao.Core.Graphics;
using Windows.Graphics;

namespace Snap.Hutao.UI.Xaml.View.Window.WebView2;

internal static class WebView2WindowPosition
{
    public static RectInt32 Padding(RectInt32 parentRect, int padding)
    {
        return new RectInt32(parentRect.X + padding, parentRect.Y + padding, parentRect.Width - (padding * 2), parentRect.Height - (padding * 2));
    }

    public static RectInt32 Vertical(RectInt32 parentRect, double parentDpi)
    {
        PointInt32 center = parentRect.GetPointInt32(PointInt32Kind.Center);
        SizeInt32 size = new SizeInt32(480, 800).Scale(parentDpi);
        RectInt32 target = RectInt32Convert.RectInt32(new(center.X - (size.Width / 2), center.Y - (size.Height / 2)), size);
        RectInt32 workArea = DisplayArea.GetFromRect(parentRect, DisplayAreaFallback.Primary).WorkArea;
        RectInt32 workAreaShrink = Padding(workArea, 48);

        if (target.Width > workAreaShrink.Width)
        {
            target.Width = workAreaShrink.Width;
        }

        if (target.Height > workAreaShrink.Height)
        {
            target.Height = workAreaShrink.Height;
        }

        PointInt32 topLeft = target.GetPointInt32(PointInt32Kind.TopLeft);

        if (topLeft.X < workAreaShrink.X)
        {
            target.X = workAreaShrink.X;
        }

        if (topLeft.Y < workAreaShrink.Y)
        {
            target.Y = workAreaShrink.Y;
        }

        PointInt32 bottomRight = target.GetPointInt32(PointInt32Kind.BottomRight);
        PointInt32 workAreeShrinkBottomRight = workAreaShrink.GetPointInt32(PointInt32Kind.BottomRight);

        if (bottomRight.X > workAreeShrinkBottomRight.X)
        {
            target.X = workAreeShrinkBottomRight.X - target.Width;
        }

        if (bottomRight.Y > workAreeShrinkBottomRight.Y)
        {
            target.Y = workAreeShrinkBottomRight.Y - target.Height;
        }

        return target;
    }
}