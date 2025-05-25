// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Foundation;

// ReSharper disable InconsistentNaming
[SuppressMessage("", "SA1310")]
internal readonly struct HANDLE
{
#pragma warning disable CS0649
    public readonly nint Value;
#pragma warning restore CS0649

    public static unsafe implicit operator HANDLE(nint value)
    {
        return *(HANDLE*)&value;
    }

    public static unsafe implicit operator nint(HANDLE handle)
    {
        return *(nint*)&handle;
    }

    public static unsafe implicit operator HANDLE(BOOL value)
    {
        return *(int*)&value;
    }
}