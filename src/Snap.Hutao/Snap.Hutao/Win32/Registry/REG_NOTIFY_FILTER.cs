// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.Registry;

[Flags]
internal enum REG_NOTIFY_FILTER : uint
{
    REG_NOTIFY_CHANGE_NAME = 1u,
    REG_NOTIFY_CHANGE_ATTRIBUTES = 2u,
    REG_NOTIFY_CHANGE_LAST_SET = 4u,
    REG_NOTIFY_CHANGE_SECURITY = 8u,

    [SupportedOSPlatform("windows8.0")]
    REG_NOTIFY_THREAD_AGNOSTIC = 0x10000000u,
}