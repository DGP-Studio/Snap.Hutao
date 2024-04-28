// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Foundation;

internal readonly struct HANDLE
{
    public readonly nint Value;

    public static unsafe implicit operator HANDLE(nint value) => *(HANDLE*)&value;

    public static implicit operator HANDLE(BOOL value) => value.Value;
}