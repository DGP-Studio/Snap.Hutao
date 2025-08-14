// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Media;

namespace Snap.Hutao.UI.Xaml.Control.Card;

[DependencyProperty<Brush>("ProgressForeground")]
[DependencyProperty<Brush>("TextForeground")]
[DependencyProperty<double>("Maximum", NotNull = true)]
[DependencyProperty<double>("Value", NotNull = true)]
[DependencyProperty<string>("Header")]
[DependencyProperty<string>("Description")]
internal sealed partial class CardProgressBar : Microsoft.UI.Xaml.Controls.Control
{
    public CardProgressBar()
    {
        DefaultStyleKey = typeof(CardProgressBar);
    }
}