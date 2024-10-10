// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.System.Memory;

[Flags]
internal enum VIRTUAL_ALLOCATION_TYPE : uint
{
    MEM_COMMIT = 0x1000U,
    MEM_RESERVE = 0x2000U,
    MEM_RESET = 0x80000U,
    MEM_RESET_UNDO = 0x1000000U,
    MEM_REPLACE_PLACEHOLDER = 0x4000U,
    MEM_LARGE_PAGES = 0x20000000U,
    MEM_RESERVE_PLACEHOLDER = 0x40000U,
    MEM_FREE = 0x10000U,
}