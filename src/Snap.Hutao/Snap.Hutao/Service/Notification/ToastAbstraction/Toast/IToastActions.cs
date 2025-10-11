// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast;

/// <summary>
/// Actions to display on a toast notification. One of <see cref="ToastActionsCustom"/> or <see cref="ToastActionsSnoozeAndDismiss"/>.
/// </summary>
internal interface IToastActions
{
    /// <summary>
    /// New in RS1: Custom context menu items, providing additional actions when the user right-clicks the toast notification.
    /// </summary>
    IList<ToastContextMenuItem> ContextMenuItems { get; }
}