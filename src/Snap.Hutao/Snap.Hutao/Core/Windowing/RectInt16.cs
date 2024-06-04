// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Graphics;

namespace Snap.Hutao.Core.Windowing;

internal readonly struct RectInt16
{
    private readonly short x;
    private readonly short y;
    private readonly short width;
    private readonly short height;

    private RectInt16(int x, int y, int width, int height)
    {
        this.x = (short)x;
        this.y = (short)y;
        this.width = (short)width;
        this.height = (short)height;
    }

    public static implicit operator RectInt32(RectInt16 rect)
    {
        return new(rect.x, rect.y, rect.width, rect.height);
    }

    public static explicit operator RectInt16(RectInt32 rect)
    {
        return new(rect.X, rect.Y, rect.Width, rect.Height);
    }

    public static unsafe explicit operator RectInt16(ulong value)
    {
        return *(RectInt16*)&value;
    }

    public static unsafe implicit operator ulong(RectInt16 rect)
    {
        return *(ulong*)&rect;
    }
}