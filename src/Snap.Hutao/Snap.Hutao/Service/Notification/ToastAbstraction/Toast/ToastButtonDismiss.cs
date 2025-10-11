// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast;

/// <summary>
/// A button that, when clicked, is interpreted as a "dismiss" by the system, and the toast is dismissed just like if the user swiped the toast away.
/// </summary>
public sealed class ToastButtonDismiss : IToastButton
{
    /// <summary>
    /// Initializes a system-handled dismiss button that displays localized "Dismiss" text on the button.
    /// </summary>
    public ToastButtonDismiss()
    {
    }

    /// <summary>
    /// Custom text displayed on the button that overrides the default localized "Dismiss" text.
    /// </summary>
    public string? CustomContent { get; private set; }

    /// <summary>
    /// Constructs a system-handled dismiss button that displays your text on the button.
    /// </summary>
    /// <param name="customContent">The text you want displayed on the button.</param>
    public ToastButtonDismiss(string customContent)
    {
        ArgumentNullException.ThrowIfNull(customContent);
        CustomContent = customContent;
    }

    internal ElementToastAction ConvertToElement()
    {
        return new()
        {
            Content = CustomContent ?? string.Empty, // If not using custom content, we need to provide empty string, otherwise toast doesn't get displayed
            Arguments = "dismiss",
            ActivationType = ElementToastActivationType.System
            // ImageUri is useless since Shell doesn't display it for system buttons
            // InputId is useless since dismiss button can't be placed to the right of text box (shell doesn't display it)
        };
    }
}