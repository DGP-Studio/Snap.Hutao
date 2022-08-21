// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace Snap.Hutao.Core.DependencyInjection;

/// <summary>
/// <see cref="Ioc"/> 与 <see cref="HttpClient"/> 配置
/// </summary>
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
        client.DefaultRequestHeaders.Add("x-rpc-app_version", CoreEnvironment.HoyolabXrpcVersion);
        client.DefaultRequestHeaders.Add("x-rpc-client_type", "5");
        client.DefaultRequestHeaders.Add("x-rpc-device_id", CoreEnvironment.HoyolabDeviceId);
    }
}