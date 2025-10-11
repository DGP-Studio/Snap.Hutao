// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
using Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast;

/// <summary>
/// A system-handled snooze button that automatically handles snoozing of a toast notification.
/// </summary>
public sealed class ToastButtonSnooze : IToastButton
{
    /// <summary>
    /// Initializes a system-handled snooze button that displays localized "Snooze" text on the button and automatically handles snoozing.
    /// </summary>
    public ToastButtonSnooze()
    {
    }

    /// <summary>
    /// Initializes a system-handled snooze button that displays your text on the button and automatically handles snoozing.
    /// </summary>
    /// <param name="customContent">The text you want displayed on the button.</param>
    public ToastButtonSnooze(string customContent)
    {
        ArgumentNullException.ThrowIfNull(customContent);
        CustomContent = customContent;
    }

    /// <summary>
    /// Custom text displayed on the button that overrides the default localized "Snooze" text.
    /// </summary>
    public string? CustomContent { get; }

    /// <summary>
    /// Optionally specify the ID of an existing <see cref="ToastSelectionBox"/> in order to allow the user to pick a custom snooze time. The ID's of the <see cref="ToastSelectionBoxItem"/>s inside the selection box must represent the snooze interval in minutes. For example, if the user selects an item that has an ID of "120", then the notification will be snoozed for 2 hours. When the user clicks this button, if you specified a SelectionBoxId, the system will parse the ID of the selected item and snooze by that amount of minutes. If you didn't specify a SelectionBoxId, the system will snooze by the default system snooze time.
    /// </summary>
    public string? SelectionBoxId { get; set; }

    internal ElementToastAction ConvertToElement()
    {
        return new()
        {
            Content = CustomContent ?? string.Empty, // If not using custom content, we need to provide empty string, otherwise toast doesn't get displayed
            Arguments = "snooze",
            ActivationType = ElementToastActivationType.System,
            InputId = SelectionBoxId
            // ImageUri is useless since Shell doesn't display it for system buttons
        };
    }
}