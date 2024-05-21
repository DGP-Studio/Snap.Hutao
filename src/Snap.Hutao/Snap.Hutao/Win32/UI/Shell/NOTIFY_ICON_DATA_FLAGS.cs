// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.UI.Shell;

[Flags]
internal enum NOTIFY_ICON_DATA_FLAGS : uint
{
    NIF_MESSAGE = 0x1U,
    NIF_ICON = 0x2U,
    NIF_TIP = 0x4U,
    NIF_STATE = 0x8U,
    NIF_INFO = 0x10U,
    NIF_GUID = 0x20U,
    NIF_REALTIME = 0x40U,
    NIF_SHOWTIP = 0x80U,
}