// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Foundation;

// RAIIFree: CloseHandle
// InvalidHandleValue: -1, 0
internal readonly struct HANDLE
{
    public readonly nint Value;

    public static unsafe implicit operator HANDLE(nint value) => *(HANDLE*)&value;

    public static unsafe implicit operator HANDLE(BOOL value) => *(int*)&value;
}