// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Common;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Adaptive.Element;

[NotificationXmlElement("subgroup")]
internal sealed class ElementAdaptiveSubgroup : IElement, IElementWithDescendants
{
    internal const AdaptiveSubgroupTextStacking DefaultTextStacking = AdaptiveSubgroupTextStacking.Default;

    [NotificationXmlAttribute("hint-textStacking", DefaultTextStacking)]
    public AdaptiveSubgroupTextStacking TextStacking { get; set; }

    [NotificationXmlAttribute("hint-weight")]
    public int? Weight
    {
        get;
        set
        {
            if (value is not null)
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value.Value, 1, nameof(Weight));
            }

            field = value;
        }
    }

    public IList<IElementAdaptiveSubgroupChild> Children { get; } = [];

    public IEnumerable<IElement> Descendants()
    {
        foreach (IElementAdaptiveSubgroupChild child in Children)
        {
            // Return each child (we know there's no further descendants)
            yield return child;
        }
    }
}