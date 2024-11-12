// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Foundation;

internal readonly struct WPARAM
{
    public readonly nuint Value;

    public WPARAM(nuint value)
    {
        Value = value;
    }

    public static unsafe implicit operator uint(WPARAM value)
    {
        return (uint)*(nuint*)&value;
    }

    public static unsafe implicit operator WPARAM(uint value)
    {
        nuint data = value;
        return *(WPARAM*)&data;
    }

    public static unsafe implicit operator WPARAM(ushort value)
    {
        return new(value);
    }
}