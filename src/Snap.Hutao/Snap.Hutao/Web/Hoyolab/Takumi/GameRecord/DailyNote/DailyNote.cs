// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote;

/// <summary>
/// 实时便笺
/// </summary>
[HighQuality]
[SuppressMessage("", "SA1124")]
internal sealed class DailyNote : DailyNoteCommon
{
    #region Binding
    [JsonIgnore]
    public string ResinFormatted
    {
        get => $"{CurrentResin}/{MaxResin}";
    }

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
                _ => SH.FormatWebDailyNoteRecoveryTimeDayFormat(totalDays),
            };

            return SH.FormatWebDailyNoteResinRecoveryFormat(day, reach);
        }
    }

    [JsonIgnore]
    public string TaskFormatted
    {
        get => $"{FinishedTaskNum}/{TotalTaskNum}";
    }

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

    [JsonIgnore]
    public int ResinDiscountUsedNum
    {
        get => ResinDiscountNumLimit - RemainResinDiscountNum;
    }

    [JsonIgnore]
    public string ResinDiscountFormatted
    {
        get => $"{ResinDiscountUsedNum}/{ResinDiscountNumLimit}";
    }

    [JsonIgnore]
    public string HomeCoinFormatted
    {
        get => MaxHomeCoin == 0 ? SH.WebDailyNoteHomeLocked : $"{CurrentHomeCoin}/{MaxHomeCoin}";
    }

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
                _ => SH.FormatWebDailyNoteRecoveryTimeDayFormat(totalDays),
            };
            return SH.FormatWebDailyNoteHomeCoinRecoveryFormat(day, reach);
        }
    }
    #endregion

    /// <summary>
    /// 剩余周本折扣次数
    /// </summary>
    [JsonPropertyName("remain_resin_discount_num")]
    public int RemainResinDiscountNum { get; set; }

    /// <summary>
    /// 周本折扣总次数
    /// </summary>
    [JsonPropertyName("resin_discount_num_limit")]
    public int ResinDiscountNumLimit { get; set; }

    /// <summary>
    /// 洞天宝钱恢复时间 <see cref="string"/>类型的秒数
    /// </summary>
    [JsonPropertyName("home_coin_recovery_time")]
    public int HomeCoinRecoveryTime { get; set; }

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

    [JsonPropertyName("daily_task")]
    public DailyTask DailyTask { get; set; } = default!;

    [JsonPropertyName("archon_quest_progress")]
    public ArchonQuestProgress ArchonQuestProgress { get; set; } = default!;
}