// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.View.Card.Primitive;

[DependencyProperty("Left", typeof(UIElement), default!)]
[DependencyProperty("Right", typeof(UIElement), default!)]
internal sealed partial class HorizontalCard : UserControl
{
    public HorizontalCard()
    {
        InitializeComponent();
    }
}
