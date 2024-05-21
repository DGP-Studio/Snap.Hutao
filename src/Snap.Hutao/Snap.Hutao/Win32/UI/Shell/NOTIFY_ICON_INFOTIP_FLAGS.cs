// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.UI.Shell;

[Flags]
internal enum NOTIFY_ICON_INFOTIP_FLAGS : uint
{
    NIIF_NONE = 0U,
    NIIF_INFO = 1U,
    NIIF_WARNING = 2U,
    NIIF_ERROR = 3U,
    NIIF_USER = 4U,
    NIIF_ICON_MASK = 0xFU,
    NIIF_NOSOUND = 0x10U,
    NIIF_LARGE_ICON = 0x20U,
    NIIF_RESPECT_QUIET_TIME = 0x80U,
}