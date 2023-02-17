// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher;

/// <summary>
/// 差异文件
/// </summary>
[HighQuality]
internal sealed class DiffPackage : Package
{
    /// <summary>
    /// 是否为推荐更新
    /// </summary>
    [JsonPropertyName("is_recommended_update")]
    public bool IsRecommendedUpdate { get; set; }
}
