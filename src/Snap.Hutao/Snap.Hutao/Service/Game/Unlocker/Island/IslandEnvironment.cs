// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Service.Game.Unlocker.Island;

internal struct IslandEnvironment
{
    public IslandState State;
    public WIN32_ERROR LastError;

    public IslandFunctionOffsets FunctionOffsets;

    public bool HookingSetFieldOfView;
    public bool EnableSetFieldOfView;
    public bool FixLowFovScene;
    public bool DisableFog;
    public float FieldOfView;
    public int TargetFrameRate;
    public bool HookingOpenTeam;
    public bool RemoveOpenTeamProgress;
    public bool HookingMickyWonderPartner2;
    public bool Reserved;
}