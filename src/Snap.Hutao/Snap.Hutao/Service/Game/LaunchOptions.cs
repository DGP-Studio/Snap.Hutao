// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Property;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Game.FileSystem;
using Snap.Hutao.Service.Game.PathAbstraction;
using Snap.Hutao.Win32;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Game;

[GeneratedConstructor(CallBaseConstructor = true)]
[Service(ServiceLifetime.Singleton)]
internal sealed partial class LaunchOptions : DbStoreOptions, IRestrictedGamePathAccess
{
    [field: MaybeNull]
    public static IObservableProperty<bool> IsGameRunning { get => field ??= GameLifeCycle.IsGameRunningProperty; }

    [field: MaybeNull]
    public static IReadOnlyObservableProperty<bool> CanKillGameProcess { get => field ??= Property.Observe(IsGameRunning, value => HutaoRuntime.IsProcessElevated && value); }

    public AsyncReaderWriterLock GamePathLock { get; } = new();

    [field: MaybeNull]
    public IObservableProperty<GamePathEntry?> GamePathEntry { get => field ??= CreateProperty(SettingEntry.GamePath, string.Empty).AsNullableSelection(GamePathEntries.Value, static entry => entry?.Path ?? string.Empty, StringComparer.OrdinalIgnoreCase).Debug("GamePathEntry"); }

    [field: MaybeNull]
    public IObservableProperty<ImmutableArray<GamePathEntry>> GamePathEntries { get => field ??= CreatePropertyForStructUsingJson(SettingEntry.GamePathEntries, ImmutableArray<GamePathEntry>.Empty); }

