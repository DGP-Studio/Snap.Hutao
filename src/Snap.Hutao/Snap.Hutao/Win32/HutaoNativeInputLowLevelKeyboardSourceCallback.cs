// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;

namespace Snap.Hutao.Win32;

internal readonly unsafe struct HutaoNativeInputLowLevelKeyboardSourceCallback
{
    private readonly delegate* unmanaged[Stdcall]<uint, KBDLLHOOKSTRUCT*, BOOL> value;

    public HutaoNativeInputLowLevelKeyboardSourceCallback(delegate* unmanaged[Stdcall]<uint, KBDLLHOOKSTRUCT*, BOOL> value)
    {
        this.value = value;
    }

    public static HutaoNativeInputLowLevelKeyboardSourceCallback Create(delegate* unmanaged[Stdcall]<uint, KBDLLHOOKSTRUCT*, BOOL> method)
    {
        return new(method);
    }
}