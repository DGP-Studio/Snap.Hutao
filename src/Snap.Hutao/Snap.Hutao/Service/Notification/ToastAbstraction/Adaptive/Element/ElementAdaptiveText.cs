// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Common;
using Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Adaptive.Element;

[NotificationXmlElement("text")]
internal sealed class ElementAdaptiveText : IElementAdaptiveSubgroupChild, IElementToastBindingChild
{
    internal const AdaptiveTextStyle DefaultStyle = AdaptiveTextStyle.Default;
    internal const AdaptiveTextAlign DefaultAlign = AdaptiveTextAlign.Default;
    internal const AdaptiveTextPlacement DefaultPlacement = AdaptiveTextPlacement.Inline;

    [NotificationXmlContent]
    public string? Text { get; set; }

    [NotificationXmlAttribute("id")]
    public int? Id { get; set; }

    [NotificationXmlAttribute("lang")]
    public string? Lang { get; set; }

    [NotificationXmlAttribute("hint-align", DefaultAlign)]
    public AdaptiveTextAlign Align { get; set; }

    [NotificationXmlAttribute("hint-maxLines")]
    public int? MaxLines
    {
        get;
        set
        {
            if (value is not null)
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value.Value, 1, nameof(MaxLines));
            }

            field = value;
        }
    }

    [NotificationXmlAttribute("hint-minLines")]
    public int? MinLines
    {
        get;
        set
        {
            if (value is not null)
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value.Value, 1, nameof(MinLines));
            }

            field = value;
        }
    }

    [NotificationXmlAttribute("hint-style", DefaultStyle)]
    public AdaptiveTextStyle Style { get; set; }

    [NotificationXmlAttribute("hint-wrap")]
    public bool? Wrap { get; set; }

    [NotificationXmlAttribute("placement", DefaultPlacement)]
    public AdaptiveTextPlacement Placement { get; set; }
}