// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;

namespace Snap.Hutao.Core.Win32;

[SuppressMessage("", "SA1600")]
[StructLayout(LayoutKind.Sequential)]
public struct POINT
{
    public int X;
    public int Y;

    public POINT(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public static implicit operator System.Drawing.Point(POINT p)
    {
        return new System.Drawing.Point(p.X, p.Y);
    }

    public static implicit operator POINT(System.Drawing.Point p)
    {
        return new POINT(p.X, p.Y);
    }
}