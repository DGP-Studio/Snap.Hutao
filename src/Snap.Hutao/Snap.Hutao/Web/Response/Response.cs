// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Service.Abstraction;
using System.Text.Json.Serialization;

namespace Snap.Hutao.Web.Response;

/// <summary>
/// 提供 <see cref="Response{T}"/> 的非泛型基类
/// </summary>
public class Response : ISupportValidation
{
    /// <summary>
    /// 构造一个新的响应
    /// </summary>
    /// <param name="returnCode">返回代码</param>
    /// <param name="message">消息</param>
    [JsonConstructor]
    public Response(int returnCode, string message)
    {
        ReturnCode = returnCode;
        Message = message;

        if (!Validate())
        {
            Ioc.Default
                .GetRequiredService<IInfoBarService>()
                .Information(ToString());
        }
    }

    /// <summary>
    /// 返回代码
    /// </summary>
    [JsonPropertyName("retcode")]
    public int ReturnCode { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = default!;

    /// <summary>
    /// 响应是否正常
    /// </summary>
    /// <param name="response">响应</param>
    /// <returns>是否Ok</returns>
    public static bool IsOk(Response? response)
    {
        return response is not null && response.ReturnCode == 0;
    }

    /// <inheritdoc/>
    public bool Validate()
    {
        return Enum.IsDefined(typeof(KnownReturnCode), ReturnCode);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"状态：{ReturnCode} | 信息：{Message}";
    }
}