// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Windowing;
using Snap.Hutao.Core.Json;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Abstraction.Property;
using Snap.Hutao.Service.Game.Launching;
using Snap.Hutao.Service.Game.Launching.Handler;
using Snap.Hutao.Service.Game.PathAbstraction;
using Snap.Hutao.Win32;
using System.Collections.Immutable;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Service.Game;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class LaunchOptions : DbStoreOptions,
    IRestrictedGamePathAccess,
    IRecipient<LaunchExecutionProcessStatusChangedMessage>
{
    private readonly ITaskContext taskContext;
    private Fields fields;

    public LaunchOptions(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();

        // Batch initialization, boost up performance
        InitializeOptions(entry => entry.Key.StartsWith("Launch."), (key, value) =>
        {
            _ = key switch
            {
                SettingEntry.LaunchUsingHoyolabAccount => InitializeNullableBooleanValue(ref fields.UsingHoyolabAccount, value),
                SettingEntry.LaunchAreCommandLineArgumentsEnabled => InitializeNullableBooleanValue(ref fields.AreCommandLineArgumentsEnabled, value),
                SettingEntry.LaunchIsFullScreen => InitializeNullableBooleanValue(ref fields.IsFullScreen, value),
                SettingEntry.LaunchIsBorderless => InitializeNullableBooleanValue(ref fields.IsBorderless, value),
                SettingEntry.LaunchIsExclusive => InitializeNullableBooleanValue(ref fields.IsExclusive, value),
                SettingEntry.LaunchScreenWidth => InitializeNullableInt32Value(ref fields.ScreenWidth, value),
                SettingEntry.LaunchIsScreenWidthEnabled => InitializeNullableBooleanValue(ref fields.IsScreenWidthEnabled, value),
                SettingEntry.LaunchScreenHeight => InitializeNullableInt32Value(ref fields.ScreenHeight, value),
                SettingEntry.LaunchIsScreenHeightEnabled => InitializeNullableBooleanValue(ref fields.IsScreenHeightEnabled, value),
                SettingEntry.LaunchIsMonitorEnabled => InitializeNullableBooleanValue(ref fields.IsMonitorEnabled, value),
                SettingEntry.LaunchIsWindowsHDREnabled => InitializeNullableBooleanValue(ref fields.IsWindowsHDREnabled, value),
                SettingEntry.LaunchUsingStarwardPlayTimeStatistics => InitializeNullableBooleanValue(ref fields.UsingStarwardPlayTimeStatistics, value),
                SettingEntry.LaunchUsingBetterGenshinImpactAutomation => InitializeNullableBooleanValue(ref fields.UsingBetterGenshinImpactAutomation, value),
                SettingEntry.LaunchSetDiscordActivityWhenPlaying => InitializeNullableBooleanValue(ref fields.SetDiscordActivityWhenPlaying, value),
                SettingEntry.LaunchIsIslandEnabled => InitializeNullableBooleanValue(ref fields.IsIslandEnabled, value),
                SettingEntry.LaunchIsSetFieldOfViewEnabled => InitializeNullableBooleanValue(ref fields.IsSetFieldOfViewEnabled, value),
                SettingEntry.LaunchTargetFov => InitializeNullableFloatValue(ref fields.TargetFov, value),
                SettingEntry.LaunchFixLowFovScene => InitializeNullableBooleanValue(ref fields.FixLowFovScene, value),
                SettingEntry.LaunchDisableFog => InitializeNullableBooleanValue(ref fields.DisableFog, value),
                SettingEntry.LaunchIsSetTargetFrameRateEnabled => InitializeNullableBooleanValue(ref fields.IsSetTargetFrameRateEnabled, value),
                SettingEntry.LaunchTargetFps => InitializeNullableInt32Value(ref fields.TargetFps, value),
                SettingEntry.LaunchRemoveOpenTeamProgress => InitializeNullableBooleanValue(ref fields.RemoveOpenTeamProgress, value),
                SettingEntry.LaunchHideQuestBanner => InitializeNullableBooleanValue(ref fields.HideQuestBanner, value),
                _ => default,
            };
        });

        serviceProvider.GetRequiredService<IMessenger>().Register(this);

        static Void InitializeNullableBooleanValue(ref bool? storage, string? value)
        {
            if (value is not null)
            {
                _ = bool.TryParse(value, out bool result);
                storage = result;
            }

            return default;
        }

        static Void InitializeNullableInt32Value(ref int? storage, string? value)
        {
            if (value is not null)
            {
                _ = int.TryParse(value, CultureInfo.InvariantCulture, out int result);
                storage = result;
            }

            return default;
        }

        static Void InitializeNullableFloatValue(ref float? storage, string? value)
        {
            if (value is not null)
            {
                _ = float.TryParse(value, CultureInfo.InvariantCulture, out float result);
                storage = result;
            }

            return default;
        }
    }

    string IRestrictedGamePathAccess.GamePath { get => GamePath; set => GamePath.Value = value; }

    [field: MaybeNull]
    public DbProperty<string> GamePath { get => field ??= CreateProperty(SettingEntry.GamePath, string.Empty); }

    ImmutableArray<GamePathEntry> IRestrictedGamePathAccess.GamePathEntries { get => GamePathEntries.Value; set => GamePathEntries.Value = value; }

    [field: MaybeNull]
    public DbProperty<ImmutableArray<GamePathEntry>> GamePathEntries { get => field ??= CreatePropertyForStructUsingJson(SettingEntry.GamePathEntries, ImmutableArray<GamePathEntry>.Empty); }

    public AsyncReaderWriterLock GamePathLock { get; } = new();

    [field: MaybeNull]
    public DbProperty<bool> UsingHoyolabAccount { get => field ??= CreateProperty(SettingEntry.LaunchUsingHoyolabAccount, false); }

    public bool AreCommandLineArgumentsEnabled
    {
        get => GetOption(ref fields.AreCommandLineArgumentsEnabled, SettingEntry.LaunchAreCommandLineArgumentsEnabled, true);
        set
        {
            if (SetOption(ref fields.AreCommandLineArgumentsEnabled, SettingEntry.LaunchAreCommandLineArgumentsEnabled, value))
            {
                if (!value)
                {
                    UsingHoyolabAccount.Value = false;
                }
            }
        }
    }

    [field: MaybeNull]
    public DbProperty<bool> IsFullScreen { get => field ??= CreateProperty(SettingEntry.LaunchIsFullScreen, false); }

    [field: MaybeNull]
    public DbProperty<bool> IsBorderless { get => field ??= CreateProperty(SettingEntry.LaunchIsBorderless, false); }

    [field: MaybeNull]
    public DbProperty<bool> IsExclusive { get => field ??= CreateProperty(SettingEntry.LaunchIsExclusive, false); }

    [field: MaybeNull]
    public DbProperty<bool> IsScreenWidthEnabled { get => field ??= CreateProperty(SettingEntry.LaunchIsScreenWidthEnabled, true); }

    [field: MaybeNull]
    public DbProperty<int> ScreenWidth { get => field ??= CreateProperty(SettingEntry.LaunchScreenWidth, DisplayArea.Primary.OuterBounds.Width); }

    [field: MaybeNull]
    public DbProperty<bool> IsScreenHeightEnabled { get => field ??= CreateProperty(SettingEntry.LaunchIsScreenHeightEnabled, true); }

    [field: MaybeNull]
    public DbProperty<int> ScreenHeight { get => field ??= CreateProperty(SettingEntry.LaunchScreenHeight, DisplayArea.Primary.OuterBounds.Height); }

    [field: MaybeNull]
    public DbProperty<bool> IsMonitorEnabled { get => field ??= CreateProperty(SettingEntry.LaunchIsMonitorEnabled, true); }

    public ImmutableArray<NameValue<int>> Monitors { get; } = InitializeMonitors();

    [NotNull]
    public NameValue<int>? Monitor
    {
        get
        {
            return GetOption(ref field, SettingEntry.LaunchMonitor, index => Monitors[RestrictIndex(Monitors, index)], Monitors[0]);

            static int RestrictIndex(ImmutableArray<NameValue<int>> monitors, string index)
            {
                return Math.Clamp(int.Parse(index, CultureInfo.InvariantCulture) - 1, 0, monitors.Length - 1);
            }
        }

        set
        {
            if (value is not null)
            {
                SetOption(ref field, SettingEntry.LaunchMonitor, value, static selected => selected.Value.ToString(CultureInfo.InvariantCulture));
            }
        }
    }

    [field: MaybeNull]
    public DbProperty<bool> IsPlatformTypeEnabled { get => field ??= CreateProperty(SettingEntry.LaunchIsPlatformTypeEnabled, false); }

    public ImmutableArray<NameValue<PlatformType>> PlatformTypes { get; } = ImmutableCollectionsNameValue.FromEnum<PlatformType>();

    [field: MaybeNull]
    public DbProperty<PlatformType> PlatformType { get => field ??= CreateProperty(SettingEntry.LaunchPlatformType, Model.Intrinsic.PlatformType.PC); }

    [field: MaybeNull]
    public DbProperty<bool> IsWindowsHDREnabled { get => field ??= CreateProperty(SettingEntry.LaunchIsWindowsHDREnabled, false); }

    [field: MaybeNull]
    public DbProperty<bool> UsingStarwardPlayTimeStatistics { get => field ??= CreateProperty(SettingEntry.LaunchUsingStarwardPlayTimeStatistics, false); }

    [field: MaybeNull]
    public DbProperty<bool> UsingBetterGenshinImpactAutomation { get => field ??= CreateProperty(SettingEntry.LaunchUsingBetterGenshinImpactAutomation, false); }

    [field: MaybeNull]
    public DbProperty<bool> SetDiscordActivityWhenPlaying { get => field ??= CreateProperty(SettingEntry.LaunchSetDiscordActivityWhenPlaying, false); }

    [field: MaybeNull]
    public DbProperty<bool> IsIslandEnabled { get => field ??= CreateProperty(SettingEntry.LaunchIsIslandEnabled, false); }

    [field: MaybeNull]
    public DbProperty<bool> IsSetFieldOfViewEnabled { get => field ??= CreateProperty(SettingEntry.LaunchIsSetFieldOfViewEnabled, true); }

    [field: MaybeNull]
    public DbProperty<float> TargetFov { get => field ??= CreateProperty(SettingEntry.LaunchTargetFov, 45f); }

    [field: MaybeNull]
    public DbProperty<bool> FixLowFovScene { get => field ??= CreateProperty(SettingEntry.LaunchFixLowFovScene, true); }

    [field: MaybeNull]
    public DbProperty<bool> DisableFog { get => field ??= CreateProperty(SettingEntry.LaunchDisableFog, false); }

    [field: MaybeNull]
    public DbProperty<bool> IsSetTargetFrameRateEnabled { get => field ??= CreateProperty(SettingEntry.LaunchIsSetTargetFrameRateEnabled, true); }

    [field: MaybeNull]
    public DbProperty<int> TargetFps { get => field ??= CreateProperty(SettingEntry.LaunchTargetFps, InitializeTargetFpsWithScreenFps); }

    [field: MaybeNull]
    public DbProperty<bool> RemoveOpenTeamProgress { get => field ??= CreateProperty(SettingEntry.LaunchRemoveOpenTeamProgress, false); }

    [field: MaybeNull]
    public DbProperty<bool> HideQuestBanner { get => field ??= CreateProperty(SettingEntry.LaunchHideQuestBanner, false); }

    [field: MaybeNull]
    public DbProperty<bool> DisableEventCameraMove { get => field ??= CreateProperty(SettingEntry.LaunchDisableEventCameraMove, false); }

    [field: MaybeNull]
    public DbProperty<bool> DisableShowDamageText { get => field ??= CreateProperty(SettingEntry.LaunchDisableShowDamageText, false); }

    [field: MaybeNull]
    public DbProperty<bool> UsingTouchScreen { get => field ??= CreateProperty(SettingEntry.LaunchUsingTouchScreen, false); }

    [field: MaybeNull]
    public DbProperty<bool> RedirectCombineEntry { get => field ??= CreateProperty(SettingEntry.LaunchRedirectCombineEntry, false); }

    [field: MaybeNull]
    public DbProperty<ImmutableArray<AspectRatio>> AspectRatios { get => field ??= CreatePropertyForStructUsingJson(SettingEntry.AspectRatios, ImmutableArray<AspectRatio>.Empty); }

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

    public bool UsingOverlay
    {
        get => GetOption(ref fields.UsingOverlay, SettingEntry.LaunchUsingOverlay, false);
        set => SetOption(ref fields.UsingOverlay, SettingEntry.LaunchUsingOverlay, value);
    }

#pragma warning disable CA1822
    public bool IsGameRunning { get => LaunchExecutionEnsureGameNotRunningHandler.IsGameRunning(); }
#pragma warning restore CA1822

    public void Receive(LaunchExecutionProcessStatusChangedMessage message)
    {
        taskContext.InvokeOnMainThread(() =>
        {
            OnPropertyChanged(nameof(IsGameRunning));
        });
    }

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

    [StructLayout(LayoutKind.Auto)]
    private struct Fields
    {
        public ImmutableArray<GamePathEntry>? GamePathEntries;
        public ImmutableArray<AspectRatio>? AspectRatios;

        public bool? UsingHoyolabAccount;
        public bool? AreCommandLineArgumentsEnabled;
        public bool? IsFullScreen;
        public bool? IsBorderless;
        public bool? IsExclusive;
        public int? ScreenWidth;
        public bool? IsScreenWidthEnabled;
        public int? ScreenHeight;
        public bool? IsScreenHeightEnabled;
        public bool? IsIslandEnabled;
        public bool? IsSetFieldOfViewEnabled;
        public float? TargetFov;
        public bool? FixLowFovScene;
        public bool? DisableFog;
        public bool? IsSetTargetFrameRateEnabled;
        public int? TargetFps;
        public bool? RemoveOpenTeamProgress;
        public bool? HideQuestBanner;
        public bool? DisableEventCameraMove;
        public bool? DisableShowDamageText;
        public bool? UsingTouchScreen;
        public bool? RedirectCombineEntry;
        public bool? UsingOverlay;
        public bool? IsMonitorEnabled;
        public PlatformType? PlatformType;
        public bool? IsPlatformTypeEnabled;
        public bool? IsWindowsHDREnabled;
        public bool? UsingStarwardPlayTimeStatistics;
        public bool? UsingBetterGenshinImpactAutomation;
        public bool? SetDiscordActivityWhenPlaying;
    }
}