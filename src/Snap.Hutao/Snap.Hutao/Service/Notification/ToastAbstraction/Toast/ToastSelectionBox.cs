// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Common;
using Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast;

/// <summary>
/// A selection box control, which lets users pick from a dropdown list of options.
/// </summary>
internal sealed class ToastSelectionBox : IToastInput
{
    /// <summary>
    /// Initializes a new toast SelectionBox input control with the required elements.
    /// </summary>
    /// <param name="id">Developer-provided ID that the developer uses later to retrieve input when the toast is interacted with.</param>
    public ToastSelectionBox(string id)
    {
        ArgumentNullException.ThrowIfNull(id);
        Id = id;
    }

    /// <summary>
    /// The ID property is required, and is used so that developers can retrieve user input once the app is activated.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Title text to display above the SelectionBox.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// This controls which item is selected by default, and refers to the Id property of <see cref="ToastSelectionBoxItem"/>. If you do not provide this, the default selection will be empty (user sees nothing).
    /// </summary>
    public string? DefaultSelectionBoxItemId { get; set; }

    /// <summary>
    /// The selection items that the user can pick from in this SelectionBox. Only 5 items can be added.
    /// </summary>
    public IList<ToastSelectionBoxItem> Items { get; } = new LimitedList<ToastSelectionBoxItem>(5);

    internal ElementToastInput ConvertToElement()
    {
        ElementToastInput input = new()
        {
            Type = ToastInputType.Selection,
            Id = Id,
            DefaultInput = DefaultSelectionBoxItemId,
            Title = Title
        };

        foreach (ToastSelectionBoxItem item in Items)
        {
            input.Children.Add(item.ConvertToElement());
        }

        return input;
    }
}