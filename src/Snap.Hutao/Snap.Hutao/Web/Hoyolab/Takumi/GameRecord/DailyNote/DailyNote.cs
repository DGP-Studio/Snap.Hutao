// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote;

internal sealed class DailyNote : DailyNoteCommon, IJsonOnDeserialized
{
    #region Binding
    [JsonIgnore]
    public string FormattedResin
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

            DateTimeOffset reach = DeserializeTime.AddSeconds(ResinRecoveryTime);
            return SH.FormatWebDailyNoteResinRecoveryTargetTime(reach);
        }
    }

    [JsonIgnore]
    public string FormattedTask
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
    public string FormattedResinDiscount
    {
        get => $"{ResinDiscountUsedNum}/{ResinDiscountNumLimit}";
    }

    [JsonIgnore]
    public string FormattedHomeCoin
    {
        get => MaxHomeCoin == 0 ? SH.WebDailyNoteHomeLocked : $"{CurrentHomeCoin}/{MaxHomeCoin}";
    }

    [JsonIgnore]
    public string FormattedHomeCoinRecoveryTargetTime
    {
        get
        {
            if (HomeCoinRecoveryTime == 0)
            {
                return SH.WebDailyNoteHomeCoinRecoveryCompleted;
            }

            DateTimeOffset reach = DeserializeTime.AddSeconds(HomeCoinRecoveryTime);
            return SH.FormatWebDailyNoteHomeCoinRecoveryTargetTime(reach);
        }
    }

    [JsonIgnore]
    public bool IsArchonQuestFinished
    {
        get => ArchonQuestProgress.List.Count == 0;
    }
    #endregion

    [JsonPropertyName("remain_resin_discount_num")]
    public int RemainResinDiscountNum { get; init; }

    [JsonPropertyName("resin_discount_num_limit")]
    public int ResinDiscountNumLimit { get; init; }

    [JsonPropertyName("home_coin_recovery_time")]
    public int HomeCoinRecoveryTime { get; init; }

    [JsonPropertyName("calendar_url")]
    public string CalendarUrl { get; init; } = default!;

    [JsonPropertyName("transformer")]
    public Transformer Transformer { get; init; } = default!;

    [JsonPropertyName("daily_task")]
    public DailyTask DailyTask { get; init; } = default!;

    [JsonPropertyName("archon_quest_progress")]
    public ArchonQuestProgress ArchonQuestProgress { get; init; } = default!;

    private DateTimeOffset DeserializeTime { get; set; }

    public void OnDeserialized()
    {
        DeserializeTime = DateTimeOffset.Now;
    }
}