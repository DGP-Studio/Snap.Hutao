// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

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
}