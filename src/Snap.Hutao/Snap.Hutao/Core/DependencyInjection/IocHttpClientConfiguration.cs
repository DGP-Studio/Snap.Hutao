// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using JetBrains.Annotations;
using Snap.Hutao.Core.IO.Http;
using Snap.Hutao.Core.IO.Http.Proxy;
using Snap.Hutao.Service.Game.Package.Advanced;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Win32;
using System.Globalization;
using System.Net.Http;
using System.Net.Mime;
using System.Text;

namespace Snap.Hutao.Core.DependencyInjection;

// ReSharper disable UnusedMember.Local
internal static partial class IocHttpClientConfiguration
{
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
                        typedHandler.UseProxy = true;
                        typedHandler.Proxy = HttpProxyUsingSystemProxy.Instance;
                    })
                    .AddHttpMessageHandler<RetryHttpHandler>();
            })
            .AddHttpClients();

        services
            .AddHttpClient(GamePackageService.HttpClientName)
            .ConfigurePrimaryHttpMessageHandler((handler, provider) =>
            {
                SocketsHttpHandler typedHandler = (SocketsHttpHandler)handler;
                typedHandler.ConnectTimeout = TimeSpan.FromSeconds(30);
                typedHandler.MaxConnectionsPerServer = 12;
            });

        return services;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static partial IServiceCollection AddHttpClients(this IServiceCollection services);

    [UsedImplicitly]
    private static void DefaultConfiguration(IServiceProvider serviceProvider, HttpClient client)
    {
        client.Timeout = Timeout.InfiniteTimeSpan;
        client.DefaultRequestHeaders.UserAgent.ParseAdd(HutaoRuntime.UserAgent);
        client.DefaultRequestHeaders.Add("x-hutao-device-id", HutaoRuntime.DeviceId);
        client.DefaultRequestHeaders.Add("x-hutao-device-os", $"Windows {HutaoNative.Instance.GetCurrentWindowsVersion()}");
        client.DefaultRequestHeaders.Add("x-hutao-device-name", EncodeNonAsciiChars(Environment.MachineName));
    }

    [UsedImplicitly]
    private static void XRpcConfiguration(HttpClient client)
    {
        client.Timeout = Timeout.InfiniteTimeSpan;
        client.DefaultRequestHeaders.UserAgent.ParseAdd(HoyolabOptions.UserAgent);
        client.DefaultRequestHeaders.Accept.ParseAdd(MediaTypeNames.Application.Json);
        client.DefaultRequestHeaders.Add("x-rpc-app_version", SaltConstants.CNVersion);
        client.DefaultRequestHeaders.Add("x-rpc-client_type", "5");
        client.DefaultRequestHeaders.Add("x-rpc-device_id", HoyolabOptions.DeviceId36);
    }

    [UsedImplicitly]
    private static void XRpc2Configuration(HttpClient client)
    {
        client.Timeout = Timeout.InfiniteTimeSpan;
        client.DefaultRequestHeaders.UserAgent.ParseAdd(HoyolabOptions.UserAgent);
        client.DefaultRequestHeaders.Accept.ParseAdd(MediaTypeNames.Application.Json);
        client.DefaultRequestHeaders.Add("x-rpc-aigis", string.Empty);
        client.DefaultRequestHeaders.Add("x-rpc-app_id", "bll8iq97cem8");
        client.DefaultRequestHeaders.Add("x-rpc-app_version", SaltConstants.CNVersion);
        client.DefaultRequestHeaders.Add("x-rpc-client_type", "2");
        client.DefaultRequestHeaders.Add("x-rpc-device_id", HoyolabOptions.DeviceId36);
        client.DefaultRequestHeaders.Add("x-rpc-device_name", string.Empty);
        client.DefaultRequestHeaders.Add("x-rpc-game_biz", "bbs_cn");
        client.DefaultRequestHeaders.Add("x-rpc-sdk_version", "2.16.0");
    }

    [UsedImplicitly]
    private static void XRpc3Configuration(HttpClient client)
    {
        client.Timeout = Timeout.InfiniteTimeSpan;
        client.DefaultRequestHeaders.UserAgent.ParseAdd(HoyolabOptions.UserAgentOversea);
        client.DefaultRequestHeaders.Accept.ParseAdd(MediaTypeNames.Application.Json);
        client.DefaultRequestHeaders.Add("x-rpc-app_version", SaltConstants.OSVersion);
        client.DefaultRequestHeaders.Add("x-rpc-client_type", "5");
        client.DefaultRequestHeaders.Add("x-rpc-language", "zh-cn");
        client.DefaultRequestHeaders.Add("x-rpc-device_id", HoyolabOptions.DeviceId36);
    }

    [UsedImplicitly]
    [SuppressMessage("", "IDE0051")]
    private static void XRpc4Configuration(HttpClient client)
    {
        client.Timeout = Timeout.InfiniteTimeSpan;
        client.DefaultRequestHeaders.UserAgent.ParseAdd(HoyolabOptions.UserAgentOversea);
        client.DefaultRequestHeaders.Accept.ParseAdd(MediaTypeNames.Application.Json);
        client.DefaultRequestHeaders.Add("x-rpc-app_version", "1.5.0");
        client.DefaultRequestHeaders.Add("x-rpc-client_type", "4");
    }

    [UsedImplicitly]
    private static void XRpc5Configuration(HttpClient client)
    {
        client.Timeout = Timeout.InfiniteTimeSpan;
        client.DefaultRequestHeaders.UserAgent.ParseAdd(HoyolabOptions.HoyoPlayUserAgent);
        client.DefaultRequestHeaders.Accept.ParseAdd(MediaTypeNames.Application.Json);
        client.DefaultRequestHeaders.Add("x-rpc-app_id", "ddxf5dufpuyo");
        client.DefaultRequestHeaders.Add("x-rpc-client_type", "3");
    }

    [UsedImplicitly]
    private static void XRpc6Configuration(HttpClient client)
    {
        client.Timeout = Timeout.InfiniteTimeSpan;
        client.DefaultRequestHeaders.UserAgent.ParseAdd(HoyolabOptions.HoyoPlayUserAgent);
        client.DefaultRequestHeaders.Accept.ParseAdd(MediaTypeNames.Application.Json);
        client.DefaultRequestHeaders.Add("x-rpc-app_id", "ddxf6vlr1reo");
        client.DefaultRequestHeaders.Add("x-rpc-client_type", "3");
        client.DefaultRequestHeaders.Add("x-rpc-device_id", HoyolabOptions.DeviceId53);
    }

    private static string EncodeNonAsciiChars(string value)
    {
        StringBuilder sb = new();
        foreach (char c in value)
        {
            if (c > 127)
            {
                sb.Append("\\u").Append(((int)c).ToString("x4", CultureInfo.InvariantCulture));
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }
}