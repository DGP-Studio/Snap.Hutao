// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.UI.Xaml.Control.Card;

[DependencyProperty<UIElement>("Top")]
[DependencyProperty<UIElement>("Bottom")]
[DependencyProperty<Thickness>("BottomPadding", NotNull = true)]
[DependencyProperty<HorizontalAlignment>("HorizontalTopAlignment", DefaultValue = HorizontalAlignment.Stretch, NotNull = true)]
[DependencyProperty<VerticalAlignment>("VerticalTopAlignment", DefaultValue = VerticalAlignment.Stretch, NotNull = true)]
[DependencyProperty<HorizontalAlignment>("HorizontalBottomAlignment", DefaultValue = HorizontalAlignment.Center, NotNull = true)]
[DependencyProperty<VerticalAlignment>("VerticalBottomAlignment", DefaultValue = VerticalAlignment.Stretch, NotNull = true)]
internal sealed partial class VerticalCard : Microsoft.UI.Xaml.Controls.Control
{
    public VerticalCard()
    {
        DefaultStyleKey = typeof(VerticalCard);
    }
}