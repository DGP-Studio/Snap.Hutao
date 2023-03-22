// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace Snap.Hutao.Core.DependencyInjection;

/// <summary>
/// <see cref="Ioc"/> 与 <see cref="HttpClient"/> 配置
/// </summary>
[HighQuality]
internal static partial class IocHttpClientConfiguration
{
    /// <summary>
    /// 添加 <see cref="HttpClient"/>
    /// </summary>
    /// <param name="services">集合</param>
    /// <returns>可继续操作的集合</returns>
    public static partial IServiceCollection AddHttpClients(this IServiceCollection services);

    /// <summary>
    /// 默认配置
    /// </summary>
    /// <param name="client">配置后的客户端</param>
    private static void DefaultConfiguration(HttpClient client)
    {
        client.Timeout = Timeout.InfiniteTimeSpan;
        client.DefaultRequestHeaders.UserAgent.ParseAdd(CoreEnvironment.CommonUA);
    }

    /// <summary>
    /// 对于需要添加动态密钥的客户端使用此配置
    /// </summary>
    /// <param name="client">配置后的客户端</param>
    private static void XRpcConfiguration(HttpClient client)
    {
        client.Timeout = Timeout.InfiniteTimeSpan;
        client.DefaultRequestHeaders.UserAgent.ParseAdd(CoreEnvironment.HoyolabUA);
        client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
        client.DefaultRequestHeaders.Add("x-rpc-app_version", CoreEnvironment.HoyolabXrpcVersion);
        client.DefaultRequestHeaders.Add("x-rpc-client_type", "5");
        client.DefaultRequestHeaders.Add("x-rpc-device_id", CoreEnvironment.HoyolabDeviceId);
    }

    /// <summary>
    /// 对于需要添加动态密钥的客户端使用此配置
    /// </summary>
    /// <param name="client">配置后的客户端</param>
    private static void XRpc2Configuration(HttpClient client)
    {
        client.Timeout = Timeout.InfiniteTimeSpan;
        client.DefaultRequestHeaders.UserAgent.ParseAdd(CoreEnvironment.HoyolabUA);
        client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
        client.DefaultRequestHeaders.Add("x-rpc-aigis", string.Empty);
        client.DefaultRequestHeaders.Add("x-rpc-app_id", "bll8iq97cem8");
        client.DefaultRequestHeaders.Add("x-rpc-app_version", CoreEnvironment.HoyolabXrpcVersion);
        client.DefaultRequestHeaders.Add("x-rpc-client_type", "2");
        client.DefaultRequestHeaders.Add("x-rpc-device_id", CoreEnvironment.HoyolabDeviceId);
        client.DefaultRequestHeaders.Add("x-rpc-game_biz", "bbs_cn");
        client.DefaultRequestHeaders.Add("x-rpc-sdk_version", "1.3.1.2");
    }

    /// <summary>
    /// 对于需要添加动态密钥的客户端使用此配置
    /// 国际服 API 测试
    /// </summary>
    /// <param name="client">配置后的客户端</param>
    private static void XRpc3Configuration(HttpClient client)
    {
        client.Timeout = Timeout.InfiniteTimeSpan;
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/106.0.0.0 Safari/537.36");
        client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
        client.DefaultRequestHeaders.Add("x-rpc-app_version", "1.5.0");
        client.DefaultRequestHeaders.Add("x-rpc-client_type", "4");
    }
}