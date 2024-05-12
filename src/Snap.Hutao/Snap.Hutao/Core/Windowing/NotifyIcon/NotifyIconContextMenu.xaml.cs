// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.ViewModel;

namespace Snap.Hutao.Core.Windowing.NotifyIcon;

internal sealed partial class NotifyIconContextMenu : Flyout
{
    public NotifyIconContextMenu()
    {
        AllowFocusOnInteraction = false;
        InitializeComponent();
        Root.DataContext = Ioc.Default.GetRequiredService<NotifyIconViewModel>();
    }
}