// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Newtonsoft.Json;

namespace Snap.Hutao.Web.Response;

/// <summary>
/// Mihoyo 标准API响应
/// </summary>
/// <typeparam name="TData">数据类型</typeparam>
public class Response<TData> : Response
{
    /// <summary>
    /// 数据
    /// </summary>
    [JsonProperty("data")]
    public TData? Data { get; set; }

    /// <summary>
    /// 构造一个失败的响应
    /// </summary>
    /// <param name="message">消息</param>
    /// <returns>响应</returns>
    public static new Response<TData> CreateFail(string message)
    {
        return new Response<TData>()
        {
            ReturnCode = (int)KnownReturnCode.InternalFailure,
            Message = message,
        };
    }
}
