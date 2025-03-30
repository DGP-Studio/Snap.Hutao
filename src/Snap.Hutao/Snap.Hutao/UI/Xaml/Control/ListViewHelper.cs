// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.UI.Xaml.Control;

[SuppressMessage("", "SH001")]
[DependencyProperty("ScrollToTopAssociatedObject", typeof(object), default, nameof(OnScrollToTopAssociatedObjectChanged), IsAttached = true, AttachedType = typeof(ListView))]
public sealed partial class ListViewHelper
{
    private static void OnScrollToTopAssociatedObjectChanged(DependencyObject dp, DependencyPropertyChangedEventArgs args)
    {
        if (dp.FindDescendant<ScrollViewer>() is not { IsLoaded: true } scrollViewer)
        {
            return;
        }

        if (dp.FindDescendant<ItemsStackPanel>() is not { IsLoaded: true } itemsStackPanel)
        {
            return;
        }

        itemsStackPanel.LayoutUpdated += ItemsStackPanelOnLayoutUpdated;

        void ItemsStackPanelOnLayoutUpdated(object? sender, object e)
        {
            if (args.OldValue != args.NewValue)
            {
                scrollViewer.ChangeView(null, 0, null);
            }

            itemsStackPanel.LayoutUpdated -= ItemsStackPanelOnLayoutUpdated;
        }
    }
}