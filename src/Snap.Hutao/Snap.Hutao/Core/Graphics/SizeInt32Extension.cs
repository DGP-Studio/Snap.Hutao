// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Graphics;

namespace Snap.Hutao.Core.Graphics;

internal static class SizeInt32Extension
{
    public static SizeInt32 Scale(this SizeInt32 sizeInt32, double scale)
    {
        return new((int)(sizeInt32.Width * scale), (int)(sizeInt32.Height * scale));
    }

    public static int Size(this SizeInt32 sizeInt32)
    {
        return sizeInt32.Width * sizeInt32.Height;
    }
}