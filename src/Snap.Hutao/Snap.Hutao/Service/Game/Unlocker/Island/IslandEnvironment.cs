// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Service.Game.Unlocker.Island;

internal struct IslandEnvironment
{
    public nuint Address;
    public int Value;
    public IslandState State;
    public WIN32_ERROR LastError;
    public int Reserved;
}