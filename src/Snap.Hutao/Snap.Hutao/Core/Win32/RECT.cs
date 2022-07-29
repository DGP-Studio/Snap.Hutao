// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Globalization;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Core.Win32;

[SuppressMessage("", "SA1132")]
[SuppressMessage("", "SA1600")]
[StructLayout(LayoutKind.Sequential)]
public struct RECT
{
    public int Left, Top, Right, Bottom;

    public RECT(int left, int top, int right, int bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public RECT(System.Drawing.Rectangle r)
        : this(r.Left, r.Top, r.Right, r.Bottom)
    {
    }

    public int X
    {
        get => Left;
        set
        {
            Right -= Left - value;
            Left = value;
        }
    }

    public int Y
    {
        get => Top;
        set
        {
            Bottom -= Top - value;
            Top = value;
        }
    }

    public int Height
    {
        get => Bottom - Top;
        set => Bottom = value + Top;
    }

    public int Width
    {
        get => Right - Left;
        set => Right = value + Left;
    }

    public long Area
    {
        get => Math.BigMul(Width, Height);
    }

    public System.Drawing.Point Location
    {
        get => new(Left, Top);
        set
        {
            X = value.X;
            Y = value.Y;
        }
    }

    public System.Drawing.Size Size
    {
        get => new(Width, Height);
        set
        {
            Width = value.Width;
            Height = value.Height;
        }
    }

    public static implicit operator System.Drawing.Rectangle(RECT r)
    {
        return new System.Drawing.Rectangle(r.Left, r.Top, r.Width, r.Height);
    }

    public static implicit operator RECT(System.Drawing.Rectangle r)
    {
        return new RECT(r);
    }

    public static bool operator ==(RECT r1, RECT r2)
    {
        return r1.Equals(r2);
    }

    public static bool operator !=(RECT r1, RECT r2)
    {
        return !r1.Equals(r2);
    }

    public bool Equals(RECT r)
    {
        return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
    }

    public override bool Equals(object? obj)
    {
        if (obj is RECT rect)
        {
            return Equals(rect);
        }
        else if (obj is System.Drawing.Rectangle rectangle)
        {
            return Equals(new RECT(rectangle));
        }

        return false;
    }

    public override int GetHashCode()
    {
        return ((System.Drawing.Rectangle)this).GetHashCode();
    }

    public override string ToString()
    {
        return string.Format(CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top, Right, Bottom);
    }
}