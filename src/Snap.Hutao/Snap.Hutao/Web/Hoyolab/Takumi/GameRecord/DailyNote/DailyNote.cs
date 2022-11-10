// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote;

/// <summary>
/// 实时便笺
/// </summary>
public class DailyNote
{
    /// <summary>
    /// 当前树脂
    /// </summary>
    [JsonPropertyName("current_resin")]
    public int CurrentResin { get; set; }

    /// <summary>
    /// 最大树脂
    /// </summary>
    [JsonPropertyName("max_resin")]
    public int MaxResin { get; set; }

    /// <summary>
    /// 树脂恢复时间 <see cref="string"/>类型的秒数
    /// </summary>
    [JsonPropertyName("resin_recovery_time")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int ResinRecoveryTime { get; set; }

    /// <summary>
    /// 格式化的树脂显示
    /// </summary>
    [JsonIgnore]
    public string ResinFormatted
    {
        get => $"{CurrentResin}/{MaxResin}";
    }

    /// <summary>
    /// 格式化的树脂恢复时间
    /// </summary>
    [JsonIgnore]
    public string ResinRecoveryTargetTime
    {
        get
        {
            DateTime tt = DateTime.Now.AddSeconds(ResinRecoveryTime);
            int totalDays = (tt - DateTime.Today).Days;
            string day = totalDays switch
            {
                0 => "今天",
                1 => "明天",
                2 => "后天",
                _ => $"{totalDays}天",
            };
            return $"{day} {tt:HH:mm} 全部恢复";
        }
    }

    /// <summary>
    /// 委托完成数
    /// </summary>
    [JsonPropertyName("finished_task_num")]
    public int FinishedTaskNum { get; set; }

    /// <summary>
    /// 委托总数
    /// </summary>
    [JsonPropertyName("total_task_num")]
    public int TotalTaskNum { get; set; }

    /// <summary>
    /// 格式化任务
    /// </summary>
    [JsonIgnore]
    public string TaskFormatted
    {
        get => $"{FinishedTaskNum}/{TotalTaskNum}";
    }

    /// <summary>
    /// 4次委托额外奖励是否领取
    /// </summary>
    [JsonPropertyName("is_extra_task_reward_received")]
    public bool IsExtraTaskRewardReceived { get; set; }

    /// <summary>
    /// 每日委托奖励字符串
    /// </summary>
    [JsonIgnore]
    public string ExtraTaskRewardDescription
    {
        get
        {
            return IsExtraTaskRewardReceived
                ? "已领取「每日委托」奖励"
                : FinishedTaskNum == TotalTaskNum
                    ? "「每日委托」奖励待领取"
                    : "今日完成委托次数不足";
        }
    }

    /// <summary>
    /// 剩余周本折扣次数
    /// </summary>
    [JsonPropertyName("remain_resin_discount_num")]
    public int RemainResinDiscountNum { get; set; }

    /// <summary>
    /// 周本树脂减免使用次数
    /// </summary>
    [JsonIgnore]
    public int ResinDiscountUsedNum
    {
        get => ResinDiscountNumLimit - RemainResinDiscountNum;
    }

    /// <summary>
    /// 周本折扣总次数
    /// </summary>
    [JsonPropertyName("resin_discount_num_limit")]
    public int ResinDiscountNumLimit { get; set; }

    /// <summary>
    /// 格式化周本
    /// </summary>
    public string ResinDiscountFormatted
    {
        get => $"{ResinDiscountUsedNum}/{ResinDiscountNumLimit}";
    }

    /// <summary>
    /// 当前派遣数
    /// </summary>
    [JsonPropertyName("current_expedition_num")]
    public int CurrentExpeditionNum { get; set; }

    /// <summary>
    /// 最大派遣数
    /// </summary>
    [JsonPropertyName("max_expedition_num")]
    public int MaxExpeditionNum { get; set; }

    /// <summary>
    /// 派遣
    /// </summary>
    [JsonPropertyName("expeditions")]
    public List<Expedition> Expeditions { get; set; } = default!;

    /// <summary>
    /// 当前洞天宝钱
    /// </summary>
    [JsonPropertyName("current_home_coin")]
    public int CurrentHomeCoin { get; set; }

    /// <summary>
    /// 最大洞天宝钱
    /// </summary>
    [JsonPropertyName("max_home_coin")]
    public int MaxHomeCoin { get; set; }

    /// <summary>
    /// 洞天宝钱恢复时间 <see cref="string"/>类型的秒数
    /// </summary>
    [JsonPropertyName("home_coin_recovery_time")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int HomeCoinRecoveryTime { get; set; }

    /// <summary>
    /// 格式化洞天宝钱
    /// </summary>
    [JsonIgnore]
    public string HomeCoinFormatted
    {
        get => $"{CurrentHomeCoin}/{MaxHomeCoin}";
    }

    /// <summary>
    /// 格式化的洞天宝钱恢复时间
    /// </summary>
    [JsonIgnore]
    public string HomeCoinRecoveryTargetTimeFormatted
    {
        get
        {
            DateTime reach = DateTime.Now.AddSeconds(HomeCoinRecoveryTime);
            int totalDays = (reach - DateTime.Today).Days;
            string day = totalDays switch
            {
                0 => "今天",
                1 => "明天",
                2 => "后天",
                _ => $"{totalDays}天",
            };
            return $"{day} {reach:HH:mm} 达到上限";
        }
    }

    /// <summary>
    /// 日历链接
    /// </summary>
    [JsonPropertyName("calendar_url")]
    public string CalendarUrl { get; set; } = default!;

    /// <summary>
    /// 参量质变仪
    /// </summary>
    [JsonPropertyName("transformer")]
    public Transformer Transformer { get; set; } = default!;
}