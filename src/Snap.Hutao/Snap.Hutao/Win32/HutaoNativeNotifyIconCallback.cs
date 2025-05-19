// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32;

internal readonly unsafe struct HutaoNativeNotifyIconCallback
{
    private readonly delegate* unmanaged[Stdcall]<HutaoNativeNotifyIconCallbackKind, RECT, POINT, nint, void> value;

    public HutaoNativeNotifyIconCallback(delegate* unmanaged[Stdcall]<HutaoNativeNotifyIconCallbackKind, RECT, POINT, nint, void> value)
    {
        this.value = value;
    }

    public static HutaoNativeNotifyIconCallback Create(delegate* unmanaged[Stdcall]<HutaoNativeNotifyIconCallbackKind, RECT, POINT, nint, void> method)
    {
        return new(method);
    }
}