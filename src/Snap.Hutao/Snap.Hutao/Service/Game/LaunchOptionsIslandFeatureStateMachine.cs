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

    [ObservableProperty]
    public partial bool CanToggleEventCameraMoveColdSwitch { get; set; }

    [ObservableProperty]
    public partial bool CanToggleEventCameraMoveHotSwitch { get; set; }

    public void Update(LaunchOptions options)
    {
        (CanToggleSetFovColdSwitch, CanToggleSetFovHotSwitch) = StandardColdHotSwitchStatement(options.IsIslandEnabled, options.IsGameRunning, options.HookingSetFieldOfView);
        (CanInputTargetFov, CanToggleFixLowFovHotSwitch, CanToggleDisableFogHotSwitch, CanInputTargetFps, CanToggleSetFpsHotSwitch) =
            (options.IsIslandEnabled, options.HookingSetFieldOfView, options.IsSetFieldOfViewEnabled, options.IsSetTargetFrameRateEnabled) switch
            {
                (F, _, _, _) => (F, F, F, F, F),
                (T, T, F, _) => (F, F, F, F, F),
                (T, F, _, T) => (F, F, F, T, T),
                (T, F, _, F) => (F, F, F, F, T),
                (T, T, T, F) => (T, T, T, F, T),
                (T, T, T, T) => (T, T, T, T, T),
            };

        CanToggleLetMeInColdSwitch = StandardColdSwitchStatement(options.IsIslandEnabled, options.IsGameRunning);
        (CanToggleTeamColdSwitch, CanToggleTeamHotSwitch) = StandardColdHotSwitchStatement(options.IsIslandEnabled, options.IsGameRunning, options.HookingOpenTeam);
        (CanToggleHideQuestBannerColdSwitch, CanToggleHideQuestBannerHotSwitch) = StandardColdHotSwitchStatement(options.IsIslandEnabled, options.IsGameRunning, options.HookingSetupQuestBanner);
        (CanToggleEventCameraMoveColdSwitch, CanToggleEventCameraMoveHotSwitch) = StandardColdHotSwitchStatement(options.IsIslandEnabled, options.IsGameRunning, options.HookingEventCameraMove);
    }

    private static (bool Cold, bool Hot) StandardColdHotSwitchStatement(bool isIslandEnabled, bool isGameRunning, bool hooking)
    {
        return (isIslandEnabled, isGameRunning, hooking) switch
        {
            (F, _, _) => (F, F),
            (T, F, F) => (T, F),
            (T, F, T) => (T, T),
            (T, T, F) => (F, F),
            (T, T, T) => (F, T),
        };
    }

    private static bool StandardColdSwitchStatement(bool isIslandEnabled, bool isGameRunning)
    {
        return (isIslandEnabled, isGameRunning) switch
        {
            (T, F) => T,
            _ => F,
        };
    }
}