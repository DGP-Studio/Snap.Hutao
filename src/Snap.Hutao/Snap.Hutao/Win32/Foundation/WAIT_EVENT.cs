// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Foundation;

internal enum WAIT_EVENT : uint
{
    WAIT_OBJECT_0 = 0U,
    WAIT_ABANDONED = 128U,
    WAIT_ABANDONED_0 = 128U,
    WAIT_IO_COMPLETION = 192U,
    WAIT_TIMEOUT = 258U,
    WAIT_FAILED = 4294967295U,
}