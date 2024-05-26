// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Foundation;

internal readonly struct HWND
{
    public readonly nint Value;

    public static unsafe implicit operator HWND(nint value) => *(HWND*)&value;

    public static unsafe implicit operator nint(HWND value) => *(nint*)&value;
}