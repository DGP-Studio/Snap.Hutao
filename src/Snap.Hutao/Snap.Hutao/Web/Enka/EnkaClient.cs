// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Enka.Model;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Sockets;

namespace Snap.Hutao.Web.Enka;

/// <summary>
/// Enka API 客户端
/// </summary>
[HighQuality]
[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class EnkaClient
{
    private const string EnkaAPI = "https://enka.network/api/uid/{0}";
    private const string EnkaAPIHutaoForward = "https://enka-api.hut.ao/{0}";

    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly JsonSerializerOptions options;
    private readonly HttpClient httpClient;

    /// <summary>
    /// 异步获取转发的 Enka API 响应
    /// </summary>
    /// <param name="playerUid">玩家Uid</param>
    /// <param name="token">取消令牌</param>
    /// <returns>Enka API 响应</returns>
    public ValueTask<EnkaResponse?> GetForwardDataAsync(in PlayerUid playerUid, CancellationToken token = default)
    {
        return TryGetEnkaResponseCoreAsync(EnkaAPIHutaoForward.Format(playerUid.Value), token);
    }

    /// <summary>
    /// 异步获取 Enka API 响应
    /// </summary>
    /// <param name="playerUid">玩家Uid</param>
    /// <param name="token">取消令牌</param>
    /// <returns>Enka API 响应</returns>
    public ValueTask<EnkaResponse?> GetDataAsync(in PlayerUid playerUid, CancellationToken token = default)
    {
        return TryGetEnkaResponseCoreAsync(EnkaAPI.Format(playerUid.Value), token);
    }

    private async ValueTask<EnkaResponse?> TryGetEnkaResponseCoreAsync(string url, CancellationToken token = default)
    {
        try
        {
            HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
                .SetRequestUri(url)
                .Get();

            HttpResponseMessage response = await httpClient.SendAsync(builder.HttpRequestMessage, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<EnkaResponse>(options, token).ConfigureAwait(false);
            }
            else
            {
                // https://github.com/yoimiya-kokomi/miao-plugin/pull/441
                // Additionally, HTTP codes for UID requests:
                // 400 = wrong UID format
                // 404 = player does not exist(MHY server told that)
                // 429 = rate - limit
                // 424 = game maintenance / everything is broken after the update
                // 500 = general server error
                // 503 = I screwed up massively
                string message = response.StatusCode switch
                {
                    HttpStatusCode.BadRequest => SH.WebEnkaResponseStatusCode400,
                    HttpStatusCode.NotFound => SH.WebEnkaResponseStatusCode404,
                    HttpStatusCode.FailedDependency => SH.WebEnkaResponseStatusCode424,
                    HttpStatusCode.TooManyRequests => SH.WebEnkaResponseStatusCode429,
                    HttpStatusCode.InternalServerError => SH.WebEnkaResponseStatusCode500,
                    HttpStatusCode.ServiceUnavailable => SH.WebEnkaResponseStatusCode503,
                    _ => SH.WebEnkaResponseStatusCodeUnknown,
                };

                return new() { Message = message, };
            }
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (IOException)
        {
            return null;
        }
        catch (JsonException)
        {
            return null;
        }
        catch (SocketException)
        {
            return null;
        }
    }
}