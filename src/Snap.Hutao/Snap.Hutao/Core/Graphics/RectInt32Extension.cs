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

    public static RECT ToRECT(this RectInt32 rect)
    {
        return new(rect.X, rect.Y, rect.X + rect.Width, rect.Y + rect.Height);
    }
}