// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.System.Com;

[SuppressMessage("", "SA1307")]
internal struct STATDATA
{
    public FORMATETC formatetc;
    public uint advf;
    public unsafe IAdviseSink.Vftbl* pAdvSink;
    public uint dwConnection;
}