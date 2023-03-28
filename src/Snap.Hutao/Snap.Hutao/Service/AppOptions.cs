// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Options;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using System.Globalization;

namespace Snap.Hutao.Service;

/// <summary>
/// 应用程序选项
/// </summary>
[Injection(InjectAs.Singleton)]
internal sealed class AppOptions : ObservableObject, IOptions<AppOptions>
{
    private readonly IServiceScopeFactory serviceScopeFactory;

    private string? gamePath;
    private bool? isEmptyHistoryWishVisible;
    private Core.Windowing.BackdropType? backdropType;
    private CultureInfo? currentCulture;
    private bool? isAdvancedLaunchOptionsEnabled;

    /// <summary>
    /// 构造一个新的应用程序选项
    /// </summary>
    /// <param name="serviceScopeFactory">服务范围工厂</param>
    public AppOptions(IServiceScopeFactory serviceScopeFactory)
    {
        this.serviceScopeFactory = serviceScopeFactory;
    }

    /// <summary>
    /// 游戏路径
    /// </summary>
    public string GamePath
    {
        get
        {
            if (gamePath == null)
            {
                using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    gamePath = appDbContext.Settings.SingleOrDefault(e => e.Key == SettingEntry.GamePath)?.Value ?? string.Empty;
                }
            }

            return gamePath;
        }

        set
        {
            if (SetProperty(ref gamePath, value))
            {
                using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    appDbContext.Settings.ExecuteDeleteWhere(e => e.Key == SettingEntry.GamePath);
                    appDbContext.Settings.AddAndSave(new(SettingEntry.GamePath, value));
                }
            }
        }
    }

    /// <summary>
    /// 游戏路径
    /// </summary>
    public bool IsEmptyHistoryWishVisible
    {
        get
        {
            if (isEmptyHistoryWishVisible == null)
            {
                using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    string? value = appDbContext.Settings.SingleOrDefault(e => e.Key == SettingEntry.IsEmptyHistoryWishVisible)?.Value;
                    isEmptyHistoryWishVisible = value != null && bool.Parse(value);
                }
            }

            return isEmptyHistoryWishVisible.Value;
        }

        set
        {
            if (SetProperty(ref isEmptyHistoryWishVisible, value))
            {
                using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    appDbContext.Settings.ExecuteDeleteWhere(e => e.Key == SettingEntry.IsEmptyHistoryWishVisible);
                    appDbContext.Settings.AddAndSave(new(SettingEntry.IsEmptyHistoryWishVisible, value.ToString()));
                }
            }
        }
    }

    /// <summary>
    /// 背景类型 默认 Mica
    /// </summary>
    public Core.Windowing.BackdropType BackdropType
    {
        get
        {
            if (backdropType == null)
            {
                using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    string? value = appDbContext.Settings.SingleOrDefault(e => e.Key == SettingEntry.SystemBackdropType)?.Value;
                    backdropType = Enum.Parse<Core.Windowing.BackdropType>(value ?? nameof(Core.Windowing.BackdropType.Mica));
                }
            }

            return backdropType.Value;
        }

        set
        {
            if (SetProperty(ref backdropType, value))
            {
                using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    appDbContext.Settings.ExecuteDeleteWhere(e => e.Key == SettingEntry.SystemBackdropType);
                    appDbContext.Settings.AddAndSave(new(SettingEntry.SystemBackdropType, value.ToString()));

                    scope.ServiceProvider.GetRequiredService<IMessenger>().Send(new Message.BackdropTypeChangedMessage(value));
                }
            }
        }
    }

    /// <summary>
    /// 当前语言
    /// </summary>
    public CultureInfo CurrentCulture
    {
        get
        {
            if (currentCulture == null)
            {
                using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    string? value = appDbContext.Settings.SingleOrDefault(e => e.Key == SettingEntry.Culture)?.Value;
                    currentCulture = value != null ? CultureInfo.GetCultureInfo(value) : CultureInfo.CurrentCulture;
                }
            }

            return currentCulture;
        }

        set
        {
            if (SetProperty(ref currentCulture, value))
            {
                using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    appDbContext.Settings.ExecuteDeleteWhere(e => e.Key == SettingEntry.Culture);
                    appDbContext.Settings.AddAndSave(new(SettingEntry.Culture, value.Name));
                }
            }
        }
    }

    /// <summary>
    /// 是否启用高级功能
    /// </summary>
    public bool IsAdvancedLaunchOptionsEnabled
    {
        get
        {
            if (isAdvancedLaunchOptionsEnabled == null)
            {
                using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    string? value = appDbContext.Settings.SingleOrDefault(e => e.Key == SettingEntry.IsAdvancedLaunchOptionsEnabled)?.Value;
                    isAdvancedLaunchOptionsEnabled = value != null && bool.Parse(value);
                }
            }

            return isAdvancedLaunchOptionsEnabled.Value;
        }

        set
        {
            if (SetProperty(ref isAdvancedLaunchOptionsEnabled, value))
            {
                using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    appDbContext.Settings.ExecuteDeleteWhere(e => e.Key == SettingEntry.IsAdvancedLaunchOptionsEnabled);
                    appDbContext.Settings.AddAndSave(new(SettingEntry.IsAdvancedLaunchOptionsEnabled, value.ToString()));
                }
            }
        }
    }

    /// <inheritdoc/>
    public AppOptions Value { get => this; }
}