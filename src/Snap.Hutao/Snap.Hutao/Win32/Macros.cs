// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Diagnostics.Debug;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Win32;

[SuppressMessage("", "SH002")]
internal static class Macros
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool SUCCEEDED(HRESULT hr)
    {
        return hr >= 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool FAILED(HRESULT hr)
    {
        return hr < 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HRESULT HRESULT_FROM_WIN32(WIN32_ERROR x)
    {
        // 0x80000000 or 0x80070000 | LOWBYTE(x)
        return x <= 0 ? (int)x : (int)(((uint)x & 0x0000FFFFU) | ((uint)FACILITY_CODE.FACILITY_WIN32 << 16) | 0x80000000U);
    }

    public static unsafe COLORREF RGB(byte r, byte g, byte b)
    {
        uint value = (ushort)(r | g << 8) | (uint)b << 16;
        return *(COLORREF*)&value;
    }
}