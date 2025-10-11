// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Common;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

[NotificationXmlElement("input")]
internal sealed class ElementToastInput : IElementToastActionsChild
{
    /// <summary>
    /// The id attribute is required and is for developers to retrieve user inputs once the app is activated (in the foreground or background).
    /// </summary>
    [NotificationXmlAttribute("id")]
    public string? Id { get; set; }

    [NotificationXmlAttribute("type")]
    public ToastInputType Type { get; set; }

    /// <summary>
    /// The title attribute is optional and is for developers to specify a title for the input for shells to render when there is affordance.
    /// </summary>
    [NotificationXmlAttribute("title")]
    public string? Title { get; set; }

    /// <summary>
    /// The placeholderContent attribute is optional and is the grey-out hint text for text input type. This attribute is ignored when the input type is not �text�.
    /// </summary>
    [NotificationXmlAttribute("placeHolderContent")]
    public string? PlaceholderContent { get; set; }

    /// <summary>
    /// The defaultInput attribute is optional and it allows developer to provide a default input value.
    /// </summary>
    [NotificationXmlAttribute("defaultInput")]
    public string? DefaultInput { get; set; }

    public IList<IElementToastInputChild> Children { get; private set; } = [];
}