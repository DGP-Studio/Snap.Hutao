// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Windowing;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Game.Launching;
using Snap.Hutao.Service.Game.Launching.Handler;
using Snap.Hutao.Service.Game.PathAbstraction;
using Snap.Hutao.Win32.Graphics.Gdi;
using System.Collections.Immutable;
using System.Globalization;
using Windows.Graphics;
using static Snap.Hutao.Win32.Gdi32;
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.Service.Game;

[Injection(InjectAs.Singleton)]
internal sealed partial class LaunchOptions : DbStoreOptions, IRecipient<LaunchExecutionProcessStatusChangedMessage>
{
    private readonly ITaskContext taskContext;

    private readonly int primaryScreenFps;

    private ImmutableArray<GamePathEntry>? gamePathEntries;

    private bool? usingHoyolabAccount;
    private bool? areCommandLineArgumentsEnabled;
    private bool? isFullScreen;
    private bool? isBorderless;
    private bool? isExclusive;
    private int? screenWidth;
    private bool? isScreenWidthEnabled;
    private int? screenHeight;
    private bool? isScreenHeightEnabled;

    private bool? isIslandEnabled;
    private bool? hookingSetFieldOfView;
    private bool? isSetFieldOfViewEnabled;
    private float? targetFov;
    private bool? fixLowFovScene;
    private bool? disableFog;
    private bool? isSetTargetFrameRateEnabled;
    private int? targetFps;
    private bool? hookingOpenTeam;
    private bool? removeOpenTeamProgress;
    private bool? hookingMickyWonderPartner2;
    private bool? isMonitorEnabled;
    private bool? usingCloudThirdPartyMobile;
    private bool? isWindowsHDREnabled;
    private bool? usingStarwardPlayTimeStatistics;
    private bool? usingBetterGenshinImpactAutomation;
    private bool? setDiscordActivityWhenPlaying;

