// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.UI.Behaviors;
using Microsoft.UI.Xaml;
using Snap.Hutao.Core;

namespace Snap.Hutao.Control.Behavior;

/// <summary>
/// 在元素加载完成后执行命令的行为
/// </summary>
internal class InvokeCommandOnLoadedBehavior : BehaviorBase<UIElement>
{
    private static readonly DependencyProperty CommandProperty = Property<InvokeCommandOnLoadedBehavior>.Depend<ICommand>(nameof(Command));
    private static readonly DependencyProperty CommandParameterProperty = Property<InvokeCommandOnLoadedBehavior>.Depend<object>(nameof(CommandParameter));

    /// <summary>
    /// 待执行的命令
    /// </summary>
    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// 命令参数
    /// </summary>
    [MaybeNull]
    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    /// <inheritdoc/>
    protected override void OnAssociatedObjectLoaded()
    {
        if (Command != null && Command.CanExecute(CommandParameter))
        {
            Command?.Execute(CommandParameter);
        }

        base.OnAssociatedObjectLoaded();
    }
}