// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Json;
using Snap.Hutao.Model.Entity.Database;

namespace Snap.Hutao.Core.DependencyInjection;

internal static class IocConfiguration
{
    public static IServiceCollection AddJsonOptions(this IServiceCollection services)
    {
        return services.AddSingleton(JsonOptions.Default);
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        return services.AddDbContextPool<AppDbContext>(AddDbContextCore);

        static void AddDbContextCore(IServiceProvider serviceProvider, DbContextOptionsBuilder builder)
        {
            string dbFile = System.IO.Path.Combine(HutaoRuntime.DataFolder, "Userdata.db");
            string sqlConnectionString = $"Data Source={dbFile}";

            using (AppDbContext context = AppDbContext.Create(serviceProvider, sqlConnectionString))
            {
                if (context.Database.GetPendingMigrations().Any())
                {
                    serviceProvider.GetRequiredService<ILogger<AppDbContext>>().LogInformation("[Database] Performing AppDbContext Migrations");
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