// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote;

/// <summary>
/// 实时便笺通用
/// </summary>
internal abstract class DailyNoteCommon
{
    /// <summary>
    /// 当前派遣数
    /// </summary>
    [JsonPropertyName("current_expedition_num")]
    public int CurrentExpeditionNum { get; set; }

    /// <summary>
    /// 当前洞天宝钱
    /// </summary>
    [JsonPropertyName("current_home_coin")]
    public int CurrentHomeCoin { get; set; }

    /// <summary>
    /// 当前树脂
    /// </summary>
    [JsonPropertyName("current_resin")]
    public int CurrentResin { get; set; }

    /// <summary>
    /// 派遣
    /// </summary>
    [JsonPropertyName("expeditions")]
    public List<Expedition> Expeditions { get; set; } = default!;

    /// <summary>
    /// 委托完成数
    /// </summary>
    [JsonPropertyName("finished_task_num")]
    public int FinishedTaskNum { get; set; }

    /// <summary>
    /// 4次委托额外奖励是否领取
    /// </summary>
    [JsonPropertyName("is_extra_task_reward_received")]
    public bool IsExtraTaskRewardReceived { get; set; }

    /// <summary>
    /// 最大派遣数
    /// </summary>
    [JsonPropertyName("max_expedition_num")]
    public int MaxExpeditionNum { get; set; }

    /// <summary>
    /// 最大洞天宝钱
    /// </summary>
    [JsonPropertyName("max_home_coin")]
    public int MaxHomeCoin { get; set; }

    /// <summary>
    /// 最大树脂
    /// </summary>
    [JsonPropertyName("max_resin")]
    public int MaxResin { get; set; }

    /// <summary>
    /// 树脂恢复时间的秒数
    /// </summary>
    [JsonPropertyName("resin_recovery_time")]
    public int ResinRecoveryTime { get; set; }

    /// <summary>
    /// 委托总数
    /// </summary>
    [JsonPropertyName("total_task_num")]
    public int TotalTaskNum { get; set; }
}
