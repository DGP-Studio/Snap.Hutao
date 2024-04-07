// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Control.Builder.ButtonBase;

internal sealed class ButtonBaseBuilder<TButton> : IButtonBaseBuilder<TButton>
    where TButton : Microsoft.UI.Xaml.Controls.Primitives.ButtonBase, new()
{
    public TButton Button { get; } = new();
}