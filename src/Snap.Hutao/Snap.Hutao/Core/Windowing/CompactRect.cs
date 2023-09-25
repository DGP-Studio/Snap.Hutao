// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using Windows.Graphics;

namespace Snap.Hutao.Core.Windowing;

internal readonly struct CompactRect
{
    private readonly short x;
    private readonly short y;
    private readonly short width;
    private readonly short height;

    private CompactRect(int x, int y, int width, int height)
    {
        this.x = (short)x;
        this.y = (short)y;
        this.width = (short)width;
        this.height = (short)height;
    }

    public static implicit operator RectInt32(CompactRect rect)
    {
        return new(rect.x, rect.y, rect.width, rect.height);
    }

    public static explicit operator CompactRect(RectInt32 rect)
    {
        return new(rect.X, rect.Y, rect.Width, rect.Height);
    }

    public static unsafe explicit operator CompactRect(ulong value)
    {
        Unsafe.SkipInit(out CompactRect rect);
        *(ulong*)&rect = value;
        return rect;
    }

    public static unsafe implicit operator ulong(CompactRect rect)
    {
        return *(ulong*)&rect;
    }
}