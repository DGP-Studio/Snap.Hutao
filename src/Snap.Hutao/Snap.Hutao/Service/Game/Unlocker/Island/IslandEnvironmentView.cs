// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Service.Game.Unlocker.Island;

internal readonly struct IslandEnvironmentView
{
    public readonly IslandState State;
    public readonly WIN32_ERROR LastError;
}