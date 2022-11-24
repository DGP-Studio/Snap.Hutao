// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Widget;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;

/// <summary>
/// 验证结果
/// </summary>
public class VerificationResult
{
    /// <summary>
    /// 极验流水号
    /// </summary>
    [JsonPropertyName("challenge")]
    public string? Challenge { get; set; }
}