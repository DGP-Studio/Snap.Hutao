// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.DynamicSecret.Http;

/// <summary>
/// 使用动态密钥2的Http客户端抽象
/// </summary>
internal interface IDynamicSecretHttpClient
{
    /// <summary>
    /// Sends a GET request to the specified Uri and returns the value that results from deserializing the response body as JSON in an asynchronous operation.
    /// </summary>
    /// <typeparam name="TValue">The target type to deserialize to.</typeparam>
    /// <param name="token">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    Task<TValue?> GetFromJsonAsync<TValue>(CancellationToken token);
}

/// <summary>
/// 使用动态密钥2的Http客户端抽象
/// </summary>
/// <typeparam name="TValue">请求数据的类型</typeparam>
internal interface IDynamicSecretHttpClient<TValue>
    where TValue : class
{
    /// <summary>
    /// Sends a POST request to the specified Uri containing the value serialized as JSON in the request body.
    /// </summary>
    /// <param name="token">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    Task<HttpResponseMessage> PostAsJsonAsync(CancellationToken token);

    /// <summary>
    /// Sends a POST request to the specified Uri containing the value serialized as JSON in the request body.
    /// </summary>
    /// <typeparam name="TResult">值的类型</typeparam>
    /// <param name="logger">日志器</param>
    /// <param name="token">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>结果</returns>
    Task<TResult?> TryCatchPostAsJsonAsync<TResult>(ILogger logger, CancellationToken token = default)
        where TResult : class;
}