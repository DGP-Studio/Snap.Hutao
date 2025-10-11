// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Common;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

[NotificationXmlElement("text")]
internal sealed class ElementToastText : IElementToastBindingChild
{
    internal const ToastTextPlacement DefaultPlacement = ToastTextPlacement.Inline;

    [NotificationXmlContent]
    public string? Text { get; set; }

    [NotificationXmlAttribute("lang")]
    public string? Lang { get; set; }

    [NotificationXmlAttribute("placement", DefaultPlacement)]
    public ToastTextPlacement Placement { get; set; } = DefaultPlacement;
}