    public LaunchOptions(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();

        RectInt32 primaryRect = DisplayArea.Primary.OuterBounds;
        ScreenWidth = primaryRect.Width;
        ScreenHeight = primaryRect.Height;

        Monitors = InitializeMonitors();
        InitializeScreenFps(out primaryScreenFps);

        // Batch initialization, boost up performance
        InitializeOptions(entry => entry.Key.StartsWith("Launch."), (key, value) =>
        {
            _ = key switch
            {
                SettingEntry.LaunchUsingHoyolabAccount => InitializeBooleanValue(ref usingHoyolabAccount, value),
                SettingEntry.LaunchAreCommandLineArgumentsEnabled => InitializeBooleanValue(ref areCommandLineArgumentsEnabled, value),
                SettingEntry.LaunchIsFullScreen => InitializeBooleanValue(ref isFullScreen, value),
                SettingEntry.LaunchIsBorderless => InitializeBooleanValue(ref isBorderless, value),
                SettingEntry.LaunchIsExclusive => InitializeBooleanValue(ref isExclusive, value),
                SettingEntry.LaunchScreenWidth => InitializeInt32Value(ref screenWidth, value),
                SettingEntry.LaunchIsScreenWidthEnabled => InitializeBooleanValue(ref isScreenWidthEnabled, value),
                SettingEntry.LaunchScreenHeight => InitializeInt32Value(ref screenHeight, value),
                SettingEntry.LaunchIsScreenHeightEnabled => InitializeBooleanValue(ref isScreenHeightEnabled, value),
                SettingEntry.LaunchIsMonitorEnabled => InitializeBooleanValue(ref isMonitorEnabled, value),
                SettingEntry.LaunchUsingCloudThirdPartyMobile => InitializeBooleanValue(ref usingCloudThirdPartyMobile, value),
                SettingEntry.LaunchIsWindowsHDREnabled => InitializeBooleanValue(ref isWindowsHDREnabled, value),
                SettingEntry.LaunchUsingStarwardPlayTimeStatistics => InitializeBooleanValue(ref usingStarwardPlayTimeStatistics, value),
                SettingEntry.LaunchUsingBetterGenshinImpactAutomation => InitializeBooleanValue(ref usingBetterGenshinImpactAutomation, value),
                SettingEntry.LaunchSetDiscordActivityWhenPlaying => InitializeBooleanValue(ref setDiscordActivityWhenPlaying, value),
                SettingEntry.LaunchIsIslandEnabled => InitializeBooleanValue(ref isIslandEnabled, value),
                SettingEntry.LaunchHookingSetFieldOfView => InitializeBooleanValue(ref hookingSetFieldOfView, value),
                SettingEntry.LaunchIsSetFieldOfViewEnabled => InitializeBooleanValue(ref isSetFieldOfViewEnabled, value),
                SettingEntry.LaunchTargetFov => InitializeFloatValue(ref targetFov, value),
                SettingEntry.LaunchFixLowFovScene => InitializeBooleanValue(ref fixLowFovScene, value),
                SettingEntry.LaunchDisableFog => InitializeBooleanValue(ref disableFog, value),
                SettingEntry.LaunchIsSetTargetFrameRateEnabled => InitializeBooleanValue(ref isSetTargetFrameRateEnabled, value),
                SettingEntry.LaunchTargetFps => InitializeInt32Value(ref targetFps, value),
                SettingEntry.LaunchHookingOpenTeam => InitializeBooleanValue(ref hookingOpenTeam, value),
                SettingEntry.LaunchRemoveOpenTeamProgress => InitializeBooleanValue(ref removeOpenTeamProgress, value),
                SettingEntry.LaunchHookingMickyWonderPartner2 => InitializeBooleanValue(ref hookingMickyWonderPartner2, value),
                _ => default,
            };
        });

        IslandFeatureStateMachine = new(this);
        serviceProvider.GetRequiredService<IMessenger>().Register(this);

        static Void InitializeBooleanValue(ref bool? storage, string? value)
        {
            if (value is not null)
            {
                bool.TryParse(value, out bool result);
                storage = result;
            }

            return default;
        }

        static Void InitializeInt32Value(ref int? storage, string? value)
        {
            if (value is not null)
            {
                int.TryParse(value, CultureInfo.InvariantCulture, out int result);
                storage = result;
            }

            return default;
        }

        static Void InitializeFloatValue(ref float? storage, string? value)
        {
            if (value is not null)
            {
                float.TryParse(value, CultureInfo.InvariantCulture, out float result);
                storage = result;
            }

            return default;
        }

        static ImmutableArray<NameValue<int>> InitializeMonitors()
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

        static void InitializeScreenFps(out int fps)
        {
            HDC hDC = default;
            try
            {
                hDC = GetDC(default);
                fps = GetDeviceCaps(hDC, GET_DEVICE_CAPS_INDEX.VREFRESH);
            }
            finally
            {
                _ = ReleaseDC(default, hDC);
            }
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
        // ImmutableList to imply that the whole list needs to be replaced
        get => GetOption(ref gamePathEntries, SettingEntry.GamePathEntries, raw => JsonSerializer.Deserialize<ImmutableArray<GamePathEntry>>(raw), []).Value;
        set => SetOption(ref gamePathEntries, SettingEntry.GamePathEntries, value, v => JsonSerializer.Serialize(v));
    }

    #region Launch Prefixed Options

    #region CLI Options

    public bool UsingHoyolabAccount
    {
        get => GetOption(ref usingHoyolabAccount, SettingEntry.LaunchUsingHoyolabAccount, false);
        set => SetOption(ref usingHoyolabAccount, SettingEntry.LaunchUsingHoyolabAccount, value);
    }

    public bool AreCommandLineArgumentsEnabled
    {
        get => GetOption(ref areCommandLineArgumentsEnabled, SettingEntry.LaunchAreCommandLineArgumentsEnabled, true);
        set
        {
            if (SetOption(ref areCommandLineArgumentsEnabled, SettingEntry.LaunchAreCommandLineArgumentsEnabled, value))
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
        get => GetOption(ref isFullScreen, SettingEntry.LaunchIsFullScreen, false);
        set => SetOption(ref isFullScreen, SettingEntry.LaunchIsFullScreen, value);
    }

    public bool IsBorderless
    {
        get => GetOption(ref isBorderless, SettingEntry.LaunchIsBorderless);
        set => SetOption(ref isBorderless, SettingEntry.LaunchIsBorderless, value);
    }

    public bool IsExclusive
    {
        get => GetOption(ref isExclusive, SettingEntry.LaunchIsExclusive);
        set => SetOption(ref isExclusive, SettingEntry.LaunchIsExclusive, value);
    }

    public int ScreenWidth
    {
        get => GetOption(ref screenWidth, SettingEntry.LaunchScreenWidth, field);
        set => SetOption(ref screenWidth, SettingEntry.LaunchScreenWidth, value);
    }

    public bool IsScreenWidthEnabled
    {
        get => GetOption(ref isScreenWidthEnabled, SettingEntry.LaunchIsScreenWidthEnabled, true);
        set => SetOption(ref isScreenWidthEnabled, SettingEntry.LaunchIsScreenWidthEnabled, value);
    }

    public int ScreenHeight
    {
        get => GetOption(ref screenHeight, SettingEntry.LaunchScreenHeight, field);
        set => SetOption(ref screenHeight, SettingEntry.LaunchScreenHeight, value);
    }

    public bool IsScreenHeightEnabled
    {
        get => GetOption(ref isScreenHeightEnabled, SettingEntry.LaunchIsScreenHeightEnabled, true);
        set => SetOption(ref isScreenHeightEnabled, SettingEntry.LaunchIsScreenHeightEnabled, value);
    }

    public ImmutableArray<NameValue<int>> Monitors { get; }

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
                SetOption(ref field, SettingEntry.LaunchMonitor, value, selected => selected.Value.ToString(CultureInfo.InvariantCulture));
            }
        }
    }

    public bool IsMonitorEnabled
    {
        get => GetOption(ref isMonitorEnabled, SettingEntry.LaunchIsMonitorEnabled, true);
        set => SetOption(ref isMonitorEnabled, SettingEntry.LaunchIsMonitorEnabled, value);
    }

    public bool UsingCloudThirdPartyMobile
    {
        get => GetOption(ref usingCloudThirdPartyMobile, SettingEntry.LaunchUsingCloudThirdPartyMobile, false);
        set => SetOption(ref usingCloudThirdPartyMobile, SettingEntry.LaunchUsingCloudThirdPartyMobile, value);
    }

    public bool IsWindowsHDREnabled
    {
        get => GetOption(ref isWindowsHDREnabled, SettingEntry.LaunchIsWindowsHDREnabled, false);
        set => SetOption(ref isWindowsHDREnabled, SettingEntry.LaunchIsWindowsHDREnabled, value);
    }
    #endregion

    #region InterProcess

    public bool UsingStarwardPlayTimeStatistics
    {
        get => GetOption(ref usingStarwardPlayTimeStatistics, SettingEntry.LaunchUsingStarwardPlayTimeStatistics, false);
        set => SetOption(ref usingStarwardPlayTimeStatistics, SettingEntry.LaunchUsingStarwardPlayTimeStatistics, value);
    }

    public bool UsingBetterGenshinImpactAutomation
    {
        get => GetOption(ref usingBetterGenshinImpactAutomation, SettingEntry.LaunchUsingBetterGenshinImpactAutomation, false);
        set => SetOption(ref usingBetterGenshinImpactAutomation, SettingEntry.LaunchUsingBetterGenshinImpactAutomation, value);
    }

    public bool SetDiscordActivityWhenPlaying
    {
        get => GetOption(ref setDiscordActivityWhenPlaying, SettingEntry.LaunchSetDiscordActivityWhenPlaying, true);
        set => SetOption(ref setDiscordActivityWhenPlaying, SettingEntry.LaunchSetDiscordActivityWhenPlaying, value);
    }
    #endregion

    #region Island Features

    public LaunchOptionsIslandFeatureStateMachine IslandFeatureStateMachine { get; }

    public bool IsIslandEnabled
    {
        get => GetOption(ref isIslandEnabled, SettingEntry.LaunchIsIslandEnabled, false);
        set
        {
            if (SetOption(ref isIslandEnabled, SettingEntry.LaunchIsIslandEnabled, value))
            {
                IslandFeatureStateMachine.Update(this);
            }
        }
    }

    public bool HookingSetFieldOfView
    {
        get => GetOption(ref hookingSetFieldOfView, SettingEntry.LaunchHookingSetFieldOfView, true);
        set
        {
            if (SetOption(ref hookingSetFieldOfView, SettingEntry.LaunchHookingSetFieldOfView, value))
            {
                IslandFeatureStateMachine.Update(this);
            }
        }
    }

    public bool IsSetFieldOfViewEnabled
    {
        get => GetOption(ref isSetFieldOfViewEnabled, SettingEntry.LaunchIsSetFieldOfViewEnabled, true);
        set
        {
            if (SetOption(ref isSetFieldOfViewEnabled, SettingEntry.LaunchIsSetFieldOfViewEnabled, value))
            {
                IslandFeatureStateMachine.Update(this);
            }
        }
    }

    public float TargetFov
    {
        get => GetOption(ref targetFov, SettingEntry.LaunchTargetFov, 45f);
        set => SetOption(ref targetFov, SettingEntry.LaunchTargetFov, value);
    }

    public bool FixLowFovScene
    {
        get => GetOption(ref fixLowFovScene, SettingEntry.LaunchFixLowFovScene, true);
        set => SetOption(ref fixLowFovScene, SettingEntry.LaunchFixLowFovScene, value);
    }

    public bool DisableFog
    {
        get => GetOption(ref disableFog, SettingEntry.LaunchDisableFog, false);
        set => SetOption(ref disableFog, SettingEntry.LaunchDisableFog, value);
    }

    public bool IsSetTargetFrameRateEnabled
    {
        get => GetOption(ref isSetTargetFrameRateEnabled, SettingEntry.LaunchIsSetTargetFrameRateEnabled, true);
        set
        {
            if (SetOption(ref isSetTargetFrameRateEnabled, SettingEntry.LaunchIsSetTargetFrameRateEnabled, value))
            {
                IslandFeatureStateMachine.Update(this);
            }
        }
    }

    public int TargetFps
    {
        get => GetOption(ref targetFps, SettingEntry.LaunchTargetFps, primaryScreenFps);
        set => SetOption(ref targetFps, SettingEntry.LaunchTargetFps, value);
    }

    public bool HookingOpenTeam
    {
        get => GetOption(ref hookingOpenTeam, SettingEntry.LaunchHookingOpenTeam, true);
        set
        {
            if (SetOption(ref hookingOpenTeam, SettingEntry.LaunchHookingOpenTeam, value))
            {
                IslandFeatureStateMachine.Update(this);
            }
        }
    }

    public bool RemoveOpenTeamProgress
    {
        get => GetOption(ref removeOpenTeamProgress, SettingEntry.LaunchRemoveOpenTeamProgress, false);
        set => SetOption(ref removeOpenTeamProgress, SettingEntry.LaunchRemoveOpenTeamProgress, value);
    }

    public bool HookingMickyWonderPartner2
    {
        get => GetOption(ref hookingMickyWonderPartner2, SettingEntry.LaunchHookingMickyWonderPartner2, true);
        set => SetOption(ref hookingMickyWonderPartner2, SettingEntry.LaunchHookingMickyWonderPartner2, value);
    }
    #endregion

    #endregion

    public ImmutableArray<AspectRatio> AspectRatios { get; } =
    [
        new(3840, 2160),
        new(2560, 1600),
        new(2560, 1440),
        new(2410, 1080),
        new(1920, 1080),
    ];

    public AspectRatio? SelectedAspectRatio
    {
        get;
        set
        {
            if (SetProperty(ref field, value) && value is { } aspectRatio)
            {
                (ScreenWidth, ScreenHeight) = ((int)aspectRatio.Width, (int)aspectRatio.Height);
            }
        }
    }

#pragma warning disable CA1822
    public bool IsGameRunning { get => LaunchExecutionEnsureGameNotRunningHandler.IsGameRunning(); }
#pragma warning restore CA1822

    public void Receive(LaunchExecutionProcessStatusChangedMessage message)
    {
        taskContext.BeginInvokeOnMainThread(() =>
        {
            IslandFeatureStateMachine.Update(this);
            OnPropertyChanged(nameof(IsGameRunning));
        });
    }
}