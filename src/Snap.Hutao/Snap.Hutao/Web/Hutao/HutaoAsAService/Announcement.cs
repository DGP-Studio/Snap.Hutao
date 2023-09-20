// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.HutaoAsAService;

/// <summary>
/// 胡桃公告
/// </summary>
internal sealed class Announcement : UploadAnnouncement
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 语言
    /// </summary>
    public string Locale { get; set; } = default!;

    /// <summary>
    /// 最后更新日期
    /// </summary>
    public long LastUpdateTime { get; set; }

    public string UpdateTimeFormatted { get => $"{DateTimeOffset.FromUnixTimeSeconds(LastUpdateTime).ToLocalTime():yyyy.MM.dd HH:mm:ss}"; }

    public ICommand? DismissCommand { get; set; }
}