// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.Web.Hutao.HutaoAsAService;

internal class UploadAnnouncement
{
    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; } = default!;

    /// <summary>
    /// 内容
    /// </summary>
    public string Content { get; set; } = default!;

    /// <summary>
    /// 严重度
    /// </summary>
    public InfoBarSeverity Severity { get; set; } = InfoBarSeverity.Informational;

    /// <summary>
    /// 原帖链接
    /// </summary>
    public string Link { get; set; } = default!;
}