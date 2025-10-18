// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Service.Game.Island;

internal struct IslandEnvironment
{
#pragma warning disable CS0649
    public IslandEnvironmentView View;
#pragma warning restore CS0649
    public IslandFunctionOffsets FunctionOffsets;

    public BOOL EnableSetFieldOfView;
    public float FieldOfView;
    public BOOL FixLowFovScene;
    public BOOL DisableFog;
    public BOOL EnableSetTargetFrameRate;
    public int TargetFrameRate;
    public BOOL RemoveOpenTeamProgress;
    public BOOL HideQuestBanner;
    public BOOL DisableEventCameraMove;
    public BOOL DisableShowDamageText;
    public BOOL UsingTouchScreen;
    public BOOL RedirectCombineEntry;
    public BOOL ResinListItemId000106Allowed;
    public BOOL ResinListItemId000201Allowed;
    public BOOL ResinListItemId107009Allowed;
    public BOOL ResinListItemId107012Allowed;
    public BOOL ResinListItemId220007Allowed;
}