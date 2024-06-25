// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.UI.Xaml.Control.Card;

[DependencyProperty("Top", typeof(UIElement), default!)]
[DependencyProperty("Bottom", typeof(UIElement), default!)]
[DependencyProperty("BottomPadding", typeof(Thickness), default!)]
internal sealed partial class VerticalCard : Microsoft.UI.Xaml.Controls.Control
{
    public VerticalCard()
    {
        DefaultStyleKey = typeof(VerticalCard);
    }
}