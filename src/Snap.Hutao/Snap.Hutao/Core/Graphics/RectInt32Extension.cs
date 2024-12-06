// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Windows.Graphics;

namespace Snap.Hutao.Core.Graphics;

internal static class RectInt32Extension
{
    public static RectInt32 Scale(this RectInt32 rectInt32, double scale)
    {
        return new((int)(rectInt32.X * scale), (int)(rectInt32.Y * scale), (int)(rectInt32.Width * scale), (int)(rectInt32.Height * scale));
    }

    public static int Size(this RectInt32 rectInt32)
    {
        return rectInt32.Width * rectInt32.Height;
    }

    public static unsafe SizeInt32 GetSizeInt32(this RectInt32 rectInt32)
    {
        return ((RectInt32View*)&rectInt32)->Size;
    }

    public static unsafe PointInt32 GetPointInt32(this RectInt32 rectInt32, PointInt32Kind kind)
    {
        RectInt32View* pView = (RectInt32View*)&rectInt32;
        PointInt32 topLeft = pView->Position;
        SizeInt32 size = pView->Size;
        return kind switch
        {
            PointInt32Kind.TopLeft => topLeft,
            PointInt32Kind.TopCenter => new(topLeft.X + (size.Width / 2), topLeft.Y),
            PointInt32Kind.TopRight => new(topLeft.X + size.Width, topLeft.Y),
            PointInt32Kind.CenterLeft => new(topLeft.X, topLeft.Y + (size.Height / 2)),
            PointInt32Kind.Center => new(topLeft.X + (size.Width / 2), topLeft.Y + (size.Height / 2)),
            PointInt32Kind.CenterRight => new(topLeft.X + size.Width, topLeft.Y + (size.Height / 2)),
            PointInt32Kind.BottomLeft => new(topLeft.X, topLeft.Y + size.Height),
            PointInt32Kind.BottomCenter => new(topLeft.X + (size.Width / 2), topLeft.Y + size.Height),
            PointInt32Kind.BottomRight => new(topLeft.X + size.Width, topLeft.Y + size.Height),
            _ => default,
        };
    }

    public static RECT ToRECT(this RectInt32 rect)
    {
        return new(rect.X, rect.Y, rect.X + rect.Width, rect.Y + rect.Height);
    }
}