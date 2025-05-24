// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Foundation;

// ReSharper disable InconsistentNaming
internal readonly struct LRESULT
{
#pragma warning disable CS0649
    public readonly nint Value;
#pragma warning restore CS0649

    public static unsafe implicit operator LRESULT(nint value)
    {
        return *(LRESULT*)&value;
    }

    public static unsafe implicit operator LRESULT(BOOL value)
    {
        return *(int*)&value;
    }
}