// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Foundation;

// ReSharper disable InconsistentNaming
internal readonly struct LPARAM
{
#pragma warning disable CS0649
    public readonly nint Value;
#pragma warning restore CS0649

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