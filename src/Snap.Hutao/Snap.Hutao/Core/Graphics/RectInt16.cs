// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Graphics;

namespace Snap.Hutao.Core.Graphics;

internal readonly struct RectInt16
{
    public readonly short X;
    public readonly short Y;
    public readonly short Width;
    public readonly short Height;

    private RectInt16(int x, int y, int width, int height)
    {
        this.X = (short)x;
        this.Y = (short)y;
        this.Width = (short)width;
        this.Height = (short)height;
    }

    public static implicit operator RectInt32(RectInt16 rect)
    {
        return new(rect.X, rect.Y, rect.Width, rect.Height);
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