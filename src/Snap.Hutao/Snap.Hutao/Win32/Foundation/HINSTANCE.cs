// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Foundation;

// RAIIFree: FreeLibrary
// InvalidHandleValue: 0
internal readonly struct HINSTANCE
{
    public readonly nint Value;

    public static unsafe implicit operator HINSTANCE(HANDLE value)
    {
        return *(HINSTANCE*)&value;
    }
}