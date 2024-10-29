// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Service.Game.Unlocker.Island;

internal readonly struct IslandEnvironmentView
{
    public readonly IslandState State;
    public readonly WIN32_ERROR LastError;

    public readonly IslandFunctionOffsets FunctionOffsets;

    public readonly bool HookingSetFieldOfView;
    public readonly bool EnableSetFieldOfView;
    public readonly bool FixLowFovScene;
    public readonly bool DisableFog;
    public readonly float FieldOfView;
    public readonly int TargetFrameRate;
    public readonly bool HookingOpenTeam;
    public readonly bool RemoveOpenTeamProgress;
    public readonly bool HookingMickyWonderPartner2;
    public readonly bool Reserved;
}