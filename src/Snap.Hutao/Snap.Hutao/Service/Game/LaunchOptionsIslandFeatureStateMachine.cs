// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;

namespace Snap.Hutao.Service.Game;

internal sealed partial class LaunchOptionsIslandFeatureStateMachine : ObservableObject
{
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
                (false, _, _, _, _, _) => (false, false, false, false, false, false, false, false, false, false),
                (true, false, false, _, false, true) => (false, false, true, false, false, false, true, true, true, true),
                (true, false, false, _, true, true) => (false, false, true, false, false, true, true, true, true, true),
                (true, false, false, _, false, false) => (false, false, true, false, false, false, true, true, false, true),
                (true, false, false, _, true, false) => (false, false, true, false, false, true, true, true, false, true),
                (true, false, true, false, false, _) => (false, true, true, false, false, false, true, true, false, false),
                (true, false, true, false, true, _) => (false, true, true, false, false, true, true, true, false, false),
                (true, false, true, true, false, false) => (true, true, true, true, true, false, true, true, false, true),
                (true, false, true, true, false, true) => (true, true, true, true, true, false, true, true, true, true),
                (true, false, true, true, true, false) => (true, true, true, true, true, true, true, true, false, true),
                (true, false, true, true, true, true) => (true, true, true, true, true, true, true, true, true, true),
                (true, true, false, _, false, false) => (false, false, false, false, false, false, false, false, false, true),
                (true, true, false, _, false, true) => (false, false, false, false, false, false, false, false, true, true),
                (true, true, false, _, true, false) => (false, false, false, false, false, true, false, false, false, true),
                (true, true, false, _, true, true) => (false, false, false, false, false, true, false, false, true, true),
                (true, true, true, false, false, _) => (false, true, false, false, false, false, false, false, false, false),
                (true, true, true, false, true, _) => (false, true, false, false, false, true, false, false, false, false),
                (true, true, true, true, false, _) => (true, true, false, true, true, false, false, false, false, true),
                (true, true, true, true, true, false) => (true, true, false, true, true, true, false, false, false, true),
                (true, true, true, true, true, true) => (true, true, false, true, true, true, false, false, true, true),
            };
    }
}