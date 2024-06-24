// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.UI.Xaml.Control.Card;

[DependencyProperty("Text", typeof(string))]
[DependencyProperty("ImageSource", typeof(object))]
[DependencyProperty("IsDotVisible", typeof(bool), false)]
[DependencyProperty("IconSquareLength", typeof(double))]
internal sealed partial class CardBlock : Microsoft.UI.Xaml.Controls.Control
{
    public CardBlock()
    {
        DefaultStyleKey = typeof(CardBlock);
    }
}