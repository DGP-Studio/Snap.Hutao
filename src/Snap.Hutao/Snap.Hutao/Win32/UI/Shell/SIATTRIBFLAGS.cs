// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.UI.Shell;

[Flags]
[SuppressMessage("", "CA1069")]
internal enum SIATTRIBFLAGS
{
    SIATTRIBFLAGS_AND = 1,
    SIATTRIBFLAGS_OR = 2,
    SIATTRIBFLAGS_APPCOMPAT = 3,
    SIATTRIBFLAGS_MASK = 3,
    SIATTRIBFLAGS_ALLITEMS = 0x4000,
}