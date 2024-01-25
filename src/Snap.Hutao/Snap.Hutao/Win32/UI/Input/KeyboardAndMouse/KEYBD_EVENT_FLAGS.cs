// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.UI.Input.KeyboardAndMouse;

[Flags]
internal enum KEYBD_EVENT_FLAGS : uint
{
    KEYEVENTF_EXTENDEDKEY = 1u,
    KEYEVENTF_KEYUP = 2u,
    KEYEVENTF_SCANCODE = 8u,
    KEYEVENTF_UNICODE = 4u,
}