    [field: MaybeNull]
    public IObservableProperty<bool> UsingHoyolabAccount { get => field ??= CreateProperty(SettingEntry.LaunchUsingHoyolabAccount, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> AreCommandLineArgumentsEnabled { get => field ??= CreateProperty(SettingEntry.LaunchAreCommandLineArgumentsEnabled, true).AlsoSetFalseWhenFalse(UsingHoyolabAccount); }

    [field: MaybeNull]
    public IObservableProperty<bool> IsFullScreen { get => field ??= CreateProperty(SettingEntry.LaunchIsFullScreen, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> IsBorderless { get => field ??= CreateProperty(SettingEntry.LaunchIsBorderless, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> IsExclusive { get => field ??= CreateProperty(SettingEntry.LaunchIsExclusive, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> IsScreenWidthEnabled { get => field ??= CreateProperty(SettingEntry.LaunchIsScreenWidthEnabled, true); }

    [field: MaybeNull]
    public IObservableProperty<int> ScreenWidth { get => field ??= CreateProperty(SettingEntry.LaunchScreenWidth, DisplayArea.Primary.OuterBounds.Width); }

    [field: MaybeNull]
    public IObservableProperty<bool> IsScreenHeightEnabled { get => field ??= CreateProperty(SettingEntry.LaunchIsScreenHeightEnabled, true); }

    [field: MaybeNull]
    public IObservableProperty<int> ScreenHeight { get => field ??= CreateProperty(SettingEntry.LaunchScreenHeight, DisplayArea.Primary.OuterBounds.Height); }

    [field: MaybeNull]
    public IObservableProperty<bool> IsMonitorEnabled { get => field ??= CreateProperty(SettingEntry.LaunchIsMonitorEnabled, true); }

    public ImmutableArray<NameValue<int>> Monitors { get; } = InitializeMonitors();

    [field: MaybeNull]
    public IObservableProperty<NameValue<int>?> Monitor { get => field ??= CreatePropertyForSelectedOneBasedIndex(SettingEntry.LaunchMonitor, Monitors); }

    [field: MaybeNull]
    public IObservableProperty<bool> IsPlatformTypeEnabled { get => field ??= CreateProperty(SettingEntry.LaunchIsPlatformTypeEnabled, false); }

    public ImmutableArray<NameValue<PlatformType>> PlatformTypes { get; } = ImmutableCollectionsNameValue.FromEnum<PlatformType>();

    [field: MaybeNull]
    public IObservableProperty<PlatformType> PlatformType { get => field ??= CreateProperty(SettingEntry.LaunchPlatformType, Model.Intrinsic.PlatformType.PC); }

    [field: MaybeNull]
    public IObservableProperty<bool> IsWindowsHDREnabled { get => field ??= CreateProperty(SettingEntry.LaunchIsWindowsHDREnabled, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> UsingStarwardPlayTimeStatistics { get => field ??= CreateProperty(SettingEntry.LaunchUsingStarwardPlayTimeStatistics, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> UsingBetterGenshinImpactAutomation { get => field ??= CreateProperty(SettingEntry.LaunchUsingBetterGenshinImpactAutomation, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> SetDiscordActivityWhenPlaying { get => field ??= CreateProperty(SettingEntry.LaunchSetDiscordActivityWhenPlaying, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> IsIslandEnabled { get => field ??= CreateProperty(SettingEntry.LaunchIsIslandEnabled, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> IsSetFieldOfViewEnabled { get => field ??= CreateProperty(SettingEntry.LaunchIsSetFieldOfViewEnabled, true); }

    [field: MaybeNull]
    public IObservableProperty<float> TargetFov { get => field ??= CreateProperty(SettingEntry.LaunchTargetFov, 45f); }

    [field: MaybeNull]
    public IObservableProperty<bool> FixLowFovScene { get => field ??= CreateProperty(SettingEntry.LaunchFixLowFovScene, true); }

    [field: MaybeNull]
    public IObservableProperty<bool> DisableFog { get => field ??= CreateProperty(SettingEntry.LaunchDisableFog, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> IsSetTargetFrameRateEnabled { get => field ??= CreateProperty(SettingEntry.LaunchIsSetTargetFrameRateEnabled, true); }

    [field: MaybeNull]
    public IObservableProperty<int> TargetFps { get => field ??= CreateProperty(SettingEntry.LaunchTargetFps, InitializeTargetFpsWithScreenFps); }

    [field: MaybeNull]
    public IObservableProperty<bool> RemoveOpenTeamProgress { get => field ??= CreateProperty(SettingEntry.LaunchRemoveOpenTeamProgress, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> HideQuestBanner { get => field ??= CreateProperty(SettingEntry.LaunchHideQuestBanner, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> DisableEventCameraMove { get => field ??= CreateProperty(SettingEntry.LaunchDisableEventCameraMove, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> DisableShowDamageText { get => field ??= CreateProperty(SettingEntry.LaunchDisableShowDamageText, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> UsingTouchScreen { get => field ??= CreateProperty(SettingEntry.LaunchUsingTouchScreen, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> RedirectCombineEntry { get => field ??= CreateProperty(SettingEntry.LaunchRedirectCombineEntry, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> ResinListItemId000106Allowed { get => field ??= CreateProperty(SettingEntry.LaunchResinListItemId000106Allowed, true); }

    [field: MaybeNull]
    public IObservableProperty<bool> ResinListItemId000201Allowed { get => field ??= CreateProperty(SettingEntry.LaunchResinListItemId000201Allowed, true); }

    [field: MaybeNull]
    public IObservableProperty<bool> ResinListItemId107009Allowed { get => field ??= CreateProperty(SettingEntry.LaunchResinListItemId107009Allowed, true); }

    [field: MaybeNull]
    public IObservableProperty<bool> ResinListItemId107012Allowed { get => field ??= CreateProperty(SettingEntry.LaunchResinListItemId107012Allowed, true); }


    [field: MaybeNull]
    public IObservableProperty<bool> ResinListItemId220007Allowed { get => field ??= CreateProperty(SettingEntry.LaunchResinListItemId220007Allowed, true); }

    [field: MaybeNull]
    public IObservableProperty<ImmutableArray<AspectRatio>> AspectRatios { get => field ??= CreatePropertyForStructUsingJson(SettingEntry.AspectRatios, ImmutableArray<AspectRatio>.Empty); }

    public AspectRatio? SelectedAspectRatio
    {
        get;
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                (ScreenWidth.Value, ScreenHeight.Value) = ((int)value.Width, (int)value.Height);
            }
        }
    }

    [field: MaybeNull]
    public IObservableProperty<bool> UsingOverlay { get => field ??= CreateProperty(SettingEntry.LaunchUsingOverlay, false); }

    private static int InitializeTargetFpsWithScreenFps()
    {
        return HutaoNative.Instance.MakeDeviceCapabilities().GetPrimaryScreenVerticalRefreshRate();
    }

    private static ImmutableArray<NameValue<int>> InitializeMonitors()
    {
        ImmutableArray<NameValue<int>>.Builder monitors = ImmutableArray.CreateBuilder<NameValue<int>>();
        try
        {
            // This list can't use foreach
            // https://github.com/microsoft/CsWinRT/issues/747
            IReadOnlyList<DisplayArea> displayAreas = DisplayArea.FindAll();
            for (int i = 0; i < displayAreas.Count; i++)
            {
                DisplayArea displayArea = displayAreas[i];
                int index = i + 1;
                monitors.Add(new($"{displayArea.DisplayId.Value:X8}:{index}", index));
            }
        }
        catch
        {
            monitors.Clear();
        }

        return monitors.ToImmutable();
    }
}