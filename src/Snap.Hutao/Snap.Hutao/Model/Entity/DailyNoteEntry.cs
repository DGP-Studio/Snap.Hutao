// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Model.Entity.Abstraction;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

/// <summary>
/// 实时便笺入口
/// </summary>
[HighQuality]
[Table("daily_notes")]
internal sealed class DailyNoteEntry : ObservableObject, IMappingFrom<DailyNoteEntry, UserAndUid>, IAppDbEntity
{
    /// <summary>
    /// 内部Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    /// <summary>
    /// 用户Id
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 用户
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = default!;

    /// <summary>
    /// Uid
    /// </summary>
    public string Uid { get; set; } = default!;

    /// <summary>
    /// 玩家角色
    /// </summary>
    [NotMapped]
    public UserGameRole? UserGameRole { get; set; }

    /// <summary>
    /// Json!!! 实时便笺
    /// </summary>
    public DailyNote? DailyNote { get; set; }

    [NotMapped]
    public List<ChapterId> ArchonQuestIds { get; set; } = default!;

    [NotMapped]
    public int ArchonQuestStatusValue
    {
        get
        {
            if (DailyNote is { ArchonQuestProgress.List: { Count: > 0 } list })
            {
                return ArchonQuestIds.IndexOf(list.Single().Id);
            }

            return ArchonQuestIds.Count;
        }
    }

    [NotMapped]
    public string ArchonQuestStatusFormatted
    {
        get
        {
            if (DailyNote is { ArchonQuestProgress.List: { Count: > 0 } list })
            {
                return list.Single().Status.GetLocalizedDescription();
            }

            return SH.WebDailyNoteArchonQuestStatusFinished;
        }
    }

    [NotMapped]
    public string ArchonQuestChapterFormatted
    {
        get
        {
            if (DailyNote is { ArchonQuestProgress.List: { Count: > 0 } list })
            {
                ArchonQuest quest = list.Single();
                return $"{quest.ChapterNum} {quest.ChapterTitle}";
            }

            return string.Empty;
        }
    }

    /// <summary>
    /// 刷新时间
    /// </summary>
    public DateTimeOffset RefreshTime { get; set; }

    [NotMapped]
    public string RefreshTimeFormatted
    {
        get
        {
            return RefreshTime == DateTimeOffsetExtension.DatebaseDefaultTime
                ? SH.ModelEntityDailyNoteNotRefreshed
                : SH.FormatModelEntityDailyNoteRefreshTimeFormat(RefreshTime.ToLocalTime());
        }
    }

    public int ResinNotifyThreshold { get; set; }

    public bool ResinNotifySuppressed { get; set; }

    public int HomeCoinNotifyThreshold { get; set; }

    public bool HomeCoinNotifySuppressed { get; set; }

    public bool TransformerNotify { get; set; }

    public bool TransformerNotifySuppressed { get; set; }

    public bool DailyTaskNotify { get; set; }

    public bool DailyTaskNotifySuppressed { get; set; }

    public bool ExpeditionNotify { get; set; }

    public bool ExpeditionNotifySuppressed { get; set; }

    public static DailyNoteEntry From(UserAndUid userAndUid)
    {
        return new()
        {
            UserId = userAndUid.User.InnerId,
            Uid = userAndUid.Uid.Value,
            ResinNotifyThreshold = 120,
            HomeCoinNotifyThreshold = 1800,
        };
    }

    public void UpdateDailyNote(DailyNote? dailyNote)
    {
        DailyNote = dailyNote;
        OnPropertyChanged(nameof(DailyNote));

        RefreshTime = DateTimeOffset.UtcNow;
        OnPropertyChanged(nameof(RefreshTimeFormatted));
    }

    public void CopyTo(DailyNoteEntry other)
    {
        other.UpdateDailyNote(DailyNote);

        other.ResinNotifySuppressed = ResinNotifySuppressed;
        other.OnPropertyChanged(nameof(ResinNotifySuppressed));

        other.HomeCoinNotifySuppressed = HomeCoinNotifySuppressed;
        other.OnPropertyChanged(nameof(HomeCoinNotifySuppressed));

        other.TransformerNotifySuppressed = TransformerNotifySuppressed;
        other.OnPropertyChanged(nameof(TransformerNotifySuppressed));

        other.DailyTaskNotifySuppressed = DailyTaskNotifySuppressed;
        other.OnPropertyChanged(nameof(DailyTaskNotifySuppressed));

        other.ExpeditionNotifySuppressed = ExpeditionNotifySuppressed;
        other.OnPropertyChanged(nameof(ExpeditionNotifySuppressed));
    }
}
