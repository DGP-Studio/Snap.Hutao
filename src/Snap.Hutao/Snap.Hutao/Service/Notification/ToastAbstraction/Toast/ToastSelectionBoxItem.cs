// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast;

/// <summary>
/// A selection box item (an item that the user can select from the drop down list).
/// </summary>
public sealed class ToastSelectionBoxItem
{
    /// <summary>
    /// Constructs a new toast SelectionBoxItem with the required elements.
    /// </summary>
    /// <param name="id">Developer-provided ID that the developer uses later to retrieve input when the toast is interacted with.</param>
    /// <param name="content">String that is displayed on the selection item. This is what the user sees.</param>
    public ToastSelectionBoxItem(string id, string content)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(content);

        Id = id;
        Content = content;
    }

    /// <summary>
    /// The ID property is required, and is used so that developers can retrieve user input once the app is activated.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// The Content property is required, and is a string that is displayed on the selection item.
    /// </summary>
    public string Content { get; }

    internal ElementToastSelection ConvertToElement()
    {
        return new()
        {
            Id = Id,
            Content = Content
        };
    }
}