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
        return services.AddDbContextPool<AppDbContext>(AddDbContextCore);

        static void AddDbContextCore(IServiceProvider serviceProvider, DbContextOptionsBuilder builder)
        {
            RuntimeOptions runtimeOptions = serviceProvider.GetRequiredService<RuntimeOptions>();
            string dbFile = System.IO.Path.Combine(runtimeOptions.DataFolder, "Userdata.db");
            string sqlConnectionString = $"Data Source={dbFile}";

            // Temporarily create a context
            using (AppDbContext context = AppDbContext.Create(serviceProvider, sqlConnectionString))
            {
                if (context.Database.GetPendingMigrations().Any())
                {
                    System.Diagnostics.Debug.WriteLine("[Database] Performing AppDbContext Migrations");
                    context.Database.Migrate();
                }
            }

            builder
                .EnableSensitiveDataLogging()
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .UseSqlite(sqlConnectionString);
        }
    }
}