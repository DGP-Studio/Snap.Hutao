// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.UI.Xaml.Control.Card;

[DependencyProperty("Left", typeof(UIElement), default!)]
[DependencyProperty("Right", typeof(UIElement), default!)]
internal sealed partial class HorizontalCard : Microsoft.UI.Xaml.Controls.Control
{
    public HorizontalCard()
    {
        DefaultStyleKey = typeof(HorizontalCard);
    }
}
