// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote;

/// <summary>
/// 参量质变仪
/// </summary>
public class Transformer
{
    /// <summary>
    /// 是否拥有该道具
    /// </summary>
    [JsonPropertyName("obtained")]
    public bool Obtained { get; set; }

    /// <summary>
    /// 恢复时间包装
    /// </summary>
    [JsonPropertyName("recovery_time")]
    public RecoveryTime? RecoveryTime { get; set; }

    /// <summary>
    /// Wiki链接
    /// </summary>
    [JsonPropertyName("wiki")]
    public Uri Wiki { get; set; } = default!;

    /// <summary>
    /// 是否提醒
    /// </summary>
    [JsonPropertyName("noticed")]
    public bool Noticed { get; set; }

    /// <summary>
    /// 上个任务的Id
    /// </summary>
    [JsonPropertyName("latest_job_id")]
    public string LastJobId { get; set; } = default!;
}
