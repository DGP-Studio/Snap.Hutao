// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.UI.Windowing;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Win32;
using Windows.Graphics;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Service.Game;

/// <summary>
/// 启动游戏选项
/// </summary>
[Injection(InjectAs.Singleton)]
internal sealed class LaunchOptions : ObservableObject, IOptions<LaunchOptions>
{
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly int primaryScreenWidth;
    private readonly int primaryScreenHeight;
    private readonly int primaryScreenFps;

    private bool? isFullScreen;
    private bool? isBorderless;
    private bool? isExclusive;
    private int? screenWidth;
    private int? screenHeight;
    private bool? unlockFps;
    private int? targetFps;
    private NameValue<int>? monitor;

    /// <summary>
    /// 构造一个新的启动游戏选项
    /// </summary>
    /// <param name="serviceScopeFactory">服务范围工厂</param>
    public LaunchOptions(IServiceScopeFactory serviceScopeFactory)
    {
        this.serviceScopeFactory = serviceScopeFactory;
        RectInt32 primaryRect = DisplayArea.Primary.OuterBounds;
        primaryScreenWidth = primaryRect.Width;
        primaryScreenHeight = primaryRect.Height;

        // This list can't use foreach
        IReadOnlyList<DisplayArea> displayAreas = DisplayArea.FindAll();
        for (int i = 0; i < displayAreas.Count; i++)
        {
            DisplayArea displayArea = displayAreas[i];
            int index = i + 1;
            Monitors.Add(new($"{displayArea.DisplayId.Value:X8}:{index}", index));
        }

        InitializeScreenFps(out primaryScreenFps);
    }

    /// <summary>
    /// 是否全屏
    /// </summary>
    public bool IsFullScreen
    {
        get
        {
            if (isFullScreen == null)
            {
                using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    string? value = appDbContext.Settings.SingleOrDefault(e => e.Key == SettingEntry.LaunchIsFullScreen)?.Value;
                    isFullScreen = value != null && bool.Parse(value);
                }
            }

            return isFullScreen.Value;
        }

