// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Behaviors;
using Microsoft.UI.Xaml;

namespace Snap.Hutao.Control.Behavior;

/// <summary>
/// 在元素加载完成后执行命令的行为
/// </summary>
[HighQuality]
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

    /// <inheritdoc/>
    protected override void OnAssociatedObjectLoaded()
    {
        TryExecuteCommand();
    }

    private void TryExecuteCommand()
    {
        if (executed)
        {
            return;
        }

        if (Command is not null && Command.CanExecute(CommandParameter))
        {
            Command.Execute(CommandParameter);
            executed = true;
        }
    }
}