// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Request;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Web.Hoyolab.DynamicSecret;

/// <summary>
/// 动态密钥扩展
/// </summary>
internal static class RequesterExtensions
{
    /// <summary>
    /// 使用二代动态密钥执行 GET 操作
    /// </summary>
    /// <typeparam name="TResult">返回的类类型</typeparam>
    /// <param name="requester">请求器</param>
    /// <param name="url">地址</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>响应</returns>
    public static async Task<Response<TResult>?> GetUsingDS2Async<TResult>(
        this Requester requester,
        string url,
        CancellationToken cancellationToken = default)
    {
        requester.Headers["DS"] = DynamicSecretProvider2.Create(requester.Json, url);

        return await requester
            .GetAsync<TResult>(url, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// 使用二代动态密钥执行 POST 操作
    /// </summary>
    /// <typeparam name="TResult">返回的类类型</typeparam>
    /// <param name="requester">请求器</param>
    /// <param name="url">地址</param>
    /// <param name="data">post数据</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>响应</returns>
    public static async Task<Response<TResult>?> PostUsingDS2Async<TResult>(
         this Requester requester,
         string url,
         object data,
         CancellationToken cancellationToken = default)
    {
        requester.Headers["DS"] = DynamicSecretProvider2.Create(requester.Json, url, data);

        return await requester
            .PostAsync<TResult>(url, data, cancellationToken)
            .ConfigureAwait(false);
    }
}
