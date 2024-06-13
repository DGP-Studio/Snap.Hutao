// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.Service.Notification;

internal sealed class InfoBarOptions
{
    public InfoBarSeverity Severity { get; set; }

    public string? Title { get; set; }

    public string? Message { get; set; }

    public object? Content { get; set; }

    public string? ActionButtonContent { get; set; }

    public ICommand? ActionButtonCommand { get; set; }

    public int MilliSecondsDelay { get; set; }
}