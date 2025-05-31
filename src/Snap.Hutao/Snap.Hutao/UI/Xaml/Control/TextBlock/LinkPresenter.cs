// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.UI.Xaml.Control.TextBlock;

[DependencyProperty("LinkName", typeof(string), default(string))]
[DependencyProperty("LinkDescription", typeof(string), default(string))]
internal sealed partial class LinkPresenter : ContentControl
{
    public LinkPresenter()
    {
        DefaultStyleKey = typeof(LinkPresenter);
    }
}