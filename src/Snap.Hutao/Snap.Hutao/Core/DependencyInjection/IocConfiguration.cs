// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Data.Sqlite;
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
        return services.AddDbContextPool<AppDbContext>(AddDbContext);

        static void AddDbContext(IServiceProvider serviceProvider, DbContextOptionsBuilder builder)
        {
            string dbFile = HutaoRuntime.GetDataFolderFile("Userdata.db");
            string sqlConnectionString = $"Data Source={dbFile}";

            using (AppDbContext context = AppDbContext.Create(serviceProvider, sqlConnectionString))
            {
                IEnumerable<string> pendingMigrations;
                try
                {
                    pendingMigrations = context.Database.GetPendingMigrations();
                }
                catch (SqliteException ex)
                {
                    ex.Data.Add("FilePath", dbFile);
                    throw;
                }

                if (pendingMigrations.Any())
                {
                    serviceProvider.GetRequiredService<ILogger<AppDbContext>>().LogInformation("[Database] Performing AppDbContext Migrations");
                    context.Database.Migrate();
                }
            }

            builder
#if DEBUG
                .EnableSensitiveDataLogging()
#endif
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>())
                .UseSqlite(sqlConnectionString);
        }
    }
}