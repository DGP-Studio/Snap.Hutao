// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast;

/// <summary>
/// Automatically constructs a selection box for snooze intervals, and snooze/dismiss buttons, all automatically localized, and snoozing logic is automatically handled by the system.
/// </summary>
internal sealed class ToastActionsSnoozeAndDismiss : IToastActions
{
    /// <summary>
    /// Automatically constructs a selection box for snooze intervals, and snooze/dismiss buttons, all automatically localized, and snoozing logic is automatically handled by the system.
    /// </summary>
    public ToastActionsSnoozeAndDismiss() { }

    /// <summary>
    /// New in RS1: Custom context menu items, providing additional actions when the user right clicks the toast notification. You can only have up to 5 items.
    /// </summary>
    public IList<ToastContextMenuItem> ContextMenuItems { get; private set; } = new List<ToastContextMenuItem>();

    internal ElementToastActions ConvertToElement()
    {
        if (ContextMenuItems.Count > 5)
        {
            throw new InvalidOperationException("You have too many context menu items. You can only have up to 5.");
        }

        ElementToastActions el = new()
        {
            SystemCommands = ToastSystemCommand.SnoozeAndDismiss
        };

        foreach (ToastContextMenuItem item in ContextMenuItems)
        {
            el.Children.Add(item.ConvertToElement());
        }

        return el;
    }
}