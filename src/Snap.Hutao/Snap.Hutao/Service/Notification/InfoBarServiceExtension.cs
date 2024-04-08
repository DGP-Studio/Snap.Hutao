// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control.Builder.ButtonBase;
using Snap.Hutao.Core.Abstraction.Extension;

namespace Snap.Hutao.Service.Notification;

internal static class InfoBarServiceExtension
{
    public static void Information(this IInfoBarService infoBarService, string message, int milliSeconds = 5000)
    {
        infoBarService.Information(builder => builder.SetMessage(message).SetDelay(milliSeconds));
    }

    public static void Information(this IInfoBarService infoBarService, string title, string message, int milliSeconds = 5000)
    {
        infoBarService.Information(builder => builder.SetTitle(title).SetMessage(message).SetDelay(milliSeconds));
    }

    public static void Information(this IInfoBarService infoBarService, string title, string message, string buttonContent, ICommand buttonCommand, int milliSeconds = 5000)
    {
        infoBarService.Information(builder => builder.SetTitle(title).SetMessage(message).SetActionButton(buttonBuilder => buttonBuilder.SetContent(buttonContent).SetCommand(buttonCommand)).SetDelay(milliSeconds));
    }

    public static void Information(this IInfoBarService infoBarService, Action<IInfoBarOptionsBuilder> configure)
    {
        infoBarService.PrepareInfoBarAndShow(builder => builder.SetSeverity(InfoBarSeverity.Informational).Configure(configure));
    }

    public static void Success(this IInfoBarService infoBarService, string message, int milliSeconds = 5000)
    {
        infoBarService.Success(builder => builder.SetMessage(message).SetDelay(milliSeconds));
    }

    public static void Success(this IInfoBarService infoBarService, string title, string message, int milliSeconds = 5000)
    {
        infoBarService.Success(builder => builder.SetTitle(title).SetMessage(message).SetDelay(milliSeconds));
    }

    public static void Success(this IInfoBarService infoBarService, Action<IInfoBarOptionsBuilder> configure)
    {
        infoBarService.PrepareInfoBarAndShow(builder => builder.SetSeverity(InfoBarSeverity.Success).Configure(configure));
    }

    public static void Warning(this IInfoBarService infoBarService, string message, int milliSeconds = 30000)
    {
        infoBarService.Warning(builder => builder.SetMessage(message).SetDelay(milliSeconds));
    }

    public static void Warning(this IInfoBarService infoBarService, string title, string message, int milliSeconds = 30000)
    {
        infoBarService.Warning(builder => builder.SetTitle(title).SetMessage(message).SetDelay(milliSeconds));
    }

    public static void Warning(this IInfoBarService infoBarService, string title, string message, string buttonContent, ICommand buttonCommand, int milliSeconds = 30000)
    {
        infoBarService.Warning(builder => builder.SetTitle(title).SetMessage(message).SetActionButton(buttonBuilder => buttonBuilder.SetContent(buttonContent).SetCommand(buttonCommand)).SetDelay(milliSeconds));
    }

    public static void Warning(this IInfoBarService infoBarService, Action<IInfoBarOptionsBuilder> configure)
    {
        infoBarService.PrepareInfoBarAndShow(builder => builder.SetSeverity(InfoBarSeverity.Warning).Configure(configure));
    }

    public static void Error(this IInfoBarService infoBarService, string message, int milliSeconds = 0)
    {
        infoBarService.Error(builder => builder.SetMessage(message).SetDelay(milliSeconds));
    }

    public static void Error(this IInfoBarService infoBarService, string title, string message, int milliSeconds = 0)
    {
        infoBarService.Error(builder => builder.SetTitle(title).SetMessage(message).SetDelay(milliSeconds));
    }

    public static void Error(this IInfoBarService infoBarService, string title, string message, string buttonContent, ICommand buttonCommand, int milliSeconds = 0)
    {
        infoBarService.Error(builder => builder.SetTitle(title).SetMessage(message).SetActionButton(buttonBuilder => buttonBuilder.SetContent(buttonContent).SetCommand(buttonCommand)).SetDelay(milliSeconds));
    }

    public static void Error(this IInfoBarService infoBarService, Exception ex, int milliSeconds = 0)
    {
        infoBarService.Error(builder => builder.SetTitle(ex.GetType().Name).SetMessage(ex.Message).SetDelay(milliSeconds));
    }

    public static void Error(this IInfoBarService infoBarService, Exception ex, string subtitle, int milliSeconds = 0)
    {
        infoBarService.Error(builder => builder.SetTitle(ex.GetType().Name).SetMessage($"{subtitle}\n{ex.Message}").SetDelay(milliSeconds));
    }

    public static void Error(this IInfoBarService infoBarService, Exception ex, string subtitle, string buttonContent, ICommand buttonCommand, int milliSeconds = 0)
    {
        infoBarService.Error(builder => builder.SetTitle(ex.GetType().Name).SetMessage($"{subtitle}\n{ex.Message}").SetActionButton(buttonBuilder => buttonBuilder.SetContent(buttonContent).SetCommand(buttonCommand)).SetDelay(milliSeconds));
    }

    public static void Error(this IInfoBarService infoBarService, Action<IInfoBarOptionsBuilder> configure)
    {
        infoBarService.PrepareInfoBarAndShow(builder => builder.SetSeverity(InfoBarSeverity.Error).Configure(configure));
    }
}
