// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.UI.Xaml.Control.Card;

[DependencyProperty("Top", typeof(UIElement), default!)]
[DependencyProperty("Bottom", typeof(UIElement), default!)]
[DependencyProperty("BottomPadding", typeof(Thickness), default!)]
[DependencyProperty("HorizontalTopAlignment", typeof(HorizontalAlignment), HorizontalAlignment.Stretch)]
[DependencyProperty("VerticalTopAlignment", typeof(VerticalAlignment), VerticalAlignment.Stretch)]
[DependencyProperty("HorizontalBottomAlignment", typeof(HorizontalAlignment), HorizontalAlignment.Center)]
[DependencyProperty("VerticalBottomAlignment", typeof(VerticalAlignment), VerticalAlignment.Stretch)]
internal sealed partial class VerticalCard : Microsoft.UI.Xaml.Controls.Control
{
    public VerticalCard()
    {
        DefaultStyleKey = typeof(VerticalCard);
    }
}