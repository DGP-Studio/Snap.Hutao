// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Behaviors;
using Microsoft.UI.Xaml;
using Snap.Hutao.UI.Input;

namespace Snap.Hutao.UI.Xaml.Behavior;

[DependencyProperty("Command", typeof(ICommand))]
[DependencyProperty("CommandParameter", typeof(object))]
internal sealed partial class InvokeCommandOnLoadedBehavior : BehaviorBase<UIElement>
{
    private bool executed;

    protected override void OnAttached()
    {
        base.OnAttached();

        // FrameworkElement in a ItemsRepeater gets attached twice
        if (AssociatedObject is FrameworkElement { IsLoaded: true })
        {
            TryExecuteCommand();
        }
    }

    protected override void OnAssociatedObjectLoaded()
    {
        TryExecuteCommand();
    }

    private void TryExecuteCommand()
    {
        if (AssociatedObject is null)
        {
            return;
        }

        if (executed)
        {
            return;
        }

        executed = Command.TryExecute(CommandParameter);
    }
}