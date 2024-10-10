// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.System.Threading;

internal readonly unsafe struct LPTHREAD_START_ROUTINE
{
    [SuppressMessage("", "IDE0052")]
    private readonly delegate* unmanaged[Stdcall]<void*, uint> value;

    public LPTHREAD_START_ROUTINE(delegate* unmanaged[Stdcall]<void*, uint> method)
    {
        value = method;
    }

    public static LPTHREAD_START_ROUTINE Create(delegate* unmanaged[Stdcall]<void*, uint> method)
    {
        return new(method);
    }
}