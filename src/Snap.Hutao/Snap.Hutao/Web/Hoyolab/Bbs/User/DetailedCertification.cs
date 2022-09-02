// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

/// <summary>
/// 详细认证信息
/// </summary>
public class DetailedCertification : Certification
{
    /// <summary>
    /// Id
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 认证Id
    /// </summary>
    [JsonPropertyName("certification_id")]
    public string CertificationId { get; set; } = default!;
}
