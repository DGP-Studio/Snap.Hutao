// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.Abstraction.Extension;

namespace Snap.Hutao.Service.Notification;

internal static class InfoBarOptionsBuilderExtension
{
    public static IInfoBarOptionsBuilder SetSeverity(this IInfoBarOptionsBuilder builder, InfoBarSeverity severity)
    {
        builder.Configure(builder => builder.Options.Severity = severity);
        return builder;
    }

    public static IInfoBarOptionsBuilder SetTitle(this IInfoBarOptionsBuilder builder, string? title)
    {
        builder.Configure(builder => builder.Options.Title = title);
        return builder;
    }

    public static IInfoBarOptionsBuilder SetMessage(this IInfoBarOptionsBuilder builder, string? message)
    {
        builder.Configure(builder => builder.Options.Message = message);
        return builder;
    }

    public static IInfoBarOptionsBuilder SetContent(this IInfoBarOptionsBuilder builder, object? content)
    {
        builder.Configure(builder => builder.Options.Content = content);
        return builder;
    }

    public static IInfoBarOptionsBuilder SetDelay(this IInfoBarOptionsBuilder builder, int milliSeconds)
    {
        builder.Configure(builder => builder.Options.MilliSecondsDelay = milliSeconds);
        return builder;
    }
}