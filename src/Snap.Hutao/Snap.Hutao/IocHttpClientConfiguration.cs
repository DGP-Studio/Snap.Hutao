// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Web.Enka;
using Snap.Hutao.Web.Hoyolab.Bbs.User;
using Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;
using Snap.Hutao.Web.Hutao;
using System.Net.Http;

namespace Snap.Hutao;

/// <summary>
/// <see cref="Ioc"/> 与 <see cref="HttpClient"/> 配置
/// </summary>
internal static class IocHttpClientConfiguration
{
    private const string CommonUA = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) Snap Hutao";

    /// <summary>
    /// 添加 <see cref="HttpClient"/>
    /// </summary>
    /// <param name="services">集合</param>
    /// <returns>可继续操作的集合</returns>
    public static IServiceCollection AddHttpClients(this IServiceCollection services)
    {
        // services
        services.AddHttpClient<MetadataService>(DefaultConfiguration);

        // normal clients
        services.AddHttpClient<AnnouncementClient>(DefaultConfiguration);
        services.AddHttpClient<EnkaClient>(DefaultConfiguration);
        services.AddHttpClient<HutaoClient>(DefaultConfiguration);
        services.AddHttpClient<UserGameRoleClient>(DefaultConfiguration);

        // x-rpc clients
        services.AddHttpClient<GameRecordClient>(XRpcConfiguration);
        services.AddHttpClient<UserClient>(XRpcConfiguration);

        return services;
    }

    /// <summary>
    /// 默认配置
    /// </summary>
    /// <param name="client">配置后的客户端</param>
    private static void DefaultConfiguration(this HttpClient client)
    {
        client.Timeout = Timeout.InfiniteTimeSpan;
        client.DefaultRequestHeaders.UserAgent.ParseAdd(CommonUA);
    }

    /// <summary>
    /// 对于需要添加动态密钥的客户端使用此配置
    /// </summary>
    /// <param name="client">配置后的客户端</param>
    private static void XRpcConfiguration(this HttpClient client)
    {
        client.DefaultConfiguration();
        client.DefaultRequestHeaders.Add("x-rpc-app_version", "2.30.1");
        client.DefaultRequestHeaders.Add("x-rpc-client_type", "5");
    }
}