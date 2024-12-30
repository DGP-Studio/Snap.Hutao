// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.Xaml.Interactivity;

namespace Snap.Hutao.UI.Xaml.Behavior.Action;

internal sealed class ShowAttachedFlyoutAction : DependencyObject, IAction
{
    public object? Execute(object? sender, object parameter)
    {
        if (sender is null)
        {
            return default;
        }

        FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        return default;
    }
}