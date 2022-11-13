// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Passport;

/// <summary>
/// 验证返回信息
/// </summary>
public class VerifyInformation
{
    /// <summary>
    /// 账户Id
    /// </summary>
    [JsonPropertyName("aid")]
    public string Aid { get; set; } = default!;

    /// <summary>
    /// 米哈游Id
    /// </summary>
    [JsonPropertyName("mid")]
    public string Mid { get; set; } = default!;

    /// <summary>
    /// 空
    /// </summary>
    [JsonPropertyName("account_name")]
    public string AccountName { get; set; } = default!;

    /// <summary>
    /// 空
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; set; } = default!;

    /// <summary>
    /// 是否为邮箱验证
    /// </summary>
    [JsonPropertyName("is_email_verify")]
    public int IsEmailVerify { get; set; } = default!;

    /// <summary>
    /// 区域码 +86
    /// </summary>
    [JsonPropertyName("area_code")]
    public string AreaCode { get; set; } = default!;

    /// <summary>
    /// 手机号 111****1111
    /// </summary>
    [JsonPropertyName("mobile")]
    public string Mobile { get; set; } = default!;

    /// <summary>
    /// 安全手机区域码 +86
    /// </summary>
    [JsonPropertyName("safe_area_code")]
    public string SafeAreaCode { get; set; } = default!;

    /// <summary>
    /// 安全手机号 111****1111
    /// </summary>
    [JsonPropertyName("safe_mobile")]
    public string SafeMobile { get; set; } = default!;

    /// <summary>
    /// 真名 **某
    /// </summary>
    [JsonPropertyName("realname")]
    public string Realname { get; set; } = default!;

    /// <summary>
    /// 身份证号 111************111
    /// </summary>
    [JsonPropertyName("identity_code")]
    public string IdentityCode { get; set; } = default!;

    /// <summary>
    /// 重新绑定区域码
    /// </summary>
    [JsonPropertyName("rebind_area_code")]
    public string RebindAreaCode { get; set; } = default!;

    /// <summary>
    /// 重新绑定的手机号
    /// </summary>
    [JsonPropertyName("rebind_mobile")]
    public string RebindMobile { get; set; } = default!;

    /// <summary>
    /// 重新绑定时间 "0"
    /// </summary>
    [JsonPropertyName("rebind_mobile_time")]
    public string RebindMobileTime { get; set; } = default!;

    /// <summary>
    /// 链接
    /// </summary>
    [JsonPropertyName("links")]
    public List<Link> Links { get; set; } = default!;
}