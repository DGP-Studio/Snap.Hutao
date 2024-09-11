// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO.Http.Proxy;
using Snap.Hutao.Service.Game.Package.Advanced;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Request;
using System.Net.Http;

namespace Snap.Hutao.Core.DependencyInjection;

internal static partial class IocHttpClientConfiguration
{
    private const string ApplicationJson = "application/json";

    public static IServiceCollection AddConfiguredHttpClients(this IServiceCollection services)
    {
        services
            .ConfigureHttpClientDefaults(clientBuilder =>
            {
                clientBuilder
                    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler())
                    .ConfigurePrimaryHttpMessageHandler((handler, provider) =>
                    {
                        SocketsHttpHandler typedHandler = (SocketsHttpHandler)handler;
                        typedHandler.ConnectTimeout = TimeSpan.FromSeconds(30);
                        typedHandler.AllowAutoRedirect = true;
                        typedHandler.UseProxy = true;
                        typedHandler.MaxConnectionsPerServer = 16;
                        typedHandler.Proxy = provider.GetRequiredService<HttpProxyUsingSystemProxy>();
                    });
            })
            .AddHttpClients();

        services
            .AddHttpClient(GamePackageService.HttpClientName)
            .ConfigurePrimaryHttpMessageHandler((handler, provider) =>
            {
                SocketsHttpHandler typedHandler = (SocketsHttpHandler)handler;
                typedHandler.AllowAutoRedirect = true;
                typedHandler.UseProxy = true;
                typedHandler.MaxConnectionsPerServer = 16;
                typedHandler.Proxy = provider.GetRequiredService<HttpProxyUsingSystemProxy>();
            });

        return services;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static partial IServiceCollection AddHttpClients(this IServiceCollection services);

    private static void DefaultConfiguration(IServiceProvider serviceProvider, HttpClient client)
    {
        RuntimeOptions runtimeOptions = serviceProvider.GetRequiredService<RuntimeOptions>();

        client.Timeout = Timeout.InfiniteTimeSpan;
        client.DefaultRequestHeaders.UserAgent.ParseAdd(runtimeOptions.UserAgent);
    }

    private static void XRpcConfiguration(HttpClient client)
    {
        client.Timeout = Timeout.InfiniteTimeSpan;
        client.DefaultRequestHeaders.UserAgent.ParseAdd(HoyolabOptions.UserAgent);
        client.DefaultRequestHeaders.Accept.ParseAdd(ApplicationJson);
        client.DefaultRequestHeaders.Add("x-rpc-app_version", SaltConstants.CNVersion);
        client.DefaultRequestHeaders.Add("x-rpc-client_type", "5");
        client.DefaultRequestHeaders.Add("x-rpc-device_id", HoyolabOptions.DeviceId36);
    }

    private static void XRpc2Configuration(HttpClient client)
    {
        client.Timeout = Timeout.InfiniteTimeSpan;
        client.DefaultRequestHeaders.UserAgent.ParseAdd(HoyolabOptions.UserAgent);
        client.DefaultRequestHeaders.Accept.ParseAdd(ApplicationJson);
        client.DefaultRequestHeaders.Add("x-rpc-aigis", string.Empty);
        client.DefaultRequestHeaders.Add("x-rpc-app_id", "bll8iq97cem8");
        client.DefaultRequestHeaders.Add("x-rpc-app_version", SaltConstants.CNVersion);
        client.DefaultRequestHeaders.Add("x-rpc-client_type", "2");
        client.DefaultRequestHeaders.Add("x-rpc-device_id", HoyolabOptions.DeviceId36);
        client.DefaultRequestHeaders.Add("x-rpc-device_name", string.Empty);
        client.DefaultRequestHeaders.Add("x-rpc-game_biz", "bbs_cn");
        client.DefaultRequestHeaders.Add("x-rpc-sdk_version", "2.16.0");
    }

    private static void XRpc3Configuration(HttpClient client)
    {
        client.Timeout = Timeout.InfiniteTimeSpan;
        client.DefaultRequestHeaders.UserAgent.ParseAdd(HoyolabOptions.UserAgentOversea);
        client.DefaultRequestHeaders.Accept.ParseAdd(ApplicationJson);
        client.DefaultRequestHeaders.Add("x-rpc-app_version", SaltConstants.OSVersion);
        client.DefaultRequestHeaders.Add("x-rpc-client_type", "5");
        client.DefaultRequestHeaders.Add("x-rpc-language", "zh-cn");
        client.DefaultRequestHeaders.Add("x-rpc-device_id", HoyolabOptions.DeviceId36);
    }

    [SuppressMessage("", "IDE0051")]
    private static void XRpc4Configuration(HttpClient client)
    {
        client.Timeout = Timeout.InfiniteTimeSpan;
        client.DefaultRequestHeaders.UserAgent.ParseAdd(HoyolabOptions.UserAgentOversea);
        client.DefaultRequestHeaders.Accept.ParseAdd(ApplicationJson);
        client.DefaultRequestHeaders.Add("x-rpc-app_version", "1.5.0");
        client.DefaultRequestHeaders.Add("x-rpc-client_type", "4");
    }
}