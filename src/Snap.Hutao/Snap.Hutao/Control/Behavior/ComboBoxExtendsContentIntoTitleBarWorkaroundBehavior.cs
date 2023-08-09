// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI.Behaviors;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.Control.Behavior;

/// <summary>
/// AppTitleBar Workaround
/// https://github.com/microsoft/microsoft-ui-xaml/issues/7756
/// </summary>
internal sealed class ComboBoxExtendsContentIntoTitleBarWorkaroundBehavior : BehaviorBase<ComboBox>
{
    private readonly IMessenger messenger;
    private readonly EventHandler<object> dropDownOpenedHandler;
    private readonly EventHandler<object> dropDownClosedHandler;

    /// <summary>
    /// AppTitleBar Workaround
    /// </summary>
    public ComboBoxExtendsContentIntoTitleBarWorkaroundBehavior()
    {
        messenger = Ioc.Default.GetRequiredService<IMessenger>();
        dropDownOpenedHandler = OnDropDownOpened;
        dropDownClosedHandler = OnDropDownClosed;
    }

    /// <inheritdoc/>
    protected override void OnAssociatedObjectLoaded()
    {
        AssociatedObject.DropDownOpened += dropDownOpenedHandler;
        AssociatedObject.DropDownClosed += dropDownClosedHandler;
    }

    /// <inheritdoc/>
    protected override void OnDetaching()
    {
        AssociatedObject.DropDownOpened -= dropDownOpenedHandler;
        AssociatedObject.DropDownClosed -= dropDownClosedHandler;

        base.OnDetaching();
    }

    private void OnDropDownOpened(object? sender, object e)
    {
        messenger.Send(Message.FlyoutStateChangedMessage.Open);
    }

    private void OnDropDownClosed(object? sender, object e)
    {
        messenger.Send(Message.FlyoutStateChangedMessage.Close);
    }
}