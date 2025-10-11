// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Common;
using Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Adaptive.Element;

[NotificationXmlElement("image")]
internal sealed class ElementAdaptiveImage : IElementToastBindingChild, IElementAdaptiveSubgroupChild
{
    internal const AdaptiveImagePlacement DefaultPlacement = AdaptiveImagePlacement.Inline;
    internal const AdaptiveImageCrop DefaultCrop = AdaptiveImageCrop.Default;
    internal const AdaptiveImageAlign DefaultAlign = AdaptiveImageAlign.Default;

    [NotificationXmlAttribute("id")]
    public int? Id { get; set; }

    [NotificationXmlAttribute("src")]
    public required string Src { get; set; }

    [NotificationXmlAttribute("alt")]
    public string? Alt { get; set; }

    [NotificationXmlAttribute("addImageQuery")]
    public bool? AddImageQuery { get; set; }

    [NotificationXmlAttribute("placement", DefaultPlacement)]
    public AdaptiveImagePlacement Placement { get; set; }

    [NotificationXmlAttribute("hint-align", DefaultAlign)]
    public AdaptiveImageAlign Align { get; set; }

    [NotificationXmlAttribute("hint-crop", DefaultCrop)]
    public AdaptiveImageCrop Crop { get; set; }

    [NotificationXmlAttribute("hint-removeMargin")]
    public bool? RemoveMargin { get; set; }

    [NotificationXmlAttribute("hint-overlay")]
    public int? Overlay { get; set; }
}