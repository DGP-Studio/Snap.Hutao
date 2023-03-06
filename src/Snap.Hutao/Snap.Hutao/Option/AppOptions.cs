// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.UI.Windowing;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using System.Globalization;
using Windows.Globalization;

namespace Snap.Hutao.Option;

/// <summary>
/// 应用程序选项
/// </summary>
[Injection(InjectAs.Singleton)]
internal sealed class AppOptions : IOptions<AppOptions>
{
    private readonly IServiceScopeFactory serviceScopeFactory;

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
    public string? GamePath
    {
        get
        {
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                return appDbContext.Settings.SingleOrDefault(e => e.Key == SettingEntry.GamePath)?.Value;
            }
        }

        set
        {
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                appDbContext.Settings.ExecuteDeleteWhere(e => e.Key == SettingEntry.GamePath);
                appDbContext.Settings.AddAndSave(new(SettingEntry.GamePath, value));
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
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                string? value = appDbContext.Settings.SingleOrDefault(e => e.Key == SettingEntry.IsEmptyHistoryWishVisible)?.Value;
                return value != null && bool.Parse(value);
            }
        }

        set
        {
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                appDbContext.Settings.ExecuteDeleteWhere(e => e.Key == SettingEntry.IsEmptyHistoryWishVisible);
                appDbContext.Settings.AddAndSave(new(SettingEntry.IsEmptyHistoryWishVisible, value.ToString()));
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
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                string? value = appDbContext.Settings.SingleOrDefault(e => e.Key == SettingEntry.SystemBackdropType)?.Value;
                return Enum.Parse<Core.Windowing.BackdropType>(value ?? nameof(Core.Windowing.BackdropType.Mica));
            }
        }

        set
        {
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                appDbContext.Settings.ExecuteDeleteWhere(e => e.Key == SettingEntry.SystemBackdropType);
                appDbContext.Settings.AddAndSave(new(SettingEntry.SystemBackdropType, value.ToString()));
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
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                string? value = appDbContext.Settings.SingleOrDefault(e => e.Key == SettingEntry.Culture)?.Value;
                return value != null ? CultureInfo.GetCultureInfo(value) : CultureInfo.CurrentCulture;
            }
        }

        set
        {
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                appDbContext.Settings.ExecuteDeleteWhere(e => e.Key == SettingEntry.Culture);
                appDbContext.Settings.AddAndSave(new(SettingEntry.Culture, value.Name));
            }
        }
    }

    /// <inheritdoc/>
    public AppOptions Value { get => this; }
}