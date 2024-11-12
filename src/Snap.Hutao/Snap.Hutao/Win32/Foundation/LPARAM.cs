// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Foundation;

internal readonly struct LPARAM
{
    public readonly nint Value;

    public static unsafe implicit operator void*(LPARAM value)
    {
        return *(void**)&value;
    }

    public static unsafe implicit operator LPARAM(uint value)
    {
        nint data = (int)value;
        return *(LPARAM*)&data;
    }
}