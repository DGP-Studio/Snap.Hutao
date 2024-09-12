// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Foundation;

// RAIIFree: FreeLibrary
// InvalidHandleValue: 0
internal readonly struct HMODULE
{
    public readonly nint Value;

    public static unsafe implicit operator HMODULE(nint value)
    {
        return *(HMODULE*)&value;
    }

    public static unsafe implicit operator nint(HMODULE module)
    {
        return *(nint*)&module;
    }
}