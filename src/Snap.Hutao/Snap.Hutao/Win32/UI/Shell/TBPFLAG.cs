// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.UI.Shell;

[Flags]
internal enum TBPFLAG
{
    TBPF_NOPROGRESS = 0,
    TBPF_INDETERMINATE = 1,
    TBPF_NORMAL = 2,
    TBPF_ERROR = 4,
    TBPF_PAUSED = 8,
}