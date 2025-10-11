// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast;

/// <summary>
/// A text box control on the toast that a user can type text into.
/// </summary>
internal sealed class ToastTextBox : IToastInput
{
    /// <summary>
    /// Initializes a new toast TextBox input control with the required elements.
    /// </summary>
    /// <param name="id">Developer-provided ID that the developer uses later to retrieve input when the toast is interacted with.</param>
    public ToastTextBox(string id)
    {
        ArgumentNullException.ThrowIfNull(id);

        Id = id;
    }

    /// <summary>
    /// The ID property is required, and is used so that developers can retrieve user input once the app is activated.
    /// </summary>
    public string Id { get; private set; }

    /// <summary>
    /// Title text to display above the text box.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Placeholder text to be displayed on the text box when the user hasn't typed any text yet.
    /// </summary>
    public string PlaceholderContent { get; set; }

    /// <summary>
    /// The initial text to place in the text box. Leave this null for a blank text box.
    /// </summary>
    public string DefaultInput { get; set; }

    internal ElementToastInput ConvertToElement()
    {
        return new()
        {
            Id = Id,
            Type = ToastInputType.Text,
            DefaultInput = DefaultInput,
            PlaceholderContent = PlaceholderContent,
            Title = Title
        };
    }
}