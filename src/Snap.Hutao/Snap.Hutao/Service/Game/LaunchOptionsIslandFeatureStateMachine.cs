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

    [ObservableProperty]
    public partial bool CanInputTargetFov { get; set; }

    [ObservableProperty]
    public partial bool CanToggleSetFovHotSwitch { get; set; }

    [ObservableProperty]
    public partial bool CanToggleSetFovColdSwitch { get; set; }

    [ObservableProperty]
    public partial bool CanToggleFixLowFovHotSwitch { get; set; }

    [ObservableProperty]
    public partial bool CanToggleDisableFogHotSwitch { get; set; }

    [ObservableProperty]
    public partial bool CanToggleTeamHotSwitch { get; set; }

    [ObservableProperty]
    public partial bool CanToggleTeamColdSwitch { get; set; }

    [ObservableProperty]
    public partial bool CanToggleLetMeInColdSwitch { get; set; }

    [ObservableProperty]
    public partial bool CanInputTargetFps { get; set; }

    [ObservableProperty]
    public partial bool CanToggleSetFpsHotSwitch { get; set; }

    [ObservableProperty]
    public partial bool CanToggleHideQuestBannerColdSwitch { get; set; }

    [ObservableProperty]
    public partial bool CanToggleHideQuestBannerHotSwitch { get; set; }

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

        (CanToggleHideQuestBannerColdSwitch, CanToggleHideQuestBannerHotSwitch) = (options.IsIslandEnabled, options.IsGameRunning, options.HookingSetupQuestBanner) switch
        {
            (F, _, _) => (F, F),
            (T, F, F) => (T, F),
            (T, F, T) => (T, T),
            (T, T, F) => (F, F),
            (T, T, T) => (F, T),
        };
    }
}