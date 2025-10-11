// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Common;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

[NotificationXmlElement("visual")]
internal sealed class ElementToastVisual
{
    internal const bool DefaultAddImageQuery = false;

    [NotificationXmlAttribute("addImageQuery")]
    public bool? AddImageQuery { get; set; }

    [NotificationXmlAttribute("baseUri")]
    public Uri? BaseUri { get; set; }

    [NotificationXmlAttribute("lang")]
    public string? Language { get; set; }

    [NotificationXmlAttribute("version")]
    public int? Version { get; set; }

    public IList<ElementToastBinding> Bindings { get; private set; } = [];
}