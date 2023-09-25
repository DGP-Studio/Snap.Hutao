// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.View.Card.Primitive;

[DependencyProperty("Text", typeof(string))]
[DependencyProperty("ImageSource", typeof(object))]
[DependencyProperty("IsDotVisible", typeof(bool), false)]
[DependencyProperty("IconSquareLength", typeof(double), 32D)]
internal sealed partial class CardBlock : Grid
{
    public CardBlock()
    {
        InitializeComponent();
    }
}
