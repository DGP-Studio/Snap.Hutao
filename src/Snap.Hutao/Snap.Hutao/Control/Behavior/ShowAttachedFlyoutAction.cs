// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.Xaml.Interactivity;
using Snap.Hutao.Core;

namespace Snap.Hutao.Control.Behavior;

/// <summary>
/// 打开附着的浮出控件操作
/// </summary>
[HighQuality]
internal sealed class ShowAttachedFlyoutAction : DependencyObject, IAction
{
    /// <inheritdoc/>
    public object? Execute(object sender, object parameter)
    {
        if (sender is null)
        {
            return default;
        }

        FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        return default;
    }
}