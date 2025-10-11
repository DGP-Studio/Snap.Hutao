// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Common;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

[NotificationXmlElement("selection")]
internal sealed class ElementToastSelection : IElementToastInputChild
{
    /// <summary>
    /// The id attribute is required, and it is for apps to retrieve back the user selected input after the app is activated.
    /// </summary>
    [NotificationXmlAttribute("id")]
    public string? Id { get; set; }

    /// <summary>
    /// The text to display for this selection element.
    /// </summary>
    [NotificationXmlAttribute("content")]
    public string? Content { get; set; }
}