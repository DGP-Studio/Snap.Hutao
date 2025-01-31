// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using JetBrains.Annotations;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Snap.Hutao.Core.Abstraction;

namespace Snap.Hutao.Service.Notification;

internal static class InfoBarOptionsBuilderExtension
{
    public static TBuilder SetSeverity<TBuilder>(this TBuilder builder, InfoBarSeverity severity)
        where TBuilder : IInfoBarOptionsBuilder
    {
        builder.Configure(builder => builder.Options.Severity = severity);
        return builder;
    }

    public static TBuilder SetTitle<TBuilder>(this TBuilder builder, [LocalizationRequired] string? title)
        where TBuilder : IInfoBarOptionsBuilder
    {
        builder.Configure(builder => builder.Options.Title = title);
        return builder;
    }

    public static IInfoBarOptionsBuilder SetMessage<TBuilder>(this TBuilder builder, [LocalizationRequired] string? message)
        where TBuilder : IInfoBarOptionsBuilder
    {
        builder.Configure(builder => builder.Options.Message = message);
        return builder;
    }

    public static IInfoBarOptionsBuilder SetContent<TBuilder>(this TBuilder builder, string xaml)
        where TBuilder : IInfoBarOptionsBuilder
    {
        builder.Configure(builder => builder.Options.Content = XamlReader.Load(xaml));
        return builder;
    }

    public static IInfoBarOptionsBuilder SetActionButtonContent<TBuilder>(this TBuilder builder, [LocalizationRequired] string? buttonContent)
        where TBuilder : IInfoBarOptionsBuilder
    {
        builder.Configure(builder => builder.Options.ActionButtonContent = buttonContent);
        return builder;
    }

    public static IInfoBarOptionsBuilder SetActionButtonCommand<TBuilder>(this TBuilder builder, ICommand? buttonCommand)
        where TBuilder : IInfoBarOptionsBuilder
    {
        builder.Configure(builder => builder.Options.ActionButtonCommand = buttonCommand);
        return builder;
    }

    public static IInfoBarOptionsBuilder SetDelay<TBuilder>(this TBuilder builder, int milliSeconds)
        where TBuilder : IInfoBarOptionsBuilder
    {
        builder.Configure(builder => builder.Options.MilliSecondsDelay = milliSeconds);
        return builder;
    }
}