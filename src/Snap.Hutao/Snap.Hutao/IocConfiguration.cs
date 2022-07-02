// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Context.FileSystem;
using System.Diagnostics;
using System.Linq;
using System.Text.Encodings.Web;
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
    public static IServiceCollection AddJsonSerializerOptions(this IServiceCollection services)
    {
        return services
            .AddSingleton(new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNameCaseInsensitive = true,
                WriteIndented = true,
            });
    }

    /// <summary>
    /// 添加专用数据库
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
        using (AppDbContext context = AppDbContext.Create(sqlConnectionString))
        {
            if (context.Database.GetPendingMigrations().Any())
            {
                Debug.WriteLine("Performing Migrations");
                context.Database.Migrate();
            }
        }

        return services
            .AddDbContextPool<AppDbContext>(builder => builder.UseSqlite(sqlConnectionString));
    }
}