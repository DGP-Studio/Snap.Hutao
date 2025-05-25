// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32;

internal readonly unsafe struct HutaoNativeHotKeyActionCallback
{
    private readonly delegate* unmanaged[Stdcall]<BOOL, nint, void> value;

    public HutaoNativeHotKeyActionCallback(delegate* unmanaged[Stdcall]<BOOL, nint, void> value)
    {
        this.value = value;
    }

    public static HutaoNativeHotKeyActionCallback Create(delegate* unmanaged[Stdcall]<BOOL, nint, void> method)
    {
        return new(method);
    }
}