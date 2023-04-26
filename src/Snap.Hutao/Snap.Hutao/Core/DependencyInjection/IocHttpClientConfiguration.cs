// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Net.Http;

namespace Snap.Hutao.Core.DependencyInjection;

/// <summary>
/// <see cref="Ioc"/> 与 <see cref="HttpClient"/> 配置
/// </summary>
[HighQuality]
internal static partial class IocHttpClientConfiguration
{
    private const string ApplicationJson = "application/json";

    /// <summary>
    /// 添加 <see cref="HttpClient"/>
    /// 此方法将会自动生成
    /// </summary>
    /// <param name="services">集合</param>
    /// <returns>可继续操作的集合</returns>
    public static partial IServiceCollection AddHttpClients(this IServiceCollection services);

    /// <summary>
    /// 默认配置
    /// </summary>
    /// <param name="client">配置后的客户端</param>
    private static void DefaultConfiguration(IServiceProvider serviceProvider, HttpClient client)
    {
        HutaoOptions hutaoOptions = serviceProvider.GetRequiredService<HutaoOptions>();

        client.Timeout = Timeout.InfiniteTimeSpan;
        client.DefaultRequestHeaders.UserAgent.ParseAdd(hutaoOptions.UserAgent);
    }

    /// <summary>
    /// 对于需要添加动态密钥的客户端使用此配置
    /// </summary>
    /// <param name="client">配置后的客户端</param>
    private static void XRpcConfiguration(HttpClient client)
    {
        client.Timeout = Timeout.InfiniteTimeSpan;
        client.DefaultRequestHeaders.UserAgent.ParseAdd(HoyolabOptions.UserAgent);
        client.DefaultRequestHeaders.Accept.ParseAdd(ApplicationJson);
        client.DefaultRequestHeaders.Add("x-rpc-app_version", HoyolabOptions.XrpcVersion);
        client.DefaultRequestHeaders.Add("x-rpc-client_type", "5");
        client.DefaultRequestHeaders.Add("x-rpc-device_id", HoyolabOptions.DeviceId);
    }

    /// <summary>
    /// 对于需要添加动态密钥的客户端使用此配置
    /// </summary>
    /// <param name="client">配置后的客户端</param>
    private static void XRpc2Configuration(HttpClient client)
    {
        client.Timeout = Timeout.InfiniteTimeSpan;
        client.DefaultRequestHeaders.UserAgent.ParseAdd(HoyolabOptions.UserAgent);
        client.DefaultRequestHeaders.Accept.ParseAdd(ApplicationJson);
        client.DefaultRequestHeaders.Add("x-rpc-aigis", string.Empty);
        client.DefaultRequestHeaders.Add("x-rpc-app_id", "bll8iq97cem8");
        client.DefaultRequestHeaders.Add("x-rpc-app_version", HoyolabOptions.XrpcVersion);
        client.DefaultRequestHeaders.Add("x-rpc-client_type", "2");
        client.DefaultRequestHeaders.Add("x-rpc-device_id", HoyolabOptions.DeviceId);
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
        client.DefaultRequestHeaders.UserAgent.ParseAdd(HoyolabOptions.UserAgentOversea);
        client.DefaultRequestHeaders.Accept.ParseAdd(ApplicationJson);
        client.DefaultRequestHeaders.Add("x-rpc-app_version", "1.5.0");
        client.DefaultRequestHeaders.Add("x-rpc-client_type", "4");
    }
}