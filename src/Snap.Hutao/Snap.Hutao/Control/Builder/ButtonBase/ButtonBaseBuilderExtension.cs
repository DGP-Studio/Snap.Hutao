// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction.Extension;

namespace Snap.Hutao.Control.Builder.ButtonBase;

internal static class ButtonBaseBuilderExtension
{
    public static TBuilder SetContent<TBuilder, TButton>(this TBuilder builder, object? content)
        where TBuilder : IButtonBaseBuilder<TButton>
        where TButton : Microsoft.UI.Xaml.Controls.Primitives.ButtonBase
    {
        builder.Configure(builder => builder.Button.Content = content);
        return builder;
    }

    public static TBuilder SetCommand<TBuilder, TButton>(this TBuilder builder, ICommand command)
        where TBuilder : IButtonBaseBuilder<TButton>
        where TButton : Microsoft.UI.Xaml.Controls.Primitives.ButtonBase
    {
        builder.Configure(builder => builder.Button.Command = command);
        return builder;
    }
}