        set
        {
            if (SetProperty(ref isFullScreen, value))
            {
                if (value)
                {
                    IsBorderless = false;
                }

                using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    appDbContext.Settings.ExecuteDeleteWhere(e => e.Key == SettingEntry.LaunchIsFullScreen);
                    appDbContext.Settings.AddAndSave(new(SettingEntry.LaunchIsFullScreen, value.ToString()));
                }
            }
        }
    }

    /// <summary>
    /// 是否无边框
    /// </summary>
    public bool IsBorderless
    {
        get
        {
            if (isBorderless == null)
            {
                using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    string? value = appDbContext.Settings.SingleOrDefault(e => e.Key == SettingEntry.LaunchIsBorderless)?.Value;
                    isBorderless = value != null && bool.Parse(value);
                }
            }

            return isBorderless.Value;
        }

        set
        {
            if (SetProperty(ref isBorderless, value))
            {
                if (value)
                {
                    IsExclusive = false;
                    IsFullScreen = false;
                }

                using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    appDbContext.Settings.ExecuteDeleteWhere(e => e.Key == SettingEntry.LaunchIsBorderless);
                    appDbContext.Settings.AddAndSave(new(SettingEntry.LaunchIsBorderless, value.ToString()));
                }
            }
        }
    }

    /// <summary>
    /// 是否独占全屏
    /// </summary>
    public bool IsExclusive
    {
        get
        {
            if (isExclusive == null)
            {
                using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    string? value = appDbContext.Settings.SingleOrDefault(e => e.Key == SettingEntry.LaunchIsExclusive)?.Value;
                    isExclusive = value != null && bool.Parse(value);
                }
            }

            return isExclusive.Value;
        }

        set
        {
            if (SetProperty(ref isExclusive, value))
            {
                if (value)
                {
                    IsFullScreen = true;
                }

                using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    appDbContext.Settings.ExecuteDeleteWhere(e => e.Key == SettingEntry.LaunchIsExclusive);
                    appDbContext.Settings.AddAndSave(new(SettingEntry.LaunchIsExclusive, value.ToString()));
                }
            }
        }
    }

    /// <summary>
    /// 屏幕宽度
    /// </summary>
    public int ScreenWidth
    {
        get
        {
            if (screenWidth == null)
            {
                using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    string? value = appDbContext.Settings.SingleOrDefault(e => e.Key == SettingEntry.LaunchScreenWidth)?.Value;
                    screenWidth = value == null ? primaryScreenWidth : int.Parse(value);
                }
            }

            return screenWidth.Value;
        }

        set
        {
            if (SetProperty(ref screenWidth, value))
            {
                using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    appDbContext.Settings.ExecuteDeleteWhere(e => e.Key == SettingEntry.LaunchScreenWidth);
                    appDbContext.Settings.AddAndSave(new(SettingEntry.LaunchScreenWidth, value.ToString()));
                }
            }
        }
    }

    /// <summary>
    /// 屏幕高度
    /// </summary>
    public int ScreenHeight
    {
        get
        {
            if (screenHeight == null)
            {
                using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    string? value = appDbContext.Settings.SingleOrDefault(e => e.Key == SettingEntry.LaunchScreenHeight)?.Value;
                    screenHeight = value == null ? primaryScreenHeight : int.Parse(value);
                }
            }

            return screenHeight.Value;
        }

        set
        {
            if (SetProperty(ref screenHeight, value))
            {
                using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    appDbContext.Settings.ExecuteDeleteWhere(e => e.Key == SettingEntry.LaunchScreenHeight);
                    appDbContext.Settings.AddAndSave(new(SettingEntry.LaunchScreenHeight, value.ToString()));
                }
            }
        }
    }

    /// <summary>
    /// 是否全屏
    /// </summary>
    public bool UnlockFps
    {
        get
        {
            if (unlockFps == null)
            {
                using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    string? value = appDbContext.Settings.SingleOrDefault(e => e.Key == SettingEntry.LaunchUnlockFps)?.Value;
                    unlockFps = value != null && bool.Parse(value);
                }
            }

            return unlockFps.Value;
        }

        set
        {
            if (SetProperty(ref unlockFps, value))
            {
                using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    appDbContext.Settings.ExecuteDeleteWhere(e => e.Key == SettingEntry.LaunchUnlockFps);
                    appDbContext.Settings.AddAndSave(new(SettingEntry.LaunchUnlockFps, value.ToString()));
                }
            }
        }
    }

    /// <summary>
    /// 目标帧率
    /// </summary>
    public int TargetFps
    {
        get
        {
            if (targetFps == null)
            {
                using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    string? value = appDbContext.Settings.SingleOrDefault(e => e.Key == SettingEntry.LaunchTargetFps)?.Value;
                    targetFps = value == null ? primaryScreenFps : int.Parse(value);
                }
            }

            return targetFps.Value;
        }

        set
        {
            if (SetProperty(ref targetFps, value))
            {
                using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    appDbContext.Settings.ExecuteDeleteWhere(e => e.Key == SettingEntry.LaunchTargetFps);
                    appDbContext.Settings.AddAndSave(new(SettingEntry.LaunchTargetFps, value.ToString()));
                }
            }
        }
    }

    /// <summary>
    /// 所有监视器
    /// </summary>
    public List<NameValue<int>> Monitors { get; } = new();

    /// <summary>
    /// 目标帧率
    /// </summary>
    public NameValue<int> Monitor
    {
        get
        {
            if (monitor == null)
            {
                using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    string? value = appDbContext.Settings.SingleOrDefault(e => e.Key == SettingEntry.LaunchMonitor)?.Value;

                    int index = value == null ? 1 : int.Parse(value);
                    monitor = Monitors[index - 1];
                }
            }

            return monitor;
        }

        set
        {
            if (SetProperty(ref monitor, value))
            {
                using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    appDbContext.Settings.ExecuteDeleteWhere(e => e.Key == SettingEntry.LaunchMonitor);
                    appDbContext.Settings.AddAndSave(new(SettingEntry.LaunchMonitor, value.Value.ToString()));
                }
            }
        }
    }

    /// <inheritdoc/>
    public LaunchOptions Value { get => this; }

    private static void InitializeScreenFps(out int fps)
    {
        HDC hDC = GetDC(HWND.Null);
        fps = GetDeviceCaps(hDC, GET_DEVICE_CAPS_INDEX.VREFRESH);
        if (ReleaseDC(HWND.Null, hDC) == 0)
        {
            // not released
            throw new Win32Exception();
        }
    }
}