// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
using Snap.Hutao.Service.Notification.ToastAbstraction.Common;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

[NotificationXmlElement("action")]
internal sealed class ElementToastAction : IElementToastActionsChild
{
    internal const ElementToastActivationType DefaultActivationType = ElementToastActivationType.Foreground;
    internal const ElementToastActionPlacement DefaultPlacement = ElementToastActionPlacement.Inline;

    /// <summary>
    /// The text to be displayed on the button.
    /// </summary>
    [NotificationXmlAttribute("content")]
    public string? Content { get; set; }

    /// <summary>
    /// The arguments attribute describes the app-defined data that the app can later retrieve once it is activated from user taking this action.
    /// </summary>
    [NotificationXmlAttribute("arguments")]
    public string? Arguments { get; set; }

    [NotificationXmlAttribute("activationType", DefaultActivationType)]
    public ElementToastActivationType ActivationType { get; set; }

    /// <summary>
    /// imageUri is optional and is used to provide an image icon for this action to display inside the button alone with the text content.
    /// </summary>
    [NotificationXmlAttribute("imageUri")]
    public string? ImageUri { get; set; }

    /// <summary>
    /// This is specifically used for the quick reply scenario.
    /// </summary>
    [NotificationXmlAttribute("hint-inputId")]
    public string? InputId { get; set; }

    [NotificationXmlAttribute("placement", DefaultPlacement)]
    public ElementToastActionPlacement Placement { get; set; }
}