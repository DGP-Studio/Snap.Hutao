// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Web.Response;
using System.Net.Http;
using System.Text;

namespace Snap.Hutao.Web.Request;

/// <summary>
/// 请求器
/// </summary>
[Injection(InjectAs.Transient)]
public class Requester
{
    private readonly HttpClient httpClient;
    private readonly Json json;
    private readonly ILogger<Requester> logger;

    /// <summary>
    /// 构造一个新的 <see cref="Requester"/> 对象
    /// </summary>
    /// <param name="httpClient">Http 客户端</param>
    /// <param name="json">Json 处理器</param>
    /// <param name="logger">消息器</param>
    public Requester(HttpClient httpClient, Json json, ILogger<Requester> logger)
    {
        this.httpClient = httpClient;
        this.json = json;
        this.logger = logger;
    }

    /// <summary>
    /// 请求头
    /// </summary>
    public RequestOptions Headers { get; set; } = new RequestOptions();

    /// <summary>
    /// 内部使用的 <see cref="HttpClient"/>
    /// </summary>
    protected HttpClient HttpClient { get => httpClient; }

    /// <summary>
    /// GET 操作
    /// </summary>
    /// <typeparam name="TResult">返回的类类型</typeparam>
    /// <param name="url">地址</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>响应</returns>
    public async Task<Response<TResult>?> GetAsync<TResult>(string? url, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("GET {urlbase}", url?.Split('?')[0]);
        return url is null
            ? null
            : await RequestAsync<TResult>(
                client => new RequestInfo(url, () => client.GetAsync(url, cancellationToken)),
                cancellationToken)
                .ConfigureAwait(false);
    }

    /// <summary>
    /// GET 操作
    /// </summary>
    /// <typeparam name="TResult">返回的类类型</typeparam>
    /// <param name="url">地址</param>
    /// <param name="encoding">编码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>响应</returns>
    public async Task<Response<TResult>?> GetAsync<TResult>(string? url, Encoding encoding, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("GET {urlbase}", url?.Split('?')[0]);
        return url is null
            ? null
            : await RequestAsync<TResult>(
                client => new RequestInfo(url, () => client.GetAsync(url, cancellationToken), encoding),
                cancellationToken)
                .ConfigureAwait(false);
    }

    /// <summary>
    /// POST 操作
    /// </summary>
    /// <typeparam name="TResult">返回的类类型</typeparam>
    /// <param name="url">地址</param>
    /// <param name="data">要发送的.NET（匿名）对象</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>响应</returns>
    public async Task<Response<TResult>?> PostAsync<TResult>(string? url, object data, CancellationToken cancellationToken = default)
    {
        string dataString = json.Stringify(data);
        logger.LogInformation("POST {urlbase} with\n{dataString}", url?.Split('?')[0], dataString);
        return url is null
            ? null
            : await RequestAsync<TResult>(
                client => new RequestInfo(url, () => client.PostAsync(url, new StringContent(dataString), cancellationToken)),
                cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// POST 操作,Content-Type
    /// </summary>
    /// <typeparam name="TResult">返回的类类型</typeparam>
    /// <param name="url">地址</param>
    /// <param name="data">要发送的.NET（匿名）对象</param>
    /// <param name="contentType">内容类型</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>响应</returns>
    public async Task<Response<TResult>?> PostAsync<TResult>(string? url, object data, string contentType, CancellationToken cancellationToken = default)
    {
        string dataString = json.Stringify(data);
        logger.LogInformation("POST {urlbase} with\n{dataString}", url?.Split('?')[0], dataString);
        return url is null
            ? null
            : await RequestAsync<TResult>(
                client => new RequestInfo(url, () => client.PostAsync(url, new StringContent(dataString, Encoding.UTF8, contentType), cancellationToken)),
                cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// 重置状态
    /// 清空请求头
    /// </summary>
    /// <returns>链式调用需要的实例</returns>
    public Requester Reset()
    {
        Headers.Clear();
        return this;
    }

    /// <summary>
    /// 添加请求头
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>链式调用需要的实例</returns>
    public Requester AddHeader(string key, string value)
    {
        Headers.Add(key, value);
        return this;
    }

    /// <summary>
    /// 在请求前准备 <see cref="System.Net.Http.HttpClient"/>
    /// </summary>
    protected virtual void PrepareHttpClient()
    {
        HttpClient.DefaultRequestHeaders.Clear();

        foreach ((string name, string value) in Headers)
        {
            HttpClient.DefaultRequestHeaders.Add(name, value);
        }
    }

    private async Task<Response<TResult>?> RequestAsync<TResult>(Func<HttpClient, RequestInfo> requestFunc, CancellationToken cancellationToken = default)
    {
        PrepareHttpClient();
        RequestInfo? info = requestFunc(HttpClient);

        try
        {
            HttpResponseMessage response = await info.RequestAsyncFunc.Invoke()
                .ConfigureAwait(false);

            string contentString = await response.Content.ReadAsStringAsync(cancellationToken)
                .ConfigureAwait(false);

            if (info.Encoding is not null)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(contentString);
                info.Encoding.GetString(bytes);
            }

            logger.LogInformation("Response String :{contentString}", contentString);

            return json.ToObject<Response<TResult>>(contentString);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "请求时遇到问题");
            return Response<TResult>.CreateFail($"{ex.Message}");
        }
        finally
        {
            logger.LogInformation("Request Completed");
        }
    }

    private record RequestInfo
    {
        public RequestInfo(string url, Func<Task<HttpResponseMessage>> httpResponseMessage, Encoding? encoding = null)
        {
            Url = url;
            RequestAsyncFunc = httpResponseMessage;
            Encoding = encoding;
        }

        public string Url { get; set; }

        public Func<Task<HttpResponseMessage>> RequestAsyncFunc { get; set; }

        public Encoding? Encoding { get; set; }
    }
}