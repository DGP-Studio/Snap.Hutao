// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Graphics.Gdi;

internal readonly struct HDC
{
    public static readonly HDC NULL = 0;

    public readonly nint Value;

    public static unsafe implicit operator HDC(nint value) => *(HDC*)&value;

    public static unsafe bool operator ==(HDC left, HDC right) => *(nint*)&left == *(nint*)&right;

    public static unsafe bool operator !=(HDC left, HDC right) => *(nint*)&left != *(nint*)&right;
}