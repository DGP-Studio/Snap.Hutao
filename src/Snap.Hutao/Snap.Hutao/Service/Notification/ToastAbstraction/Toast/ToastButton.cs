// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast;

/// <summary>
/// A button that the user can click on a toast notification.
/// </summary>
internal sealed class ToastButton : IToastButton
{
    /// <summary>
    /// Initializes a toast button with the required properties.
    /// </summary>
    /// <param name="content">The text to display on the button.</param>
    /// <param name="arguments">App-defined string of arguments that the app can later retrieve once it is activated when the user clicks the button.</param>
    public ToastButton(string content, string arguments)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(arguments);

        Content = content;
        Arguments = arguments;
    }

    /// <summary>
    /// Required. The text to display on the button.
    /// </summary>
    public string Content { get; }

    /// <summary>
    /// Required. App-defined string of arguments that the app can later retrieve once it is activated when the user clicks the button.
    /// </summary>
    public string Arguments { get; }

    /// <summary>
    /// Controls what type of activation this button will use when clicked. Defaults to Foreground.
    /// </summary>
    public ToastActivationType ActivationType { get; set; } = ToastActivationType.Foreground;

    /// <summary>
    /// An optional image icon for the button to display (required for buttons adjacent to inputs like quick reply).
    /// </summary>
    public string? ImageUri { get; set; }

    /// <summary>
    /// Specify the ID of an existing <see cref="ToastTextBox"/> in order to have this button display to the right of the input, achieving a quick reply scenario.
    /// </summary>
    public string? TextBoxId { get; set; }

    internal ElementToastAction ConvertToElement()
    {
        return new()
        {
            Content = Content,
            Arguments = Arguments,
            ActivationType = GetElementActivationType(),
            ImageUri = ImageUri,
            InputId = TextBoxId
        };
    }

    private ElementToastActivationType GetElementActivationType()
    {
        return ActivationType switch
        {
            ToastActivationType.Foreground => ElementToastActivationType.Foreground,
            ToastActivationType.Background => ElementToastActivationType.Background,
            ToastActivationType.Protocol => ElementToastActivationType.Protocol,
            _ => throw new NotImplementedException()
        };
    }
}