// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Passport;

/// <summary>
/// 链接
/// </summary>
public class Link
{
    /// <summary>
    /// 第三方 sn
    /// </summary>
    [JsonPropertyName("thirdparty")]
    public string Thirdparty { get; set; } = default!;

    /// <summary>
    /// Union Id
    /// </summary>
    [JsonPropertyName("union_id")]
    public string UnionId { get; set; } = default!;

    /// <summary>
    /// 昵称
    /// </summary>
    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = default!;
}
