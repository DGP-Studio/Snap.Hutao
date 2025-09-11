// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using JetBrains.Annotations;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Core.ExceptionService;
using System.Text;

namespace Snap.Hutao.Service.Notification;

internal static class InfoBarServiceExtension
{
    [Obsolete("Use InfoBarMessage instead")]
    public static Void Information(this IInfoBarService infoBarService, [LocalizationRequired] string message, int milliSeconds = 5000)
    {
        return infoBarService.Information(builder => builder.SetMessage(message).SetDelay(milliSeconds));
    }

    [Obsolete("Use InfoBarMessage instead")]
    public static Void Information(this IInfoBarService infoBarService, [LocalizationRequired] string title, [LocalizationRequired] string message, int milliSeconds = 5000)
    {
        return infoBarService.Information(builder => builder.SetTitle(title).SetMessage(message).SetDelay(milliSeconds));
    }

    [Obsolete("Use InfoBarMessage instead")]
    public static Void Information(this IInfoBarService infoBarService, [LocalizationRequired] string title, [LocalizationRequired] string message, [LocalizationRequired] string buttonContent, ICommand buttonCommand, int milliSeconds = 5000)
    {
        return infoBarService.Information(builder => builder.SetTitle(title).SetMessage(message).SetActionButtonContent(buttonContent).SetActionButtonCommand(buttonCommand).SetDelay(milliSeconds));
    }

    [Obsolete("Use InfoBarMessage instead")]
    public static Void Information(this IInfoBarService infoBarService, Action<IInfoBarOptionsBuilder> configure)
    {
        infoBarService.PrepareInfoBarAndShow(builder => builder.SetSeverity(InfoBarSeverity.Informational).Configure(configure));
        return default;
    }

    [Obsolete("Use InfoBarMessage instead")]
    public static Void Success(this IInfoBarService infoBarService, [LocalizationRequired] string message, int milliSeconds = 5000)
    {
        return infoBarService.Success(builder => builder.SetMessage(message).SetDelay(milliSeconds));
    }

    [Obsolete("Use InfoBarMessage instead")]
    public static Void Success(this IInfoBarService infoBarService, [LocalizationRequired] string title, [LocalizationRequired] string message, int milliSeconds = 5000)
    {
        return infoBarService.Success(builder => builder.SetTitle(title).SetMessage(message).SetDelay(milliSeconds));
    }

    [Obsolete("Use InfoBarMessage instead")]
    public static Void Success(this IInfoBarService infoBarService, Action<IInfoBarOptionsBuilder> configure)
    {
        infoBarService.PrepareInfoBarAndShow(builder => builder.SetSeverity(InfoBarSeverity.Success).Configure(configure));
        return default;
    }

    [Obsolete("Use InfoBarMessage instead")]
    public static Void Warning(this IInfoBarService infoBarService, [LocalizationRequired] string message, int milliSeconds = 30000)
    {
        return infoBarService.Warning(builder => builder.SetMessage(message).SetDelay(milliSeconds));
    }

    [Obsolete("Use InfoBarMessage instead")]
    public static Void Warning(this IInfoBarService infoBarService, [LocalizationRequired] string title, [LocalizationRequired] string message, int milliSeconds = 30000)
    {
        return infoBarService.Warning(builder => builder.SetTitle(title).SetMessage(message).SetDelay(milliSeconds));
    }

    [Obsolete("Use InfoBarMessage instead")]
    public static Void Warning(this IInfoBarService infoBarService, [LocalizationRequired] string title, [LocalizationRequired] string message, [LocalizationRequired] string buttonContent, ICommand buttonCommand, int milliSeconds = 30000)
    {
        return infoBarService.Warning(builder => builder.SetTitle(title).SetMessage(message).SetActionButtonContent(buttonContent).SetActionButtonCommand(buttonCommand).SetDelay(milliSeconds));
    }

    [Obsolete("Use InfoBarMessage instead")]
    public static Void Warning(this IInfoBarService infoBarService, Action<IInfoBarOptionsBuilder> configure)
    {
        infoBarService.PrepareInfoBarAndShow(builder => builder.SetSeverity(InfoBarSeverity.Warning).Configure(configure));
        return default;
    }

    [Obsolete("Use InfoBarMessage instead")]
    public static Void Error(this IInfoBarService infoBarService, [LocalizationRequired] string message, int milliSeconds = 0)
    {
        return infoBarService.Error(builder => builder.SetMessage(message).SetDelay(milliSeconds));
    }

    [Obsolete("Use InfoBarMessage instead")]
    public static Void Error(this IInfoBarService infoBarService, [LocalizationRequired] string title, [LocalizationRequired] string message, int milliSeconds = 0)
    {
        return infoBarService.Error(builder => builder.SetTitle(title).SetMessage(message).SetDelay(milliSeconds));
    }

    [Obsolete("Use InfoBarMessage instead")]
    public static Void Error(this IInfoBarService infoBarService, [LocalizationRequired] string title, [LocalizationRequired] string message, [LocalizationRequired] string buttonContent, ICommand buttonCommand, int milliSeconds = 0)
    {
        return infoBarService.Error(builder => builder.SetTitle(title).SetMessage(message).SetActionButtonContent(buttonContent).SetActionButtonCommand(buttonCommand).SetDelay(milliSeconds));
    }

    [Obsolete("Use InfoBarMessage instead")]
    public static Void Error(this IInfoBarService infoBarService, Exception ex, int milliSeconds = 0)
    {
        return infoBarService.Error(builder => builder.SetTitle(ex.GetType().Name).SetMessage(ExceptionFormat.Format(ex)).SetDelay(milliSeconds));
    }

    [Obsolete("Use InfoBarMessage instead")]
    public static Void Error(this IInfoBarService infoBarService, Exception ex, [LocalizationRequired] string subtitle, int milliSeconds = 0)
    {
        // ReSharper disable once LocalizableElement
        return infoBarService.Error(builder => builder.SetTitle(ex.GetType().Name).SetMessage(ExceptionFormat.Format(new StringBuilder().AppendLine(subtitle), ex).ToString()).SetDelay(milliSeconds));
    }

    [Obsolete("Use InfoBarMessage instead")]
    public static Void Error(this IInfoBarService infoBarService, Exception ex, [LocalizationRequired] string subtitle, [LocalizationRequired] string buttonContent, ICommand buttonCommand, int milliSeconds = 0)
    {
        // ReSharper disable once LocalizableElement
        return infoBarService.Error(builder => builder.SetTitle(ex.GetType().Name).SetMessage(ExceptionFormat.Format(new StringBuilder().AppendLine(subtitle), ex).ToString()).SetActionButtonContent(buttonContent).SetActionButtonCommand(buttonCommand).SetDelay(milliSeconds));
    }

    [Obsolete("Use InfoBarMessage instead")]
    public static Void Error(this IInfoBarService infoBarService, Action<IInfoBarOptionsBuilder> configure)
    {
        infoBarService.PrepareInfoBarAndShow(builder => builder.SetSeverity(InfoBarSeverity.Error).Configure(configure));
        return default;
    }
}