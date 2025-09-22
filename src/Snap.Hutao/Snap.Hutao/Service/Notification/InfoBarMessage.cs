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

    public static InfoBarMessage Any(InfoBarSeverity severity, string message)
    {
        return new()
        {
            Severity = severity,
            Message = message,
            MilliSecondsDelay = 5000 + TextReadingTime.Estimate(message),
        };
    }

    public static InfoBarMessage Error(string message)
    {
        return new()
        {
            Severity = InfoBarSeverity.Error,
            Message = message,
            MilliSecondsDelay = 0,
        };
    }

    public static InfoBarMessage Error(string title, string message)
    {
        return new()
        {
            Severity = InfoBarSeverity.Error,
            Title = title,
            Message = message,
            MilliSecondsDelay = 0,
        };
    }

    public static InfoBarMessage Error(string title, Exception exception)
    {
        return new()
        {
            Severity = InfoBarSeverity.Error,
            Title = title,
            Message = ExceptionFormat.Format(exception),
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

    public static InfoBarMessage Information(string message)
    {
        return new()
        {
            Severity = InfoBarSeverity.Informational,
            Message = message,
            MilliSecondsDelay = 5000 + TextReadingTime.Estimate(message),
        };
    }

    public static InfoBarMessage Success(string message)
    {
        return new()
        {
            Severity = InfoBarSeverity.Success,
            Message = message,
            MilliSecondsDelay = 5000 + TextReadingTime.Estimate(message),
        };
    }

    public static InfoBarMessage Success(string title, string message)
    {
        return new()
        {
            Severity = InfoBarSeverity.Success,
            Title = title,
            Message = message,
            MilliSecondsDelay = 5000 + TextReadingTime.Estimate(title) + TextReadingTime.Estimate(message),
        };
    }

    public static InfoBarMessage Warning(string message)
    {
        return new()
        {
            Severity = InfoBarSeverity.Warning,
            Message = message,
            MilliSecondsDelay = 5000 + TextReadingTime.Estimate(message),
        };
    }

    public static InfoBarMessage Warning(string title, string message)
    {
        return new()
        {
            Severity = InfoBarSeverity.Warning,
            Title = title,
            Message = message,
            MilliSecondsDelay = 5000 + TextReadingTime.Estimate(title) + TextReadingTime.Estimate(message),
        };
    }

    public static InfoBarMessage Warning(string title, string message, string actionButtonContent, ICommand actionButtonCommand)
    {
        return new()
        {
            Severity = InfoBarSeverity.Warning,
            Title = title,
            Message = message,
            ActionButtonContent = actionButtonContent,
            ActionButtonCommand = actionButtonCommand,
            MilliSecondsDelay = 5000 + TextReadingTime.Estimate(title) + TextReadingTime.Estimate(message),
        };
    }
}