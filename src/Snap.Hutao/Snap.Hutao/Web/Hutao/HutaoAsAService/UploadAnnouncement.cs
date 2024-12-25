// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.Web.Hutao.HutaoAsAService;

internal class UploadAnnouncement
{
    public string Title { get; set; } = default!;

    public string Content { get; set; } = default!;

    public InfoBarSeverity Severity { get; set; } = InfoBarSeverity.Informational;

    public string Link { get; set; } = default!;

    public string? MaxPresentVersion { get; set; }
}