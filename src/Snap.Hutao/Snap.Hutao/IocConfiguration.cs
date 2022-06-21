// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Context.FileSystem;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Snap.Hutao;

/// <summary>
/// <see cref="Ioc"/> 配置
/// </summary>
internal static class IocConfiguration
{
    /// <summary>
    /// 添加默认的 <see cref="JsonSerializer"/> 配置
    /// </summary>
    /// <param name="services">集合</param>
    /// <returns>可继续操作的集合</returns>
    public static IServiceCollection AddDefaultJsonSerializerOptions(this IServiceCollection services)
    {
        // default json options, global configuration
        return services
            .AddSingleton(new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNameCaseInsensitive = true,
                WriteIndented = true,
            });
    }

    /// <summary>
    /// 添加数据库
    /// </summary>
    /// <param name="services">集合</param>
    /// <returns>可继续操作的集合</returns>
    public static IServiceCollection AddDatebase(this IServiceCollection services)
    {
        MyDocumentContext myDocument = new(new());
        myDocument.EnsureDirectory();

        string dbFile = myDocument.Locate("Userdata.db");
        string sqlConnectionString = $"Data Source={dbFile}";

        // temporarily create a context
        using (AppDbContext context = AppDbContext.CreateFrom(sqlConnectionString))
        {
            Debug.WriteLine("Migrate started");
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            Debug.WriteLine("Migrate completed");
        }

        // LocalSetting.Set(SettingKeys.LastAppVersion, CoreEnvironment.Version.ToString());
        return services
            .AddDbContextPool<AppDbContext>(builder => builder.UseSqlite(sqlConnectionString));
    }
}
