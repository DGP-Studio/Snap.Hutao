// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.Service.Notification;

internal sealed class InfoBarMessage
{
    public InfoBarSeverity Severity { get; init; }

    public string? Title { get; init; }

    public string? Message { get; init; }

    public object? Content { get; init; }

    public string? ActionButtonContent { get; init; }

    public ICommand? ActionButtonCommand { get; init; }

    public int MilliSecondsDelay { get; init; }
}