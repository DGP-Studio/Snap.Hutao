// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao;

internal static class GlobalSwitch
{
#if DEBUG
    public const bool PreventCopyIslandDll = false;
#else
    public const bool PreventCopyIslandDll = false;
#endif
}