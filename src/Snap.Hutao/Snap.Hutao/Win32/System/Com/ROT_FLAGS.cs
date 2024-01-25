// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.System.Com;

[Flags]
internal enum ROT_FLAGS : uint
{
    ROTFLAGS_REGISTRATIONKEEPSALIVE = 1u,
    ROTFLAGS_ALLOWANYCLIENT = 2u,
}