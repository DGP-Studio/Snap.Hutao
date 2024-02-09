// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Windowing;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Game.PathAbstraction;
using Snap.Hutao.Win32.Graphics.Gdi;
using System.Collections.Immutable;
using System.Globalization;
using Windows.Graphics;
using static Snap.Hutao.Win32.Gdi32;
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.Service.Game;

/// <summary>
/// 启动游戏选项
/// </summary>
[Injection(InjectAs.Singleton)]
internal sealed class LaunchOptions : DbStoreOptions
{
    private readonly int primaryScreenWidth;
    private readonly int primaryScreenHeight;
    private readonly int primaryScreenFps;

    private string? gamePath;
    private ImmutableList<GamePathEntry>? gamePathEntries;
    private bool? isEnabled;
    private bool? isAdvancedLaunchOptionsEnabled;
    private bool? isFullScreen;
    private bool? isBorderless;
    private bool? isExclusive;
    private int? screenWidth;
    private bool? isScreenWidthEnabled;
    private int? screenHeight;
    private bool? isScreenHeightEnabled;
    private bool? unlockFps;
    private int? targetFps;
    private NameValue<int>? monitor;
    private bool? isMonitorEnabled;
    private bool? isUseCloudThirdPartyMobile;
    private bool? isWindowsHDREnabled;
    private AspectRatio? selectedAspectRatio;
    private bool? useStarwardPlayTimeStatistics;
    private bool? setDiscordActivityWhenPlaying;

    public LaunchOptions(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        RectInt32 primaryRect = DisplayArea.Primary.OuterBounds;
        primaryScreenWidth = primaryRect.Width;
        primaryScreenHeight = primaryRect.Height;

        InitializeMonitors(Monitors);
        InitializeScreenFps(out primaryScreenFps);

        static void InitializeMonitors(List<NameValue<int>> monitors)
        {
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

    public string GamePath
    {
        get => GetOption(ref gamePath, SettingEntry.GamePath);
        set => SetOption(ref gamePath, SettingEntry.GamePath, value);
    }

    public ImmutableList<GamePathEntry> GamePathEntries
    {
        // Because DbStoreOptions can't detect collection change, We use
        // ImmutableList to imply that the whole list needs to be replaced
        get => GetOption(ref gamePathEntries, SettingEntry.GamePathEntries, raw => JsonSerializer.Deserialize<ImmutableList<GamePathEntry>>(raw), []);
        set => SetOption(ref gamePathEntries, SettingEntry.GamePathEntries, value, value => JsonSerializer.Serialize(value));
    }

    public bool IsEnabled
    {
        get => GetOption(ref isEnabled, SettingEntry.LaunchIsLaunchOptionsEnabled, true);
        set => SetOption(ref isEnabled, SettingEntry.LaunchIsLaunchOptionsEnabled, value);
    }

    public bool IsAdvancedLaunchOptionsEnabled
    {
        get => GetOption(ref isAdvancedLaunchOptionsEnabled, SettingEntry.IsAdvancedLaunchOptionsEnabled);
        set => SetOption(ref isAdvancedLaunchOptionsEnabled, SettingEntry.IsAdvancedLaunchOptionsEnabled, value);
    }

    public bool IsFullScreen
    {
        get => GetOption(ref isFullScreen, SettingEntry.LaunchIsFullScreen);
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
        get => GetOption(ref screenWidth, SettingEntry.LaunchScreenWidth, primaryScreenWidth);
        set => SetOption(ref screenWidth, SettingEntry.LaunchScreenWidth, value);
    }

    public bool IsScreenWidthEnabled
    {
        get => GetOption(ref isScreenWidthEnabled, SettingEntry.LaunchIsScreenWidthEnabled, true);
        set => SetOption(ref isScreenWidthEnabled, SettingEntry.LaunchIsScreenWidthEnabled, value);
    }

    public int ScreenHeight
    {
        get => GetOption(ref screenHeight, SettingEntry.LaunchScreenHeight, primaryScreenHeight);
        set => SetOption(ref screenHeight, SettingEntry.LaunchScreenHeight, value);
    }

    public bool IsScreenHeightEnabled
    {
        get => GetOption(ref isScreenHeightEnabled, SettingEntry.LaunchIsScreenHeightEnabled, true);
        set => SetOption(ref isScreenHeightEnabled, SettingEntry.LaunchIsScreenHeightEnabled, value);
    }

    public bool UnlockFps
    {
        get => GetOption(ref unlockFps, SettingEntry.LaunchUnlockFps);
        set => SetOption(ref unlockFps, SettingEntry.LaunchUnlockFps, value);
    }

    public int TargetFps
    {
        get => GetOption(ref targetFps, SettingEntry.LaunchTargetFps, primaryScreenFps);
        set => SetOption(ref targetFps, SettingEntry.LaunchTargetFps, value);
    }

    public List<NameValue<int>> Monitors { get; } = [];

    [AllowNull]
    public NameValue<int> Monitor
    {
        get => GetOption(ref monitor, SettingEntry.LaunchMonitor, index => Monitors[int.Parse(index, CultureInfo.InvariantCulture) - 1], Monitors[0]);
        set
        {
            if (value is not null)
            {
                SetOption(ref monitor, SettingEntry.LaunchMonitor, value, selected => selected.Value.ToString(CultureInfo.InvariantCulture));
            }
        }
    }

    public bool IsMonitorEnabled
    {
        get => GetOption(ref isMonitorEnabled, SettingEntry.LaunchIsMonitorEnabled, true);
        set => SetOption(ref isMonitorEnabled, SettingEntry.LaunchIsMonitorEnabled, value);
    }

    public bool IsUseCloudThirdPartyMobile
    {
        get => GetOption(ref isUseCloudThirdPartyMobile, SettingEntry.LaunchIsUseCloudThirdPartyMobile, false);
        set => SetOption(ref isUseCloudThirdPartyMobile, SettingEntry.LaunchIsUseCloudThirdPartyMobile, value);
    }

    public bool IsWindowsHDREnabled
    {
        get => GetOption(ref isWindowsHDREnabled, SettingEntry.LaunchIsWindowsHDREnabled, false);
        set => SetOption(ref isWindowsHDREnabled, SettingEntry.LaunchIsWindowsHDREnabled, value);
    }

    public List<AspectRatio> AspectRatios { get; } =
    [
        new(3840, 2160),
        new(2560, 1600),
        new(2560, 1440),
        new(2410, 1080),
        new(1920, 1080),
    ];

    public AspectRatio? SelectedAspectRatio
    {
        get => selectedAspectRatio;
        set
        {
            if (SetProperty(ref selectedAspectRatio, value) && value is { } aspectRatio)
            {
                (ScreenWidth, ScreenHeight) = ((int)aspectRatio.Width, (int)aspectRatio.Height);
            }
        }
    }

    public bool UseStarwardPlayTimeStatistics
    {
        get => GetOption(ref useStarwardPlayTimeStatistics, SettingEntry.LaunchUseStarwardPlayTimeStatistics, false);
        set => SetOption(ref useStarwardPlayTimeStatistics, SettingEntry.LaunchUseStarwardPlayTimeStatistics, value);
    }

    public bool SetDiscordActivityWhenPlaying
    {
        get => GetOption(ref setDiscordActivityWhenPlaying, SettingEntry.LaunchSetDiscordActivityWhenPlaying, true);
        set => SetOption(ref setDiscordActivityWhenPlaying, SettingEntry.LaunchSetDiscordActivityWhenPlaying, value);
    }
}