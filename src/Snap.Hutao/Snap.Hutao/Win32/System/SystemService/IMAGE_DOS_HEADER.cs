// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.System.SystemService;

[SuppressMessage("", "SA1307")]
[SuppressMessage("", "SA1310")]
[StructLayout(LayoutKind.Sequential, Pack = 2)]
internal struct IMAGE_DOS_HEADER
{
    public ushort e_magic;
    public ushort e_cblp;
    public ushort e_cp;
    public ushort e_crlc;
    public ushort e_cparhdr;
    public ushort e_minalloc;
    public ushort e_maxalloc;
    public ushort e_ss;
    public ushort e_sp;
    public ushort e_csum;
    public ushort e_ip;
    public ushort e_cs;
    public ushort e_lfarlc;
    public ushort e_ovno;
    public unsafe fixed ushort e_res[4];
    public ushort e_oemid;
    public ushort e_oeminfo;
    public unsafe fixed ushort e_res2[10];
    public int e_lfanew;
}
