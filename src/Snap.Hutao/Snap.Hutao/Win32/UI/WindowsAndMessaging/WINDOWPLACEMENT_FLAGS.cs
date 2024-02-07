// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.UI.WindowsAndMessaging;

[Flags]
internal enum WINDOWPLACEMENT_FLAGS : uint
{
    WPF_ASYNCWINDOWPLACEMENT = 4u,
    WPF_RESTORETOMAXIMIZED = 2u,
    WPF_SETMINPOSITION = 1u,
}