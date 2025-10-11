// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Common;
using Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Adaptive.Element;

[NotificationXmlElement("group")]
internal sealed class ElementAdaptiveGroup : IElementToastBindingChild, IElementWithDescendants
{
    public IList<ElementAdaptiveSubgroup> Children { get; } = [];

    public IEnumerable<IElement> Descendants()
    {
        foreach (ElementAdaptiveSubgroup subgroup in Children)
        {
            // Return the subgroup
            yield return subgroup;

            // And also return its descendants
            foreach (IElement descendant in subgroup.Descendants())
            {
                yield return descendant;
            }
        }
    }
}