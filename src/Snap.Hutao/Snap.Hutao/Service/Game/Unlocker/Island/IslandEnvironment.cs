 // Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Service.Game.Unlocker.Island;

internal struct IslandEnvironment
{
    public IslandState State;
    public WIN32_ERROR LastError;

    public IslandFunctionOffsets FunctionOffsets;

    public BOOL HookingSetFieldOfView;
    public BOOL EnableSetFieldOfView;
    public float FieldOfView;
    public BOOL FixLowFovScene;
    public BOOL DisableFog;
    public BOOL EnableSetTargetFrameRate;
    public int TargetFrameRate;
    public BOOL HookingOpenTeam;
    public BOOL RemoveOpenTeamProgress;
    public BOOL HookingMickyWonderPartner2;
}