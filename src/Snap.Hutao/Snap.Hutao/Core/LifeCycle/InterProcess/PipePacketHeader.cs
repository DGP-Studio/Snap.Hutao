// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;

namespace Snap.Hutao.Core.LifeCycle.InterProcess;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct PipePacketHeader
{
    public byte Version;
    public PipePacketType Type;
    public PipePacketCommand Command;
    public PipePacketContentType ContentType;
    public int ContentLength;
    public ulong Checksum;
}