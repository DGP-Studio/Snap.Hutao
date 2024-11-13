// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;

namespace Snap.Hutao.Service.Game;

internal sealed partial class LaunchOptionsIslandFeatureStateMachine : ObservableObject
{
    private const bool T = true;
    private const bool F = false;

    private bool canInputTargetFov;
    private bool canToggleSetFovHotSwitch;
    private bool canToggleSetFovColdSwitch;
    private bool canToggleFixLowFovHotSwitch;
    private bool canToggleDisableFogHotSwitch;
    private bool canToggleTeamHotSwitch;
    private bool canToggleTeamColdSwitch;
    private bool canToggleLetMeInColdSwitch;
    private bool canInputTargetFps;
    private bool canToggleSetFpsHotSwitch;

    public LaunchOptionsIslandFeatureStateMachine(LaunchOptions options)
    {
        Update(options);
    }

    public bool CanInputTargetFov { get => canInputTargetFov; set => SetProperty(ref canInputTargetFov, value); }

    public bool CanToggleSetFovHotSwitch { get => canToggleSetFovHotSwitch; set => SetProperty(ref canToggleSetFovHotSwitch, value); }

    public bool CanToggleSetFovColdSwitch { get => canToggleSetFovColdSwitch; set => SetProperty(ref canToggleSetFovColdSwitch, value); }

    public bool CanToggleFixLowFovHotSwitch { get => canToggleFixLowFovHotSwitch; set => SetProperty(ref canToggleFixLowFovHotSwitch, value); }

    public bool CanToggleDisableFogHotSwitch { get => canToggleDisableFogHotSwitch; set => SetProperty(ref canToggleDisableFogHotSwitch, value); }

    public bool CanToggleTeamHotSwitch { get => canToggleTeamHotSwitch; set => SetProperty(ref canToggleTeamHotSwitch, value); }

    public bool CanToggleTeamColdSwitch { get => canToggleTeamColdSwitch; set => SetProperty(ref canToggleTeamColdSwitch, value); }

    public bool CanToggleLetMeInColdSwitch { get => canToggleLetMeInColdSwitch; set => SetProperty(ref canToggleLetMeInColdSwitch, value); }

    public bool CanInputTargetFps { get => canInputTargetFps; set => SetProperty(ref canInputTargetFps, value); }

    public bool CanToggleSetFpsHotSwitch { get => canToggleSetFpsHotSwitch; set => SetProperty(ref canToggleSetFpsHotSwitch, value); }

    public void Update(LaunchOptions options)
    {
        (
            CanInputTargetFov,
            CanToggleSetFovHotSwitch,
            CanToggleSetFovColdSwitch,
            CanToggleFixLowFovHotSwitch,
            CanToggleDisableFogHotSwitch,
            CanToggleTeamHotSwitch,
            CanToggleTeamColdSwitch,
            CanToggleLetMeInColdSwitch,
            CanInputTargetFps,
            CanToggleSetFpsHotSwitch) =
            (options.IsIslandEnabled, options.IsGameRunning, options.HookingSetFieldOfView, options.IsSetFieldOfViewEnabled, options.HookingOpenTeam, options.IsSetTargetFrameRateEnabled) switch
            {
                (F, _, _, _, _, _) => (F, F, F, F, F, F, F, F, F, F),
                (T, F, F, _, F, T) => (F, F, T, F, F, F, T, T, T, T),
                (T, F, F, _, T, T) => (F, F, T, F, F, T, T, T, T, T),
                (T, F, F, _, F, F) => (F, F, T, F, F, F, T, T, F, T),
                (T, F, F, _, T, F) => (F, F, T, F, F, T, T, T, F, T),
                (T, F, T, F, F, _) => (F, T, T, F, F, F, T, T, F, F),
                (T, F, T, F, T, _) => (F, T, T, F, F, T, T, T, F, F),
                (T, F, T, T, F, F) => (T, T, T, T, T, F, T, T, F, T),
                (T, F, T, T, F, T) => (T, T, T, T, T, F, T, T, T, T),
                (T, F, T, T, T, F) => (T, T, T, T, T, T, T, T, F, T),
                (T, F, T, T, T, T) => (T, T, T, T, T, T, T, T, T, T),
                (T, T, F, _, F, F) => (F, F, F, F, F, F, F, F, F, T),
                (T, T, F, _, F, T) => (F, F, F, F, F, F, F, F, T, T),
                (T, T, F, _, T, F) => (F, F, F, F, F, T, F, F, F, T),
                (T, T, F, _, T, T) => (F, F, F, F, F, T, F, F, T, T),
                (T, T, T, F, F, _) => (F, T, F, F, F, F, F, F, F, F),
                (T, T, T, F, T, _) => (F, T, F, F, F, T, F, F, F, F),
                (T, T, T, T, F, F) => (T, T, F, T, T, F, F, F, F, T),
                (T, T, T, T, F, T) => (T, T, F, T, T, F, F, F, T, T),
                (T, T, T, T, T, F) => (T, T, F, T, T, T, F, F, F, T),
                (T, T, T, T, T, T) => (T, T, F, T, T, T, F, F, T, T),
            };
    }
}