// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Context.FileSystem;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Json;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Web.Request;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Snap.Hutao;

/// <summary>
/// <see cref="Ioc"/> 配置
/// </summary>
internal static class IocConfiguration
{
    /// <summary>
    /// 添加 <see cref="System.Net.Http.HttpClient"/>
    /// </summary>
    /// <param name="services">集合</param>
    /// <returns>可继续操作的集合</returns>
    public static IServiceCollection AddHttpClients(this IServiceCollection services)
    {
        // http json
        services
            .AddHttpClient<HttpJson>()
            .ConfigureHttpClient(client =>
            {
                client.Timeout = Timeout.InfiniteTimeSpan;
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) Snap Hutao");
            });

        // requester & auth reuqester
        services
            .AddHttpClient<Requester>(nameof(Requester))
            .AddTypedClient<AuthRequester>()
            .ConfigureHttpClient(client => client.Timeout = Timeout.InfiniteTimeSpan);

        return services;
    }

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

        bool shouldMigrate = false;

        if (myDocument.FileExists(dbFile))
        {
            string? versionString = LocalSetting.Get<string>(SettingKeys.LastAppVersion);
            if (Version.TryParse(versionString, out Version? lastVersion))
            {
                if (lastVersion < CoreEnvironment.Version)
                {
                    shouldMigrate = true;
                }
            }
        }
        else
        {
            shouldMigrate = true;
        }

        if (shouldMigrate)
        {
            // temporarily create a context
            using (AppDbContext context = new(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(sqlConnectionString).Options))
            {
                context.Database.Migrate();
            }
        }

        LocalSetting.Set(SettingKeys.LastAppVersion, CoreEnvironment.Version.ToString());

        return services
            .AddDbContextPool<AppDbContext>(builder => builder.UseSqlite(sqlConnectionString));
    }
}