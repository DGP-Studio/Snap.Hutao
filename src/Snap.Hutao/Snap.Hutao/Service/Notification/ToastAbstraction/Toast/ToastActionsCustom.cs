// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
using Snap.Hutao.Service.Notification.ToastAbstraction.Common;
using Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast;

/// <summary>
/// Create your own custom actions, using controls like <see cref="ToastButton"/>, <see cref="ToastTextBox"/>, and <see cref="ToastSelectionBox"/>.
/// </summary>
internal sealed class ToastActionsCustom : IToastActions
{
    /// <summary>
    /// Initializes a new instance of custom actions, which can use controls like <see cref="ToastButton"/>, <see cref="ToastTextBox"/>, and <see cref="ToastSelectionBox"/>.
    /// </summary>
    public ToastActionsCustom() { }

    /// <summary>
    /// Inputs like <see cref="ToastTextBox"/> and <see cref="ToastSelectionBox"/> can be added to the toast. Only up to 5 inputs can be added; after that, an exception is thrown.
    /// </summary>
    public IList<IToastInput> Inputs { get; } = new LimitedList<IToastInput>(5);

    /// <summary>
    /// Buttons are displayed after all the inputs (or adjacent to inputs if used as quick reply buttons). Only up to 5 buttons can be added (or fewer if you are also including context menu items). After that, an exception is thrown. You can add <see cref="ToastButton"/>, <see cref="ToastButtonSnooze"/>, or <see cref="ToastButtonDismiss"/>
    /// </summary>
    public IList<IToastButton> Buttons { get; } = new LimitedList<IToastButton>(5);

    /// <summary>
    /// New in RS1: Custom context menu items, providing additional actions when the user right clicks the toast notification. You can only have up to 5 buttons and context menu items *combined*. Thus, if you have one context menu item, you can only have four buttons, etc.
    /// </summary>
    public IList<ToastContextMenuItem> ContextMenuItems { get; } = [];

    internal ElementToastActions ConvertToElement()
    {
        if (Buttons.Count + ContextMenuItems.Count > 5)
        {
            throw new InvalidOperationException("You have too many buttons/context menu items. You can only have up to 5 total.");
        }

        ElementToastActions el = new();

        foreach (IToastInput input in Inputs)
        {
            el.Children.Add(ConvertToInputElement(input));
        }

        foreach (IToastButton button in Buttons)
        {
            el.Children.Add(ConvertToActionElement(button));
        }

        foreach (ToastContextMenuItem item in ContextMenuItems)
        {
            el.Children.Add(item.ConvertToElement());
        }

        return el;
    }

    private static ElementToastAction ConvertToActionElement(IToastButton button)
    {
        return button switch
        {
            ToastButton toastButton => toastButton.ConvertToElement(),
            ToastButtonDismiss dismiss => dismiss.ConvertToElement(),
            ToastButtonSnooze snooze => snooze.ConvertToElement(),
            _ => throw new NotImplementedException("Unknown button child: " + button.GetType())
        };
    }

    private static ElementToastInput ConvertToInputElement(IToastInput input)
    {
        return input switch
        {
            ToastTextBox box => box.ConvertToElement(),
            ToastSelectionBox box => box.ConvertToElement(),
            _ => throw new NotImplementedException("Unknown input child: " + input.GetType())
        };
    }
}