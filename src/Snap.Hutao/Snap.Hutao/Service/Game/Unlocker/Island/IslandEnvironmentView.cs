// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Service.Game.Unlocker.Island;

internal struct IslandEnvironmentView
{
    public IslandState State;
    public WIN32_ERROR LastError;

    public float FieldOfView;
    public int TargetFrameRate;
    public bool DisableFog;
    public bool FixLowFovScene;
    public bool RemoveOpenTeamProgress;
    public bool LoopAdjustFpsOnly;

    public uint FunctionOffsetMickeyWonderMethod;
    public uint FunctionOffsetMickeyWonderMethodPartner;
    public uint FunctionOffsetMickeyWonderMethodPartner2;
    public uint FunctionOffsetSetFieldOfView;
    public uint FunctionOffsetSetEnableFogRendering;
    public uint FunctionOffsetSetTargetFrameRate;
    public uint FunctionOffsetOpenTeam;
    public uint FunctionOffsetOpenTeamPageAccordingly;

    public float DebugOriginalFieldOfView;
    public int DebugOpenTeamCount;
}