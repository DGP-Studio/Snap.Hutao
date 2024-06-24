// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Extension;

internal static class SizeExtension
{
    public static double AspectRatio(this Windows.Foundation.Size size)
    {
        return size.Width / size.Height;
    }

    public static double Size(this Windows.Foundation.Size size)
    {
        return size.Width * size.Height;
    }
}