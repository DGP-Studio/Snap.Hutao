// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
