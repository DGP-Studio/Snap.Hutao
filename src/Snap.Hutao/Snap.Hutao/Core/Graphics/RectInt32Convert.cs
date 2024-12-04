// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Numerics;
using Windows.Foundation;
using Windows.Graphics;

namespace Snap.Hutao.Core.Graphics;

internal static class RectInt32Convert
{
    public static RectInt32 RectInt32(RECT rect)
    {
        return new(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
    }

    public static RectInt32 RectInt32(Point position, Vector2 size)
    {
        return new((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
    }

    public static RectInt32 RectInt32(int x, int y, Vector2 size)
    {
        return new(x, y, (int)size.X, (int)size.Y);
    }

    public static unsafe RectInt32 RectInt32(PointInt32 position, SizeInt32 size)
    {
        RectInt32View view = default;
        view.Position = position;
        view.Size = size;
        return *(RectInt32*)&view;
    }
}