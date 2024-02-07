// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.System.Com;

[SuppressMessage("", "SA1307")]
internal struct BIND_OPTS
{
    public uint cbStruct;
    public uint grfFlags;
    public uint grfMode;
    public uint dwTickCountDeadline;
}