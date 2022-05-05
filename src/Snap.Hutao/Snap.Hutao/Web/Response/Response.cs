// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Serialization;

namespace Snap.Hutao.Web.Response;

/// <summary>
/// 提供 <see cref="Response{T}"/> 的非泛型基类
/// </summary>
public class Response
{
    /// <summary>
    /// 返回代码
    /// </summary>
    [JsonPropertyName("retcode")]
    public int ReturnCode { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>
    /// 响应是否正常
    /// </summary>
    /// <param name="response">响应</param>
    /// <returns>是否Ok</returns>
    public static bool IsOk(Response? response)
    {
        return response is not null && response.ReturnCode == 0;
    }

    /// <summary>
    /// 构造一个失败的响应
    /// </summary>
    /// <param name="message">消息</param>
    /// <returns>响应</returns>
    public static Response CreateForException(string message)
    {
        return new Response()
        {
            ReturnCode = (int)KnownReturnCode.InternalFailure,
            Message = message,
        };
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"状态：{ReturnCode} | 信息：{Message}";
    }
}