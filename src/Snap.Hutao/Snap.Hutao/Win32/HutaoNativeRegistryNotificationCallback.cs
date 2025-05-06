// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32;

internal readonly unsafe struct HutaoNativeRegistryNotificationCallback
{
    private readonly delegate* unmanaged[Stdcall]<nint, void> value;

    public HutaoNativeRegistryNotificationCallback(delegate* unmanaged[Stdcall]<nint, void> value)
    {
        this.value = value;
    }

    public static HutaoNativeRegistryNotificationCallback Create(delegate* unmanaged[Stdcall]<nint, void> method)
    {
        return new(method);
    }
}