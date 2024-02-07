// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Registry;

[SuppressMessage("", "CA1069")]
[Flags]
internal enum REG_SAM_FLAGS : uint
{
    KEY_QUERY_VALUE = 1u,
    KEY_SET_VALUE = 2u,
    KEY_CREATE_SUB_KEY = 4u,
    KEY_ENUMERATE_SUB_KEYS = 8u,
    KEY_NOTIFY = 0x10u,
    KEY_CREATE_LINK = 0x20u,
    KEY_WOW64_32KEY = 0x200u,
    KEY_WOW64_64KEY = 0x100u,
    KEY_WOW64_RES = 0x300u,
    KEY_READ = 0x20019u,
    KEY_WRITE = 0x20006u,
    KEY_EXECUTE = 0x20019u,
    KEY_ALL_ACCESS = 0xF003Fu,
}