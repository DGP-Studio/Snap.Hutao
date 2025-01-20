// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;

namespace Snap.Hutao.Core.LifeCycle.InterProcess.Yae;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal readonly struct YaePacketHeader
{
    public readonly YaeDataKind Kind;
    public readonly int ContentLength;
}