// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.UI.Behaviors;
using Microsoft.UI.Xaml;

namespace Snap.Hutao.Control.Behavior;

/// <summary>
/// 在元素卸载完成后执行命令的行为
/// </summary>
internal class InvokeCommandOnUnloadedBehavior : BehaviorBase<UIElement>
{
    private static readonly DependencyProperty CommandProperty = Property<InvokeCommandOnUnloadedBehavior>.Depend<ICommand>(nameof(Command));

    /// <summary>
    /// 待执行的命令
    /// </summary>
    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <inheritdoc/>
    protected override void OnDetaching()
    {
        // 由于卸载顺序问题，必须重写此方法才能正确触发命令
        if (Command != null && Command.CanExecute(null))
        {
            Command.Execute(null);
        }

        base.OnDetaching();
    }
}