// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using JetBrains.Annotations;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Core.Text.Json;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.User;
using Snap.Hutao.Web;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.DataSigning;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Net.Http;

namespace Snap.Hutao.Core.Scripting;

[UsedImplicitly]
[SuppressMessage("", "SH001", Justification = "ScriptContext must be public in order to be exposed to the scripting environment")]
public sealed class ScriptContext
{
    public IServiceProvider ServiceProvider { get => Ioc.Default; }

    public IHutaoDiagnostics Diagnostics { get => ServiceProvider.GetRequiredService<IHutaoDiagnostics>(); }

    public static string FormatJson(string input)
    {
        return Serialize(JsonSerializer.Deserialize<JsonElement>(input));
    }

    public static string Serialize<T>(T data)
    {
        return JsonSerializer.Serialize(data, JsonOptions.Default);
    }

    public async ValueTask<string> RequestAsync(string method, string url, string[] headers, string? body = default)
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory = scope.ServiceProvider.GetRequiredService<IHttpRequestMessageBuilderFactory>();
            HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
                .SetMethod(method)
                .SetRequestUri(url);

            foreach (string header in headers)
            {
                int indexOfColon = header.IndexOf(':', StringComparison.Ordinal);
                if (indexOfColon > 0)
                {
                    builder.AddHeader(header.AsSpan()[..indexOfColon].Trim().ToString(), header.AsSpan()[(indexOfColon + 1)..].Trim().ToString());
                }
                else
                {
                    builder.AddHeader(header);
                }
            }

            if (!string.IsNullOrEmpty(body))
            {
                builder.SetStringContent(body);
            }

            using (HttpClient httpClient = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient())
            {
                return await SendAsync(builder, httpClient, default).ConfigureAwait(false);
            }
        }
    }

    public async ValueTask<string> RequestWithCurrentHomaUserAsync(string method, string url, string[] headers, string? body = default)
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            HutaoUserOptions hutaoUserOptions = scope.ServiceProvider.GetRequiredService<HutaoUserOptions>();
            string? accessToken = await hutaoUserOptions.GetAccessTokenAsync().ConfigureAwait(false);
            if (string.IsNullOrEmpty(accessToken))
            {
                return "Passport not logged in";
            }

            IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory = scope.ServiceProvider.GetRequiredService<IHttpRequestMessageBuilderFactory>();
            HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
                .SetMethod(method)
                .SetAccessToken(accessToken)
                .SetHomaToken(accessToken)
                .SetRequestUri(url);

            foreach (string header in headers)
            {
                int indexOfColon = header.IndexOf(':', StringComparison.Ordinal);
                if (indexOfColon > 0)
                {
                    builder.AddHeader(header.AsSpan()[..indexOfColon].Trim().ToString(), header.AsSpan()[(indexOfColon + 1)..].Trim().ToString());
                }
                else
                {
                    builder.AddHeader(header);
                }
            }

            if (!string.IsNullOrEmpty(body))
            {
                builder.SetStringContent(body);
            }

            using (HttpClient httpClient = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient())
            {
                return await SendAsync(builder, httpClient, default).ConfigureAwait(false);
            }
        }
    }

    public async ValueTask<string> RequestWithCurrentUserAndUidAsync(string method, string url, string[] headers, string? body = default, string? ds = default)
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            IUserService userService = scope.ServiceProvider.GetRequiredService<IUserService>();
            if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is not { } userAndUid)
            {
                return "No user and uid selected";
            }

            IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory = scope.ServiceProvider.GetRequiredService<IHttpRequestMessageBuilderFactory>();
            HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
                .SetMethod(method)
                .SetRequestUri(url)
                .SetUserCookieAndFpHeader(userAndUid, CookieType.All);

            foreach (string header in headers)
            {
                int indexOfColon = header.IndexOf(':', StringComparison.Ordinal);
                if (indexOfColon > 0)
                {
                    builder.AddHeader(header.AsSpan()[..indexOfColon].Trim().ToString(), header.AsSpan()[(indexOfColon + 1)..].Trim().ToString());
                }
                else
                {
                    builder.AddHeader(header);
                }
            }

            if (!string.IsNullOrEmpty(body))
            {
                builder.SetStringContent(body);
            }

            if (ParseDsFromString(ds) is { } sign)
            {
                await sign(builder).ConfigureAwait(false);
            }

            using (HttpClient httpClient = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient())
            {
                return await SendAsync(builder, httpClient, default).ConfigureAwait(false);
            }
        }
    }

    private static Func<HttpRequestMessageBuilder, ValueTask>? ParseDsFromString(string? instruction)
    {
        if (string.IsNullOrEmpty(instruction))
        {
            return default;
        }

        string[] parts = instruction.Split('|');
        DataSignAlgorithmVersion version = Enum.Parse<DataSignAlgorithmVersion>(parts[0]);
        SaltType saltType = Enum.Parse<SaltType>(parts[1]);
        bool includeChars = bool.Parse(parts[2]);

        return builder => builder.SignDataAsync(version, saltType, includeChars);
    }

    private static async ValueTask<string> SendAsync(HttpRequestMessageBuilder builder, HttpClient httpClient, CancellationToken token)
    {
        HttpContext context = new()
        {
            HttpClient = httpClient,
            RequestAborted = token,
        };

        using (context)
        {
            await builder.SendAsync(context).ConfigureAwait(false);

            context.Exception?.Throw();
            ArgumentNullException.ThrowIfNull(context.Response);
            return await context.Response.Content.ReadAsStringAsync(token).ConfigureAwait(false);
        }
    }
}