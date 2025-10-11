// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Common;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

[NotificationXmlElement("image")]
internal sealed class ElementToastImage : IElementToastBindingChild
{
    internal const ToastImagePlacement DefaultPlacement = ToastImagePlacement.Inline;
    internal const bool DefaultAddImageQuery = false;
    internal const ToastImageCrop DefaultCrop = ToastImageCrop.None;

    [NotificationXmlAttribute("src")]
    public string? Src { get; set; }

    [NotificationXmlAttribute("alt")]
    public string? Alt { get; set; }

    [NotificationXmlAttribute("addImageQuery", DefaultAddImageQuery)]
    public bool AddImageQuery { get; set; }

    [NotificationXmlAttribute("placement", DefaultPlacement)]
    public ToastImagePlacement Placement { get; set; }

    [NotificationXmlAttribute("hint-crop", DefaultCrop)]
    public ToastImageCrop Crop { get; set; }
}