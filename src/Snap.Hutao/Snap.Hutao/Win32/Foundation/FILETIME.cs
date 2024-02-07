// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Foundation;

[SuppressMessage("", "SA1307")]
internal readonly struct FILETIME
{
    public readonly uint dwLowDateTime;
    public readonly uint dwHighDateTime;
}