// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.Service.Notification;

internal static class InfoBarServiceExtension
{
    public static void Information(this IInfoBarService infoBarService, string message, int delay = 5000)
    {
        infoBarService.PrepareInfoBarAndShow(InfoBarSeverity.Informational, null, message, delay);
    }

    public static void Information(this IInfoBarService infoBarService, string title, string message, int delay = 5000)
    {
        infoBarService.PrepareInfoBarAndShow(InfoBarSeverity.Informational, title, message, delay);
    }

    public static void Success(this IInfoBarService infoBarService, string message, int delay = 5000)
    {
        infoBarService.PrepareInfoBarAndShow(InfoBarSeverity.Success, null, message, delay);
    }

    public static void Success(this IInfoBarService infoBarService, string title, string message, int delay = 5000)
    {
        infoBarService.PrepareInfoBarAndShow(InfoBarSeverity.Success, title, message, delay);
    }

    public static void Warning(this IInfoBarService infoBarService, string message, int delay = 30000)
    {
        infoBarService.PrepareInfoBarAndShow(InfoBarSeverity.Warning, null, message, delay);
    }

    public static void Warning(this IInfoBarService infoBarService, string title, string message, int delay = 30000)
    {
        infoBarService.PrepareInfoBarAndShow(InfoBarSeverity.Warning, title, message, delay);
    }

    public static void Error(this IInfoBarService infoBarService, string message, int delay = 0)
    {
        infoBarService.PrepareInfoBarAndShow(InfoBarSeverity.Error, null, message, delay);
    }

    public static void Error(this IInfoBarService infoBarService, string title, string message, int delay = 0)
    {
        infoBarService.PrepareInfoBarAndShow(InfoBarSeverity.Error, title, message, delay);
    }

    public static void Error(this IInfoBarService infoBarService, Exception ex, int delay = 0)
    {
        infoBarService.PrepareInfoBarAndShow(InfoBarSeverity.Error, ex.GetType().Name, ex.Message, delay);
    }

    public static void Error(this IInfoBarService infoBarService, Exception ex, string title, int delay = 0)
    {
        infoBarService.PrepareInfoBarAndShow(InfoBarSeverity.Error, ex.GetType().Name, $"{title}\n{ex.Message}", delay);
    }
}
