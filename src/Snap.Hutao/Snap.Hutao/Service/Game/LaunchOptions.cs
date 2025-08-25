// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Windowing;
using Snap.Hutao.Core.Json;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Game.Launching;
using Snap.Hutao.Service.Game.Launching.Handler;
using Snap.Hutao.Service.Game.PathAbstraction;
using Snap.Hutao.Win32;
using System.Collections.Immutable;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Service.Game;

[Injection(InjectAs.Singleton)]
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
                SettingEntry.LaunchGameOnAppStartup => InitializeNullableBooleanValue(ref fields.LaunchGameOnAppStartup, value),
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

    [field: AllowNull]
    public string GamePath
    {
        get => GetOption(ref field, SettingEntry.GamePath);
        set => SetOption(ref field, SettingEntry.GamePath, value);
    }

    public ImmutableArray<GamePathEntry> GamePathEntries
    {
        // Because DbStoreOptions can't detect collection change, We use
        // ImmutableArray to imply that the whole list needs to be replaced
        get => GetOption(ref fields.GamePathEntries, SettingEntry.GamePathEntries, raw => JsonSerializer.Deserialize<ImmutableArray<GamePathEntry>>(raw, JsonOptions.Default), []);
        set => SetOption(ref fields.GamePathEntries, SettingEntry.GamePathEntries, value, static v => JsonSerializer.Serialize(v, JsonOptions.Default));
    }

    public AsyncReaderWriterLock GamePathLock { get; } = new();

    public bool UsingHoyolabAccount
    {
        get => GetOption(ref fields.UsingHoyolabAccount, SettingEntry.LaunchUsingHoyolabAccount, false);
        set => SetOption(ref fields.UsingHoyolabAccount, SettingEntry.LaunchUsingHoyolabAccount, value);
    }

    public bool AreCommandLineArgumentsEnabled
    {
        get => GetOption(ref fields.AreCommandLineArgumentsEnabled, SettingEntry.LaunchAreCommandLineArgumentsEnabled, true);
        set
        {
            if (SetOption(ref fields.AreCommandLineArgumentsEnabled, SettingEntry.LaunchAreCommandLineArgumentsEnabled, value))
            {
                if (!value)
                {
                    UsingHoyolabAccount = false;
                }
            }
        }
    }

    public bool IsFullScreen
    {
        get => GetOption(ref fields.IsFullScreen, SettingEntry.LaunchIsFullScreen, false);
        set => SetOption(ref fields.IsFullScreen, SettingEntry.LaunchIsFullScreen, value);
    }

    public bool IsBorderless
    {
        get => GetOption(ref fields.IsBorderless, SettingEntry.LaunchIsBorderless, false);
        set => SetOption(ref fields.IsBorderless, SettingEntry.LaunchIsBorderless, value);
    }

    public bool IsExclusive
    {
        get => GetOption(ref fields.IsExclusive, SettingEntry.LaunchIsExclusive, false);
        set => SetOption(ref fields.IsExclusive, SettingEntry.LaunchIsExclusive, value);
    }

    public int ScreenWidth
    {
        get => GetOption(ref fields.ScreenWidth, SettingEntry.LaunchScreenWidth, DisplayArea.Primary.OuterBounds.Width);
        set => SetOption(ref fields.ScreenWidth, SettingEntry.LaunchScreenWidth, value);
    }

    public bool IsScreenWidthEnabled
    {
        get => GetOption(ref fields.IsScreenWidthEnabled, SettingEntry.LaunchIsScreenWidthEnabled, true);
        set => SetOption(ref fields.IsScreenWidthEnabled, SettingEntry.LaunchIsScreenWidthEnabled, value);
    }

    public int ScreenHeight
    {
        get => GetOption(ref fields.ScreenHeight, SettingEntry.LaunchScreenHeight, DisplayArea.Primary.OuterBounds.Height);
        set => SetOption(ref fields.ScreenHeight, SettingEntry.LaunchScreenHeight, value);
    }

    public bool IsScreenHeightEnabled
    {
        get => GetOption(ref fields.IsScreenHeightEnabled, SettingEntry.LaunchIsScreenHeightEnabled, true);
        set => SetOption(ref fields.IsScreenHeightEnabled, SettingEntry.LaunchIsScreenHeightEnabled, value);
    }

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

    public bool IsMonitorEnabled
    {
        get => GetOption(ref fields.IsMonitorEnabled, SettingEntry.LaunchIsMonitorEnabled, true);
        set => SetOption(ref fields.IsMonitorEnabled, SettingEntry.LaunchIsMonitorEnabled, value);
    }

    public ImmutableArray<NameValue<PlatformType>> PlatformTypes { get; } = ImmutableCollectionsNameValue.FromEnum<PlatformType>();

    public PlatformType PlatformType
    {
        get => GetOption(ref fields.PlatformType, SettingEntry.LaunchPlatformType, Enum.Parse<PlatformType>, PlatformType.PC);
        set => SetOption(ref fields.PlatformType, SettingEntry.LaunchPlatformType, value, EnumToStringOrEmpty);
    }

    public bool IsPlatformTypeEnabled
    {
        get => GetOption(ref fields.IsPlatformTypeEnabled, SettingEntry.LaunchIsPlatformTypeEnabled, false);
        set => SetOption(ref fields.IsPlatformTypeEnabled, SettingEntry.LaunchIsPlatformTypeEnabled, value);
    }

    public bool IsWindowsHDREnabled
    {
        get => GetOption(ref fields.IsWindowsHDREnabled, SettingEntry.LaunchIsWindowsHDREnabled, false);
        set => SetOption(ref fields.IsWindowsHDREnabled, SettingEntry.LaunchIsWindowsHDREnabled, value);
    }

    public bool UsingStarwardPlayTimeStatistics
    {
        get => GetOption(ref fields.UsingStarwardPlayTimeStatistics, SettingEntry.LaunchUsingStarwardPlayTimeStatistics, false);
        set => SetOption(ref fields.UsingStarwardPlayTimeStatistics, SettingEntry.LaunchUsingStarwardPlayTimeStatistics, value);
    }

    public bool UsingBetterGenshinImpactAutomation
    {
        get => GetOption(ref fields.UsingBetterGenshinImpactAutomation, SettingEntry.LaunchUsingBetterGenshinImpactAutomation, false);
        set => SetOption(ref fields.UsingBetterGenshinImpactAutomation, SettingEntry.LaunchUsingBetterGenshinImpactAutomation, value);
    }

    public bool SetDiscordActivityWhenPlaying
    {
        get => GetOption(ref fields.SetDiscordActivityWhenPlaying, SettingEntry.LaunchSetDiscordActivityWhenPlaying, true);
        set => SetOption(ref fields.SetDiscordActivityWhenPlaying, SettingEntry.LaunchSetDiscordActivityWhenPlaying, value);
    }

    public bool LaunchGameOnAppStartup
    {
        get => GetOption(ref fields.LaunchGameOnAppStartup, SettingEntry.LaunchGameOnAppStartup, false);
        set => SetOption(ref fields.LaunchGameOnAppStartup, SettingEntry.LaunchGameOnAppStartup, value);
    }

    public bool IsIslandEnabled
    {
        get => GetOption(ref fields.IsIslandEnabled, SettingEntry.LaunchIsIslandEnabled, false);
        set => SetOption(ref fields.IsIslandEnabled, SettingEntry.LaunchIsIslandEnabled, value);
    }

    public bool IsSetFieldOfViewEnabled
    {
        get => GetOption(ref fields.IsSetFieldOfViewEnabled, SettingEntry.LaunchIsSetFieldOfViewEnabled, true);
        set => SetOption(ref fields.IsSetFieldOfViewEnabled, SettingEntry.LaunchIsSetFieldOfViewEnabled, value);
    }

    public float TargetFov
    {
        get => GetOption(ref fields.TargetFov, SettingEntry.LaunchTargetFov, 45f);
        set => SetOption(ref fields.TargetFov, SettingEntry.LaunchTargetFov, value);
    }

    public bool FixLowFovScene
    {
        get => GetOption(ref fields.FixLowFovScene, SettingEntry.LaunchFixLowFovScene, true);
        set => SetOption(ref fields.FixLowFovScene, SettingEntry.LaunchFixLowFovScene, value);
    }

    public bool DisableFog
    {
        get => GetOption(ref fields.DisableFog, SettingEntry.LaunchDisableFog, false);
        set => SetOption(ref fields.DisableFog, SettingEntry.LaunchDisableFog, value);
    }

    public bool IsSetTargetFrameRateEnabled
    {
        get => GetOption(ref fields.IsSetTargetFrameRateEnabled, SettingEntry.LaunchIsSetTargetFrameRateEnabled, true);
        set => SetOption(ref fields.IsSetTargetFrameRateEnabled, SettingEntry.LaunchIsSetTargetFrameRateEnabled, value);
    }

    public int TargetFps
    {
        get => GetOption(ref fields.TargetFps, SettingEntry.LaunchTargetFps, InitializeScreenFps);
        set => SetOption(ref fields.TargetFps, SettingEntry.LaunchTargetFps, value);
    }

    public bool RemoveOpenTeamProgress
    {
        get => GetOption(ref fields.RemoveOpenTeamProgress, SettingEntry.LaunchRemoveOpenTeamProgress, false);
        set => SetOption(ref fields.RemoveOpenTeamProgress, SettingEntry.LaunchRemoveOpenTeamProgress, value);
    }

    public bool HideQuestBanner
    {
        get => GetOption(ref fields.HideQuestBanner, SettingEntry.LaunchHideQuestBanner, false);
        set => SetOption(ref fields.HideQuestBanner, SettingEntry.LaunchHideQuestBanner, value);
    }

    public bool DisableEventCameraMove
    {
        get => GetOption(ref fields.DisableEventCameraMove, SettingEntry.LaunchDisableEventCameraMove, false);
        set => SetOption(ref fields.DisableEventCameraMove, SettingEntry.LaunchDisableEventCameraMove, value);
    }

    public bool DisableShowDamageText
    {
        get => GetOption(ref fields.DisableShowDamageText, SettingEntry.LaunchDisableShowDamageText, false);
        set => SetOption(ref fields.DisableShowDamageText, SettingEntry.LaunchDisableShowDamageText, value);
    }

    public bool UsingTouchScreen
    {
        get => GetOption(ref fields.UsingTouchScreen, SettingEntry.LaunchUsingTouchScreen, false);
        set => SetOption(ref fields.UsingTouchScreen, SettingEntry.LaunchUsingTouchScreen, value);
    }

    public bool RedirectCombineEntry
    {
        get => GetOption(ref fields.RedirectCombineEntry, SettingEntry.LaunchRedirectCombineEntry, false);
        set => SetOption(ref fields.RedirectCombineEntry, SettingEntry.LaunchRedirectCombineEntry, value);
    }

    public ImmutableArray<AspectRatio> AspectRatios
    {
        get => GetOption(ref fields.AspectRatios, SettingEntry.AspectRatios, static raw => JsonSerializer.Deserialize<ImmutableArray<AspectRatio>>(raw, JsonOptions.Default), []);
        set => SetOption(ref fields.AspectRatios, SettingEntry.AspectRatios, value, static v => JsonSerializer.Serialize(v, JsonOptions.Default));
    }

    public AspectRatio? SelectedAspectRatio
    {
        get;
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                (ScreenWidth, ScreenHeight) = ((int)value.Width, (int)value.Height);
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

    private static int InitializeScreenFps()
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
        public bool? LaunchGameOnAppStartup;
    }
}