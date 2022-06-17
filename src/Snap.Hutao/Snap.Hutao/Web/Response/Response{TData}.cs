// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Serialization;

namespace Snap.Hutao.Web.Response;

/// <summary>
/// Mihoyo 标准API响应
/// </summary>
/// <typeparam name="TData">数据类型</typeparam>
public class Response<TData> : Response
{
    /// <summary>
    /// 构造一个新的 Mihoyo 标准API响应
    /// </summary>
    /// <param name="returnCode">返回代码</param>
    /// <param name="message">消息</param>
    /// <param name="data">数据</param>
    public Response(int returnCode, string message, TData? data)
        : base(returnCode, message)
    {
        Data = data;
    }

    /// <summary>
    /// 数据
    /// </summary>
    [JsonPropertyName("data")]
    public TData? Data { get; set; }
}
