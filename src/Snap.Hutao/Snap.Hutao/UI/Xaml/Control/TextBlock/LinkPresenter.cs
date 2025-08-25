// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.UI.Xaml.Control.TextBlock;

[DependencyProperty<string>("LinkName")]
[DependencyProperty<string>("LinkDescription")]
internal sealed partial class LinkPresenter : ContentControl
{
    public LinkPresenter()
    {
        DefaultStyleKey = typeof(LinkPresenter);
    }
}