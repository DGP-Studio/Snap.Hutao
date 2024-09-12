﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Service.Game.Unlocker.Island;

internal struct IslandEnvironment
{
    public IslandState State;
    public WIN32_ERROR LastError;

    public float FieldOfView;
    public int TargetFrameRate;
    public bool DisableFog;
    public bool LoopAdjustFpsOnly;

    public uint FunctionOffsetSetFieldOfView;
    public uint FunctionOffsetSetEnableFogRendering;
    public uint FunctionOffsetSetTargetFrameRate;

    public float DebugOriginalFieldOfView;
}