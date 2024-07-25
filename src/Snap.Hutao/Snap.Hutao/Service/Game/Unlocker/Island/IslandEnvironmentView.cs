// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Service.Game.Unlocker.Island;

internal struct IslandEnvironmentView
{
    public nuint Reserved1;
    public int Reserved2;
    public IslandState State;
    public WIN32_ERROR LastError;
    public int Reserved3;
    public float FieldOfView;
    public int TargetFrameRate;
    public bool DisableFog;
    public nuint FunctionOffsetFieldOfView;
    public nuint FunctionOffsetTargetFrameRate;
    public nuint FunctionOffsetFog;
}