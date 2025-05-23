// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32;

internal readonly unsafe struct HutaoNativeHotKeyBeforeSwitchCallback
{
    private readonly delegate* unmanaged[Stdcall]<BOOL> value;

    public HutaoNativeHotKeyBeforeSwitchCallback(delegate* unmanaged[Stdcall]<BOOL> value)
    {
        this.value = value;
    }

    public static HutaoNativeHotKeyBeforeSwitchCallback Create(delegate* unmanaged[Stdcall]<BOOL> method)
    {
        return new(method);
    }
}