// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Json;
using Snap.Hutao.Model.Entity.Database;

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
            .AddTransient(typeof(Database.ScopedDbCurrent<,,>))
            .AddDbContext<AppDbContext>(AddDbContextCore);
    }

    private static void AddDbContextCore(IServiceProvider provider, DbContextOptionsBuilder builder)
    {
        RuntimeOptions runtimeOptions = provider.GetRequiredService<RuntimeOptions>();
        string dbFile = System.IO.Path.Combine(runtimeOptions.DataFolder, "Userdata.db");
        string sqlConnectionString = $"Data Source={dbFile}";

        // Temporarily create a context
        using (AppDbContext context = AppDbContext.Create(sqlConnectionString))
        {
            if (context.Database.GetPendingMigrations().Any())
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("[Database] Performing AppDbContext Migrations");
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