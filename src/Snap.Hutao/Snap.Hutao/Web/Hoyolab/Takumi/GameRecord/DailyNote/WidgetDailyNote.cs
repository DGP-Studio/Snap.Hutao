// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote;

/// <summary>
/// 实时便笺小组件数据
/// </summary>
internal sealed class WidgetDailyNote : DailyNoteCommon
{
    /// <summary>
    /// 是否签到
    /// </summary>
    [JsonPropertyName("has_signed")]
    public bool HasSigned { get; set; }

    /// <summary>
    /// 签到页面链接
    /// </summary>
    [JsonPropertyName("sign_url")]
    public Uri SignUrl { get; set; } = default!;
}