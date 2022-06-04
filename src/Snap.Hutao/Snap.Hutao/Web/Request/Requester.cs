// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Json;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
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
    private readonly IInfoBarService infoBarService;
    private readonly ILogger<Requester> logger;

    private bool isRequesting = false;

    /// <summary>
    /// 构造一个新的 <see cref="Requester"/> 对象
    /// </summary>
    /// <param name="httpClient">Http 客户端</param>
    /// <param name="json">Json 处理器</param>
    /// <param name="infoBarService">信息条服务</param>
    /// <param name="logger">消息器</param>
    public Requester(HttpClient httpClient, Json json, IInfoBarService infoBarService, ILogger<Requester> logger)
    {
        this.httpClient = httpClient;
        this.json = json;
        this.infoBarService = infoBarService;
        this.logger = logger;
    }

    /// <summary>
    /// 请求头
    /// </summary>
    public RequestOptions Headers { get; set; } = new RequestOptions();

    /// <summary>
    /// 此请求器使用的Json解析器
    /// </summary>
    public Json Json { get => json; }

    /// <summary>
    /// 内部使用的 <see cref="System.Net.Http.HttpClient"/>
    /// </summary>
    protected HttpClient HttpClient { get => httpClient; }

    /// <summary>
    /// GET 操作
    /// </summary>
    /// <typeparam name="TResult">返回的类类型</typeparam>
    /// <param name="url">地址</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>响应</returns>
    public async Task<Response<TResult>?> GetAsync<TResult>(string url, CancellationToken cancellationToken = default)
    {
        if (url is null)
        {
            return Response<TResult>.CreateForEmptyUrl();
        }

        Task<HttpResponseMessage> GetMethod(HttpClient client, CancellationToken token) => client.GetAsync(url, token);

        return await RequestAsync<TResult>(GetMethod, cancellationToken)
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
    public async Task<Response<TResult>?> PostAsync<TResult>(string url, object data, CancellationToken cancellationToken = default)
    {
        if (url is null)
        {
            return Response<TResult>.CreateForEmptyUrl();
        }

        string dataString = Json.Stringify(data);
        HttpContent content = new StringContent(dataString);

        Task<HttpResponseMessage> PostMethod(HttpClient client, CancellationToken token) => client.PostAsync(url, content, token);

        return await RequestAsync<TResult>(PostMethod, cancellationToken)
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
    public async Task<Response<TResult>?> PostAsync<TResult>(string url, object data, string contentType, CancellationToken cancellationToken = default)
    {
        if (url is null)
        {
            return Response<TResult>.CreateForEmptyUrl();
        }

        string dataString = Json.Stringify(data);
        HttpContent content = new StringContent(dataString, Encoding.UTF8, contentType);

        Task<HttpResponseMessage> PostMethod(HttpClient client, CancellationToken token) => client.PostAsync(url, content, token);

        return await RequestAsync<TResult>(PostMethod, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// 重置状态
    /// 清空请求头
    /// </summary>
    /// <returns>链式调用需要的实例</returns>
    public Requester Reset()
    {
        Verify.Operation(!isRequesting, "无法在请求发生时重置请求器");
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
        Verify.Operation(!isRequesting, "无法在请求发生时修改请求头");
        Headers.Add(key, value);
        return this;
    }

    /// <summary>
    /// 接受Json
    /// </summary>
    /// <returns>链式调用需要的实例</returns>
    public Requester SetAcceptJson()
    {
        return AddHeader(RequestHeaders.Accept, RequestOptions.Json);
    }

    /// <summary>
    /// 设置常规UA
    /// </summary>
    /// <returns>链式调用需要的实例</returns>
    public Requester SetCommonUA()
    {
        return AddHeader(RequestHeaders.UserAgent, RequestOptions.CommonUA);
    }

    /// <summary>
    /// 设置为由米游社请求
    /// </summary>
    /// <returns>链式调用需要的实例</returns>
    public Requester SetRequestWithHyperion()
    {
        return AddHeader(RequestHeaders.RequestWith, RequestOptions.Hyperion);
    }

    /// <summary>
    /// 添加Cookie请求头
    /// </summary>
    /// <param name="user">用户</param>
    /// <returns>链式调用需要的实例</returns>
    public Requester SetUser(User user)
    {
        return AddHeader(RequestHeaders.Cookie, user.Cookie ?? string.Empty);
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

    private async Task<Response<TResult>?> RequestAsync<TResult>(
        Func<HttpClient, CancellationToken, Task<HttpResponseMessage>> requestAsyncFunc,
        CancellationToken cancellationToken = default)
    {
        isRequesting = true;
        PrepareHttpClient();

        try
        {
            HttpResponseMessage response = await requestAsyncFunc
                .Invoke(HttpClient, cancellationToken)
                .ConfigureAwait(false);

            string contentString = await response.Content
                .ReadAsStringAsync(cancellationToken)
                .ConfigureAwait(false);

            Response<TResult>? resp = Json.ToObject<Response<TResult>>(contentString);

            if (resp is null)
            {
                return Response<TResult>.CreateForJsonException(contentString);
            }

            ValidateResponse(resp);

            return resp;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "请求时遇到问题");
            return Response<TResult>.CreateForException($"{ex.Message}");
        }
        finally
        {
            isRequesting = false;
            logger.LogInformation("Request Completed");
        }
    }

    private void ValidateResponse<TResult>(Response<TResult> resp)
    {
        // 未知的返回代码
        if (!Enum.IsDefined(typeof(KnownReturnCode), resp.ReturnCode))
        {
            infoBarService.Information(resp.ToString());
        }
    }
}