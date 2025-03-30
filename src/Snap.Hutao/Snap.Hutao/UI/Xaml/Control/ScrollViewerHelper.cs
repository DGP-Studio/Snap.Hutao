// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.UI.Xaml.Control;

[SuppressMessage("", "SH001")]
[DependencyProperty("RightPanel", typeof(UIElement), IsAttached = true, AttachedType = typeof(ScrollViewer))]
[DependencyProperty("ScrollToTopAssociatedObject", typeof(object), default, nameof(OnScrollToTopAssociatedObjectChanged), IsAttached = true, AttachedType = typeof(ScrollViewer))]
public sealed partial class ScrollViewerHelper
{
    private static void OnScrollToTopAssociatedObjectChanged(DependencyObject dp, DependencyPropertyChangedEventArgs args)
    {
        if (dp is not ScrollViewer { IsLoaded: true } scrollViewer)
        {
            return;
        }

        if (args.OldValue != args.NewValue)
        {
            scrollViewer.ChangeView(null, 0, null);
        }
    }
}