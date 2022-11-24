// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Snap.Hutao.Web.Geetest;

/// <summary>
/// 极验客户端
/// </summary>
[HttpClient(HttpClientConfigration.Default)]
internal class GeetestClient
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;

    /// <summary>
    /// 构造一个新的极验客户端
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    public GeetestClient(HttpClient httpClient, JsonSerializerOptions options)
    {
        this.httpClient = httpClient;
        this.options = options;
    }

    /// <summary>
    /// 获取gt类型
    /// </summary>
    /// <param name="gt">gt</param>
    /// <returns>类型</returns>
    public async Task<GeetestResult<JsonElement>?> GetTypeAsync(string gt)
    {
        string raw = await httpClient.GetStringAsync(ApiEndpoints.GeetestGetType(gt)).ConfigureAwait(false);
        raw = raw[0] == '(' ? raw[1..^1] : raw; // remove surrounded ( )
        return JsonSerializer.Deserialize<GeetestResult<JsonElement>>(raw, options);
    }

    /// <summary>
    /// 获取验证方式
    /// 概率获取validate
    /// </summary>
    /// <param name="gt">gt</param>
    /// <param name="challenge">验证流水号</param>
    /// <returns>验证方式</returns>
    public async Task<GeetestResult<GeetestData>?> GetAjaxAsync(string gt, string challenge)
    {
        string raw = await httpClient.GetStringAsync(ApiEndpoints.GeetestAjax(gt, challenge)).ConfigureAwait(false);
        raw = raw[0] == '(' ? raw[1..^1] : raw; // remove surrounded ( )
        return JsonSerializer.Deserialize<GeetestResult<GeetestData>>(raw, options);
    }

    /// <summary>
    /// 获取验证方式
    /// 概率获取validate
    /// </summary>
    /// <param name="registration">验证注册</param>
    /// <returns>验证方式</returns>
    public Task<GeetestResult<GeetestData>?> GetAjaxAsync(VerificationRegistration registration)
    {
        return GetAjaxAsync(registration.Gt, registration.Challenge);
    }
}

/// <summary>
/// 极验返回结果信息
/// </summary>
public class GeetestResult<T>
{
    /// <summary>
    /// 成功失败的标识码 success
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = default!;

    /// <summary>
    /// 返回数据，json格式
    /// </summary>
    [JsonPropertyName("data")]
    public T Data { get; set; } = default!;
}

/// <summary>
/// 极验数据
/// </summary>
public class GeetestData
{
    /// <summary>
    /// 结果
    /// </summary>
    [JsonPropertyName("result")]
    public string Result { get; set; } = default!;

    /// <summary>
    /// 验证
    /// </summary>
    [JsonPropertyName("validate")]
    public string? Validate { get; set; }

    /// <summary>
    /// 分数
    /// </summary>
    public int Score { get; set; } = default!;
}