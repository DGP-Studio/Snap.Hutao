// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI.UI.Behaviors;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.Control.Behavior;

/// <summary>
/// AppTitleBar Workaround
/// https://github.com/microsoft/microsoft-ui-xaml/issues/7756
/// </summary>
internal class ComboBoxExtendsContentIntoTitleBarWorkaroundBehavior : BehaviorBase<ComboBox>
{
    private readonly IMessenger messenger;

    /// <summary>
    /// AppTitleBar Workaround
    /// </summary>
    public ComboBoxExtendsContentIntoTitleBarWorkaroundBehavior()
    {
        messenger = Ioc.Default.GetRequiredService<IMessenger>();
    }

    /// <inheritdoc/>
    protected override void OnAssociatedObjectLoaded()
    {
        AssociatedObject.DropDownOpened += OnDropDownOpened;
        AssociatedObject.DropDownClosed += OnDropDownClosed;
    }

    private void OnDropDownOpened(object? sender, object e)
    {
        messenger.Send(new Message.FlyoutOpenCloseMessage(true));
    }

    private void OnDropDownClosed(object? sender, object e)
    {
        messenger.Send(new Message.FlyoutOpenCloseMessage(false));
    }
}