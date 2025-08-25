// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.UI.Xaml.Control.Card;

[DependencyProperty<string>("Text")]
[DependencyProperty<object>("ImageSource")]
[DependencyProperty<Visibility>("DotVisibility", DefaultValue = Visibility.Collapsed, NotNull = true)]
[DependencyProperty<double>("IconSquareLength", NotNull = true)]
internal sealed partial class CardBlock : Microsoft.UI.Xaml.Controls.Control
{
    public CardBlock()
    {
        DefaultStyleKey = typeof(CardBlock);
    }
}