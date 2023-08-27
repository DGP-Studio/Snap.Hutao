// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote;

/// <summary>
/// 实时便笺
/// </summary>
[HighQuality]
internal sealed class DailyNote : DailyNoteCommon
{
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
            if (ResinRecoveryTime == 0)
            {
                return SH.WebDailyNoteResinRecoveryCompleted;
            }

            DateTime reach = DateTime.Now.AddSeconds(ResinRecoveryTime);
            int totalDays = (reach - DateTime.Today).Days;
            string day = totalDays switch
            {
                0 => SH.WebDailyNoteRecoveryTimeDay0,
                1 => SH.WebDailyNoteRecoveryTimeDay1,
                2 => SH.WebDailyNoteRecoveryTimeDay2,
                _ => SH.WebDailyNoteRecoveryTimeDayFormat.Format(totalDays),
            };

            return SH.WebDailyNoteResinRecoveryFormat.Format(day, reach);
        }
    }

    /// <summary>
    /// 格式化任务
    /// </summary>
    [JsonIgnore]
    public string TaskFormatted
    {
        get => $"{FinishedTaskNum}/{TotalTaskNum}";
    }

    /// <summary>
    /// 每日委托奖励字符串
    /// </summary>
    [JsonIgnore]
    public string ExtraTaskRewardDescription
    {
        get
        {
            return IsExtraTaskRewardReceived
                ? SH.WebDailyNoteExtraTaskRewardReceived
                : FinishedTaskNum == TotalTaskNum
                    ? SH.WebDailyNoteExtraTaskRewardNotTaken
                    : SH.WebDailyNoteExtraTaskRewardNotAllowed;
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
    /// 洞天宝钱恢复时间 <see cref="string"/>类型的秒数
    /// </summary>
    [JsonPropertyName("home_coin_recovery_time")]
    public int HomeCoinRecoveryTime { get; set; }

    /// <summary>
    /// 格式化洞天宝钱
    /// </summary>
    [JsonIgnore]
    public string HomeCoinFormatted
    {
        get => MaxHomeCoin == 0 ? SH.WebDailyNoteHomeLocked : $"{CurrentHomeCoin}/{MaxHomeCoin}";
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
                0 => SH.WebDailyNoteRecoveryTimeDay0,
                1 => SH.WebDailyNoteRecoveryTimeDay1,
                2 => SH.WebDailyNoteRecoveryTimeDay2,
                _ => SH.WebDailyNoteRecoveryTimeDayFormat.Format(totalDays),
            };
            return SH.WebDailyNoteHomeCoinRecoveryFormat.Format(day, reach);
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