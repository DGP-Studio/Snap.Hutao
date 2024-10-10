// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.System.Memory;

internal enum VIRTUAL_FREE_TYPE : uint
{
    MEM_DECOMMIT = 0x4000U,
    MEM_RELEASE = 0x8000U,
}