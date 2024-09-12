﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;

namespace Snap.Hutao.Win32.Foundation;

[SuppressMessage("", "SA1310")]
internal readonly partial struct HRESULT
{
    public static readonly HRESULT S_OK = unchecked(0x00000000);
    public static readonly HRESULT E_ASYNC_OPERATION_NOT_STARTED = unchecked((int)0x80000019);
    public static readonly HRESULT E_FAIL = unchecked((int)0x80004005);
    public static readonly HRESULT DXGI_ERROR_NOT_FOUND = unchecked((int)0x887A0002);
    public static readonly HRESULT DXGI_ERROR_DEVICE_REMOVED = unchecked((int)0x887A0005);
    public static readonly HRESULT DXGI_ERROR_DEVICE_RESET = unchecked((int)0x887A0007);

    public readonly int Value;

    public static unsafe implicit operator int(HRESULT value)
    {
        return *(int*)&value;
    }

    public static unsafe implicit operator HRESULT(int value)
    {
        return *(HRESULT*)&value;
    }

    public override string ToString()
    {
        return $"0x{Value:X8}";
    }
}

#if DEBUG
[DebuggerDisplay("{DebuggerDisplay}")]
internal readonly partial struct HRESULT
{
    private string DebuggerDisplay { get => ToString(); }
}
#endif