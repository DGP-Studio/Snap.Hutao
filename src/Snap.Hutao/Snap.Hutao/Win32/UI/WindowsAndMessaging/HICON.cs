// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.UI.WindowsAndMessaging;

// RAIIFree: DestroyIcon
// InvalidHandleValue: -1, 0
internal readonly struct HICON
{
    public readonly nint Value;

    public static unsafe implicit operator HICON(nint value) => *(HICON*)&value;

    public static unsafe implicit operator nint(HICON value) => *(nint*)&value;
}