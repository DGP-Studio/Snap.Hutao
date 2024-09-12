// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.UI.WindowsAndMessaging;

// RAIIFree: UnhookWindowsHookEx
// InvalidHandleValue: -1, 0
internal readonly struct HHOOK
{
    public readonly nint Value;
}