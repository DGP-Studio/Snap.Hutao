// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core;
using Snap.Hutao.Core.ExceptionService;

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

    public static InfoBarMessage Error(string message)
    {
        return new()
        {
            Severity = InfoBarSeverity.Error,
            Message = message,
            MilliSecondsDelay = 0,
        };
    }

    public static InfoBarMessage Error(Exception exception)
    {
        return new()
        {
            Severity = InfoBarSeverity.Error,
            Title = TypeNameHelper.GetTypeDisplayName(exception),
            Message = ExceptionFormat.Format(exception),
            MilliSecondsDelay = 0,
        };
    }

    public static InfoBarMessage Warning(string message)
    {
        return new()
        {
            Severity = InfoBarSeverity.Warning,
            Message = message,
            MilliSecondsDelay = 30000,
        };
    }
}