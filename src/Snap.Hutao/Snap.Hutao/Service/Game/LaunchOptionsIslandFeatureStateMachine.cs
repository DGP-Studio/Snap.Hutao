// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;

namespace Snap.Hutao.Service.Game;

internal sealed partial class LaunchOptionsIslandFeatureStateMachine : ObservableObject
{
    private const bool T = true;
    private const bool F = false;

    public LaunchOptionsIslandFeatureStateMachine(LaunchOptions options)
    {
        Update(options);
    }

    public bool CanInputTargetFov { get; set => SetProperty(ref field, value); }

    public bool CanToggleSetFovHotSwitch { get; set => SetProperty(ref field, value); }

    public bool CanToggleSetFovColdSwitch { get; set => SetProperty(ref field, value); }

    public bool CanToggleFixLowFovHotSwitch { get; set => SetProperty(ref field, value); }

    public bool CanToggleDisableFogHotSwitch { get; set => SetProperty(ref field, value); }

    public bool CanToggleTeamHotSwitch { get; set => SetProperty(ref field, value); }

    public bool CanToggleTeamColdSwitch { get; set => SetProperty(ref field, value); }

    public bool CanToggleLetMeInColdSwitch { get; set => SetProperty(ref field, value); }

    public bool CanInputTargetFps { get; set => SetProperty(ref field, value); }

    public bool CanToggleSetFpsHotSwitch { get; set => SetProperty(ref field, value); }

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