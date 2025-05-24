// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Foundation;

internal readonly struct HWND
{
#pragma warning disable CS0649
    public readonly nint Value;
#pragma warning restore CS0649

    public static unsafe implicit operator HWND(nint value)
    {
        return *(HWND*)&value;
    }

    public static unsafe implicit operator nint(HWND value)
    {
        return *(nint*)&value;
    }
}