// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Enka.Model;
using Snap.Hutao.Web.Hoyolab;
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
[HttpClient(HttpClientConfiguration.Default)]
internal sealed class EnkaClient
{
    private const string EnkaAPI = "https://enka.network/api/uid/{0}";
    private const string EnkaAPIHutaoForward = "https://enka-api.hut.ao/{0}";

    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;

    /// <summary>
    /// 构造一个新的 Enka API 客户端
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    /// <param name="options">序列化选项</param>
    /// <param name="logger">日志器</param>
    public EnkaClient(HttpClient httpClient, JsonSerializerOptions options)
    {
        this.httpClient = httpClient;
        this.options = options;
    }

    /// <summary>
    /// 异步获取转发的 Enka API 响应
    /// </summary>
    /// <param name="playerUid">玩家Uid</param>
    /// <param name="token">取消令牌</param>
    /// <returns>Enka API 响应</returns>
    public Task<EnkaResponse?> GetForwardDataAsync(in PlayerUid playerUid, CancellationToken token = default)
    {
        return TryGetEnkaResponseCoreAsync(string.Format(EnkaAPIHutaoForward, playerUid.Value), token);
    }

    /// <summary>
    /// 异步获取 Enka API 响应
    /// </summary>
    /// <param name="playerUid">玩家Uid</param>
    /// <param name="token">取消令牌</param>
    /// <returns>Enka API 响应</returns>
    public Task<EnkaResponse?> GetDataAsync(in PlayerUid playerUid, CancellationToken token = default)
    {
        return TryGetEnkaResponseCoreAsync(string.Format(EnkaAPI, playerUid.Value), token);
    }

    private async Task<EnkaResponse?> TryGetEnkaResponseCoreAsync(string url, CancellationToken token = default)
    {
        try
        {
            HttpResponseMessage response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false);
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