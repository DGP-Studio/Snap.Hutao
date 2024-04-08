// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Snap.Hutao.Control.Builder.ButtonBase;
using Snap.Hutao.Core.Abstraction.Extension;

namespace Snap.Hutao.Service.Notification;

internal static class InfoBarOptionsBuilderExtension
{
    public static TBuilder SetSeverity<TBuilder>(this TBuilder builder, InfoBarSeverity severity)
        where TBuilder : IInfoBarOptionsBuilder
    {
        builder.Configure(builder => builder.Options.Severity = severity);
        return builder;
    }

    public static TBuilder SetTitle<TBuilder>(this TBuilder builder, string? title)
        where TBuilder : IInfoBarOptionsBuilder
    {
        builder.Configure(builder => builder.Options.Title = title);
        return builder;
    }

    public static IInfoBarOptionsBuilder SetMessage<TBuilder>(this TBuilder builder, string? message)
        where TBuilder : IInfoBarOptionsBuilder
    {
        builder.Configure(builder => builder.Options.Message = message);
        return builder;
    }

    public static IInfoBarOptionsBuilder SetContent<TBuilder>(this TBuilder builder, object? content)
        where TBuilder : IInfoBarOptionsBuilder
    {
        builder.Configure(builder => builder.Options.Content = content);
        return builder;
    }

    public static IInfoBarOptionsBuilder SetActionButton<TBuilder, TButton>(this TBuilder builder, Action<ButtonBaseBuilder<TButton>> configureButton)
        where TBuilder : IInfoBarOptionsBuilder
        where TButton : ButtonBase, new()
    {
        ButtonBaseBuilder<TButton> buttonBaseBuilder = new ButtonBaseBuilder<TButton>().Configure(configureButton);
        builder.Configure(builder => builder.Options.ActionButton = buttonBaseBuilder.Button);
        return builder;
    }

    public static IInfoBarOptionsBuilder SetActionButton<TBuilder>(this TBuilder builder, Action<ButtonBuilder> configureButton)
        where TBuilder : IInfoBarOptionsBuilder
    {
        ButtonBuilder buttonBaseBuilder = new ButtonBuilder().Configure(configureButton);
        builder.Configure(builder => builder.Options.ActionButton = buttonBaseBuilder.Button);
        return builder;
    }

    public static IInfoBarOptionsBuilder SetDelay<TBuilder>(this TBuilder builder, int milliSeconds)
        where TBuilder : IInfoBarOptionsBuilder
    {
        builder.Configure(builder => builder.Options.MilliSecondsDelay = milliSeconds);
        return builder;
    }
}
