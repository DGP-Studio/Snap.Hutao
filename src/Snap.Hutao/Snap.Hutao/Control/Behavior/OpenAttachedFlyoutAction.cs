// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.Xaml.Interactivity;

namespace Snap.Hutao.Control.Behavior;

/// <summary>
/// 打开附着的浮出控件操作
/// </summary>
internal class OpenAttachedFlyoutAction : DependencyObject, IAction
{
    /// <inheritdoc/>
    public object Execute(object sender, object parameter)
    {
        FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        return null!;
    }
}