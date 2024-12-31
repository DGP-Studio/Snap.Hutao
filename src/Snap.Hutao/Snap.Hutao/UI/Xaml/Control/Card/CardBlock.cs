// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.UI.Xaml.Control.Card;

[DependencyProperty("Text", typeof(string))]
[DependencyProperty("ImageSource", typeof(object))]
[DependencyProperty("DotVisibility", typeof(Visibility), Visibility.Collapsed)]
[DependencyProperty("IconSquareLength", typeof(double))]
internal sealed partial class CardBlock : Microsoft.UI.Xaml.Controls.Control
{
    public CardBlock()
    {
        DefaultStyleKey = typeof(CardBlock);
    }
}