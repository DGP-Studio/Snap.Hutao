// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast;

internal sealed class ToastContextMenuItem
{
    /// <summary>
    /// Initializes a toast context menu item with the required properties.
    /// </summary>
    /// <param name="content">The text to display on the menu item.</param>
    /// <param name="arguments">App-defined string of arguments that the app can later retrieve once it is activated when the user clicks the menu item.</param>
    public ToastContextMenuItem(string content, string arguments)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(arguments);

        Content = content;
        Arguments = arguments;
    }

    /// <summary>
    /// Required. The text to display on the menu item.
    /// </summary>
    public string Content { get; }

    /// <summary>
    /// Required. App-defined string of arguments that the app can later retrieve once it is activated when the user clicks the menu item.
    /// </summary>
    public string Arguments { get; }

    /// <summary>
    /// Controls what type of activation this menu item will use when clicked. Defaults to Foreground.
    /// </summary>
    public ToastActivationType ActivationType { get; set; } = ToastActivationType.Foreground;

    internal ElementToastAction ConvertToElement()
    {
        return new()
        {
            Content = Content,
            Arguments = Arguments,
            ActivationType = GetElementActivationType(),
            Placement = ElementToastActionPlacement.ContextMenu
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