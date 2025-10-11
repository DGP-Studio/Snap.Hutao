// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Common;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

[NotificationXmlElement("toast")]
internal sealed class ElementToast : BaseElement
{
    internal const ToastScenario DefaultScenario = ToastScenario.Default;
    internal const ToastActivationType DefaultActivationType = ToastActivationType.Foreground;
    internal const ToastDuration DefaultDuration = ToastDuration.Short;

    [NotificationXmlAttribute("activationType", DefaultActivationType)]
    public ToastActivationType ActivationType { get; set; }

    [NotificationXmlAttribute("duration", DefaultDuration)]
    public ToastDuration Duration { get; set; }

    [NotificationXmlAttribute("launch")]
    public string? Launch { get; set; }

    [NotificationXmlAttribute("scenario", DefaultScenario)]
    public ToastScenario Scenario { get; set; }

    public ElementToastVisual? Visual { get; set; }

    public ElementToastAudio? Audio { get; set; }

    public ElementToastActions? Actions { get; set; }
}