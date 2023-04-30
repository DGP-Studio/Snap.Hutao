// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Json;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Service;
using System.Diagnostics;
using System.Globalization;
using Windows.Globalization;

namespace Snap.Hutao.Core.DependencyInjection;

/// <summary>
/// <see cref="Ioc"/> 配置
/// </summary>
[HighQuality]
internal static class IocConfiguration
{
    /// <summary>
    /// 添加默认的 <see cref="JsonSerializerOptions"/>
    /// </summary>
    /// <param name="services">集合</param>
    /// <returns>可继续操作的集合</returns>
    public static IServiceCollection AddJsonOptions(this IServiceCollection services)
    {
        return services.AddSingleton(JsonOptions.Default);
    }

    /// <summary>
    /// 添加专用数据库
    /// </summary>
    /// <param name="services">集合</param>
    /// <returns>可继续操作的集合</returns>
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        return services
            .AddTransient(typeof(Database.ScopedDbCurrent<,>))
            .AddDbContext<AppDbContext>(AddDbContextCore);
    }

    /// <summary>
    /// 初始化语言
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    /// <returns>服务提供器，用于链式调用</returns>
    public static IServiceProvider InitializeCulture(this IServiceProvider serviceProvider)
    {
        AppOptions appOptions = serviceProvider.GetRequiredService<AppOptions>();
        appOptions.PreviousCulture = CultureInfo.CurrentCulture;

        CultureInfo cultureInfo = appOptions.CurrentCulture;

        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;
        ApplicationLanguages.PrimaryLanguageOverride = cultureInfo.Name;

        return serviceProvider;
    }

    private static void AddDbContextCore(IServiceProvider provider, DbContextOptionsBuilder builder)
    {
        HutaoOptions hutaoOptions = provider.GetRequiredService<HutaoOptions>();
        string dbFile = System.IO.Path.Combine(hutaoOptions.DataFolder, "Userdata.db");
        string sqlConnectionString = $"Data Source={dbFile}";

        // Temporarily create a context
        using (AppDbContext context = AppDbContext.Create(sqlConnectionString))
        {
            if (context.Database.GetPendingMigrations().Any())
            {
#if DEBUG
                Debug.WriteLine("[Database] Performing AppDbContext Migrations");
#endif
                context.Database.Migrate();
            }
        }

        builder
#if DEBUG
            .EnableSensitiveDataLogging()
#endif
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .UseSqlite(sqlConnectionString);
    }
}