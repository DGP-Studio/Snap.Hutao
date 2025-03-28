// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.UI.WindowsAndMessaging;

[Flags]
internal enum WINEVENT_FLAGS : uint
{
    WINEVENT_OUTOFCONTEXT = 0u,
    WINEVENT_SKIPOWNTHREAD = 1u,
    WINEVENT_SKIPOWNPROCESS = 2u,
    WINEVENT_INCONTEXT = 4u,
}