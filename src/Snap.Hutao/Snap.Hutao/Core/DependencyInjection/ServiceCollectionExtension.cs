// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Json;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Win32;
using System.Data.Common;
using System.Diagnostics;

namespace Snap.Hutao.Core.DependencyInjection;

internal static partial class ServiceCollectionExtension
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static partial IServiceCollection AddInjections(this IServiceCollection services);

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

            try
            {
                using (AppDbContext context = AppDbContext.Create(serviceProvider, sqlConnectionString))
                {
                    if (context.Database.GetPendingMigrations().Any())
                    {
                        serviceProvider.GetRequiredService<ILogger<AppDbContext>>().LogInformation("[Database] Performing AppDbContext Migrations");
                        context.Database.Migrate();
                    }
                }
            }
            catch (DbException ex)
            {
                ex.Data.Add("FilePath", dbFile);
                ex.SetSentryMechanism("DependencyInjection.DatabaseMigration", handled: true);

                string message = $"""
                    Snap Hutao 在执行数据库迁移时发生错误。
                    Snap Hutao encountered an error while performing database migration.
                    
                    Database at '{dbFile}'
                    
                    {ex.Message}
                    """;
                HutaoNative.Instance.ShowErrorMessage("Warning | 警告", message);
                Process.GetCurrentProcess().Kill();
                return;
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