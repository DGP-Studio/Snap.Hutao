// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using JetBrains.Annotations;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.Abstraction;

namespace Snap.Hutao.Service.Notification;

internal static class InfoBarServiceExtension
{
    public static Void Information(this IInfoBarService infoBarService, [LocalizationRequired] string message, int milliSeconds = 5000)
    {
        return infoBarService.Information(builder => builder.SetMessage(message).SetDelay(milliSeconds));
    }

    public static Void Information(this IInfoBarService infoBarService, [LocalizationRequired] string title, [LocalizationRequired] string message, int milliSeconds = 5000)
    {
        return infoBarService.Information(builder => builder.SetTitle(title).SetMessage(message).SetDelay(milliSeconds));
    }

    public static Void Information(this IInfoBarService infoBarService, [LocalizationRequired] string title, [LocalizationRequired] string message, [LocalizationRequired] string buttonContent, ICommand buttonCommand, int milliSeconds = 5000)
    {
        return infoBarService.Information(builder => builder.SetTitle(title).SetMessage(message).SetActionButtonContent(buttonContent).SetActionButtonCommand(buttonCommand).SetDelay(milliSeconds));
    }

    public static Void Information(this IInfoBarService infoBarService, Action<IInfoBarOptionsBuilder> configure)
    {
        infoBarService.PrepareInfoBarAndShow(builder => builder.SetSeverity(InfoBarSeverity.Informational).Configure(configure));
        return default;
    }

    public static Void Success(this IInfoBarService infoBarService, [LocalizationRequired] string message, int milliSeconds = 5000)
    {
        return infoBarService.Success(builder => builder.SetMessage(message).SetDelay(milliSeconds));
    }

    public static Void Success(this IInfoBarService infoBarService, [LocalizationRequired] string title, [LocalizationRequired] string message, int milliSeconds = 5000)
    {
        return infoBarService.Success(builder => builder.SetTitle(title).SetMessage(message).SetDelay(milliSeconds));
    }

    public static Void Success(this IInfoBarService infoBarService, Action<IInfoBarOptionsBuilder> configure)
    {
        infoBarService.PrepareInfoBarAndShow(builder => builder.SetSeverity(InfoBarSeverity.Success).Configure(configure));
        return default;
    }

    public static Void Warning(this IInfoBarService infoBarService, [LocalizationRequired] string message, int milliSeconds = 30000)
    {
        return infoBarService.Warning(builder => builder.SetMessage(message).SetDelay(milliSeconds));
    }

    public static Void Warning(this IInfoBarService infoBarService, [LocalizationRequired] string title, [LocalizationRequired] string message, int milliSeconds = 30000)
    {
        return infoBarService.Warning(builder => builder.SetTitle(title).SetMessage(message).SetDelay(milliSeconds));
    }

    public static Void Warning(this IInfoBarService infoBarService, [LocalizationRequired] string title, [LocalizationRequired] string message, [LocalizationRequired] string buttonContent, ICommand buttonCommand, int milliSeconds = 30000)
    {
        return infoBarService.Warning(builder => builder.SetTitle(title).SetMessage(message).SetActionButtonContent(buttonContent).SetActionButtonCommand(buttonCommand).SetDelay(milliSeconds));
    }

    public static Void Warning(this IInfoBarService infoBarService, Action<IInfoBarOptionsBuilder> configure)
    {
        infoBarService.PrepareInfoBarAndShow(builder => builder.SetSeverity(InfoBarSeverity.Warning).Configure(configure));
        return default;
    }

    public static Void Error(this IInfoBarService infoBarService, [LocalizationRequired] string message, int milliSeconds = 0)
    {
        return infoBarService.Error(builder => builder.SetMessage(message).SetDelay(milliSeconds));
    }

    public static Void Error(this IInfoBarService infoBarService, [LocalizationRequired] string title, [LocalizationRequired] string message, int milliSeconds = 0)
    {
        return infoBarService.Error(builder => builder.SetTitle(title).SetMessage(message).SetDelay(milliSeconds));
    }

    public static Void Error(this IInfoBarService infoBarService, [LocalizationRequired] string title, [LocalizationRequired] string message, [LocalizationRequired] string buttonContent, ICommand buttonCommand, int milliSeconds = 0)
    {
        return infoBarService.Error(builder => builder.SetTitle(title).SetMessage(message).SetActionButtonContent(buttonContent).SetActionButtonCommand(buttonCommand).SetDelay(milliSeconds));
    }

    public static Void Error(this IInfoBarService infoBarService, Exception ex, int milliSeconds = 0)
    {
        return infoBarService.Error(builder => builder.SetTitle(ex.GetType().Name).SetMessage(ex.Message).SetDelay(milliSeconds));
    }

    public static Void Error(this IInfoBarService infoBarService, Exception ex, [LocalizationRequired] string subtitle, int milliSeconds = 0)
    {
        return infoBarService.Error(builder => builder.SetTitle(ex.GetType().Name).SetMessage($"{subtitle}\n{ex.Message}").SetDelay(milliSeconds));
    }

    public static Void Error(this IInfoBarService infoBarService, Exception ex, [LocalizationRequired] string subtitle, [LocalizationRequired] string buttonContent, ICommand buttonCommand, int milliSeconds = 0)
    {
        return infoBarService.Error(builder => builder.SetTitle(ex.GetType().Name).SetMessage($"{subtitle}\n{ex.Message}").SetActionButtonContent(buttonContent).SetActionButtonCommand(buttonCommand).SetDelay(milliSeconds));
    }

    public static Void Error(this IInfoBarService infoBarService, Action<IInfoBarOptionsBuilder> configure)
    {
        infoBarService.PrepareInfoBarAndShow(builder => builder.SetSeverity(InfoBarSeverity.Error).Configure(configure));
        return default;
    }
}