// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;

namespace Snap.Hutao.UI.Xaml.Behavior.Action;

[DependencyProperty<TeachingTip>("TeachingTip")]
internal sealed partial class ShowTeachingTipAction : DependencyObject, IAction
{
    public object? Execute(object? sender, object parameter)
    {
        if (sender is null)
        {
            return default;
        }

        if (TeachingTip is { } teachingTip)
        {
            teachingTip.IsOpen = true;
        }

        return default;
    }
}