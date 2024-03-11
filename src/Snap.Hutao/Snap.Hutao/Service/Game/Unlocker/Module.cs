// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

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
}