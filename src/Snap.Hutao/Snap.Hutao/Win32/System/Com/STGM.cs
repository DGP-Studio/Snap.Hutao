// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.System.Com;

[SuppressMessage("", "CA1069")]
internal enum STGM : uint
{
    STGM_DIRECT = 0u,
    STGM_TRANSACTED = 0x10000u,
    STGM_SIMPLE = 0x8000000u,
    STGM_READ = 0u,
    STGM_WRITE = 1u,
    STGM_READWRITE = 2u,
    STGM_SHARE_DENY_NONE = 0x40u,
    STGM_SHARE_DENY_READ = 0x30u,
    STGM_SHARE_DENY_WRITE = 0x20u,
    STGM_SHARE_EXCLUSIVE = 0x10u,
    STGM_PRIORITY = 0x40000u,
    STGM_DELETEONRELEASE = 0x4000000u,
    STGM_NOSCRATCH = 0x100000u,
    STGM_CREATE = 0x1000u,
    STGM_CONVERT = 0x20000u,
    STGM_FAILIFTHERE = 0u,
    STGM_NOSNAPSHOT = 0x200000u,
    STGM_DIRECT_SWMR = 0x400000u,
}