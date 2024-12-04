// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.UI.Xaml;
using Snap.Hutao.ViewModel;

namespace Snap.Hutao.UI.Shell;

internal sealed partial class NotifyIconContextMenu : Flyout
{
    public NotifyIconContextMenu(IServiceProvider serviceProvider)
    {
        AllowFocusOnInteraction = false;
        InitializeComponent();
        Root.InitializeDataContext<NotifyIconViewModel>(serviceProvider);
    }
}