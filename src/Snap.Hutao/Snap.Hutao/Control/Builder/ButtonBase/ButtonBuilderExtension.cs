// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.Control.Builder.ButtonBase;

internal static class ButtonBuilderExtension
{
    public static ButtonBuilder SetContent(this ButtonBuilder builder, object? content)
    {
        return builder.SetContent<ButtonBuilder, Button>(content);
    }

    public static ButtonBuilder SetCommand(this ButtonBuilder builder, ICommand command)
    {
        return builder.SetCommand<ButtonBuilder, Button>(command);
    }
}