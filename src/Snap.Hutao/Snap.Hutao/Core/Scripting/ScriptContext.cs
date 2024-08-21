// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.User;
using Snap.Hutao.Web;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.DataSigning;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Net.Http;

namespace Snap.Hutao.Core.Scripting;

[SuppressMessage("", "SH001", Justification = "ScriptContext must be public in order to be exposed to the scripting environment")]
public sealed class ScriptContext
{
    public IServiceProvider ServiceProvider { get; } = Ioc.Default;

    public ISnapHutaoDiagnostics Diagnostics { get => ServiceProvider.GetRequiredService<ISnapHutaoDiagnostics>(); }

    public static string FormatJson(string input)
    {
        return JsonSerializer.Serialize(JsonSerializer.Deserialize<JsonElement>(input), Json.JsonOptions.Default);
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

            ILogger<ScriptContext> logger = scope.ServiceProvider.GetRequiredService<ILogger<ScriptContext>>();

            using (HttpClient httpClient = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient())
            {
                return await SendAsync(builder, httpClient, logger, default).ConfigureAwait(false);
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

            if (ParseDSFromString(ds) is { } sign)
            {
                await sign(builder).ConfigureAwait(false);
            }

            ILogger<ScriptContext> logger = scope.ServiceProvider.GetRequiredService<ILogger<ScriptContext>>();

            using (HttpClient httpClient = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient())
            {
                return await SendAsync(builder, httpClient, logger, default).ConfigureAwait(false);
            }
        }
    }

    private static Func<HttpRequestMessageBuilder, ValueTask>? ParseDSFromString(string? instruction)
    {
        if (string.IsNullOrEmpty(instruction))
        {
            return default;
        }

        string[] parts = instruction.Split('|');
        DataSignAlgorithmVersion version = Enum.Parse<DataSignAlgorithmVersion>(parts[0]);
        SaltType saltType = Enum.Parse<SaltType>(parts[1]);
        bool includeChars = bool.Parse(parts[2]);

        return (builder) => builder.SignDataAsync(version, saltType, includeChars);
    }

    private static async ValueTask<string> SendAsync(HttpRequestMessageBuilder builder, HttpClient httpClient, ILogger logger, CancellationToken token)
    {
        HttpContext context = new()
        {
            HttpClient = httpClient,
            Logger = logger,
            RequestAborted = token,
        };

        using (context)
        {
            await builder.SendAsync(context).ConfigureAwait(false);
            ArgumentNullException.ThrowIfNull(context.Response);
            return await context.Response.Content.ReadAsStringAsync(token).ConfigureAwait(false);
        }
    }
}