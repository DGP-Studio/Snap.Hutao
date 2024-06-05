// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Service.Game.Unlocker;

internal readonly struct Module
{
    public readonly bool HasValue = false;
    public readonly nuint Address;
    public readonly uint Size;

    public Module(nuint address, uint size)
    {
        HasValue = true;
        Address = address;
        Size = size;
    }

    public unsafe Span<byte> AsSpan()
    {
        return new((void*)Address, (int)Size);
    }
}