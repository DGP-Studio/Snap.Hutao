// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.Web.Hutao.HutaoAsAService;

internal sealed class Announcement : UploadAnnouncement
{
    public long Id { get; set; }

    public string Locale { get; set; } = default!;

    public long LastUpdateTime { get; set; }

    public string UpdateTimeFormatted { get => $"{DateTimeOffset.FromUnixTimeSeconds(LastUpdateTime).ToLocalTime():yyyy.MM.dd HH:mm:ss}"; }

    public ICommand? DismissCommand { get; set; }

    public bool CanDismiss { get => Severity <= InfoBarSeverity.Success; }
}