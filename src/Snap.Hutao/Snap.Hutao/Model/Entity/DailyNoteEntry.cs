// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
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
internal sealed class DailyNoteEntry : ObservableObject
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

    /// <summary>
    /// 树脂提醒阈值
    /// </summary>
    public int ResinNotifyThreshold { get; set; }

    /// <summary>
    /// 用于判断树脂是否继续提醒
    /// </summary>
    public bool ResinNotifySuppressed { get; set; }

    /// <summary>
    /// 洞天宝钱提醒阈值
    /// </summary>
    public int HomeCoinNotifyThreshold { get; set; }

    /// <summary>
    /// 用于判断洞天宝钱是否继续提醒
    /// </summary>
    public bool HomeCoinNotifySuppressed { get; set; }

    /// <summary>
    /// 参量质变仪提醒
    /// </summary>
    public bool TransformerNotify { get; set; }

    /// <summary>
    /// 用于判断参量质变仪是否继续提醒
    /// </summary>
    public bool TransformerNotifySuppressed { get; set; }

    /// <summary>
    /// 每日委托提醒
    /// </summary>
    public bool DailyTaskNotify { get; set; }

    /// <summary>
    /// 用于判断每日委托是否继续提醒
    /// </summary>
    public bool DailyTaskNotifySuppressed { get; set; }

    /// <summary>
    /// 探索派遣提醒
    /// </summary>
    public bool ExpeditionNotify { get; set; }

    /// <summary>
    /// 用于判断探索派遣是否继续提醒
    /// </summary>
    public bool ExpeditionNotifySuppressed { get; set; }

    /// <summary>
    /// 构造一个新的实时便笺
    /// </summary>
    /// <param name="userAndUid">用户与角色</param>
    /// <returns>新的实时便笺</returns>
    public static DailyNoteEntry Create(UserAndUid userAndUid)
    {
        return new()
        {
            UserId = userAndUid.User.InnerId,
            Uid = userAndUid.Uid.Value,
            ResinNotifyThreshold = 120,
            HomeCoinNotifyThreshold = 1800,
        };
    }

    /// <summary>
    /// 更新实时便笺
    /// </summary>
    /// <param name="dailyNote">新的值</param>
    public void UpdateDailyNote(DailyNote? dailyNote)
    {
        DailyNote = dailyNote;
        OnPropertyChanged(nameof(DailyNote));
    }
}