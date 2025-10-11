// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Adaptive.Element;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Adaptive;

/// <summary>
/// Groups semantically identify that the content in the group must either be displayed as a whole, or not displayed if it cannot fit. Groups also allow creating multiple columns.
/// </summary>
internal sealed class AdaptiveGroup : IAdaptiveChild
{
    /// <summary>
    /// Initializes a new group. Groups semantically identify that the content in the group must either be displayed as a whole, or not displayed if it cannot fit. Groups also allow creating multiple columns.
    /// </summary>
    public AdaptiveGroup()
    {
    }

    /// <summary>
    /// The only valid children of groups are <see cref="AdaptiveSubgroup"/>. Each subgroup is displayed as a separate vertical column. Note that you must include at least one subgroup in your group, otherwise an <see cref="InvalidOperationException"/> will be thrown when you try to retrieve the XML for the notification.
    /// </summary>
    public IList<AdaptiveSubgroup> Children { get; } = [];

    internal ElementAdaptiveGroup ConvertToElement()
    {
        if (Children.Count is 0)
        {
            throw new InvalidOperationException("Groups must have at least one child subgroup. The Children property had zero items in it.");
        }

        ElementAdaptiveGroup group = new();

        foreach (AdaptiveSubgroup subgroup in Children)
        {
            group.Children.Add(subgroup.ConvertToElement());
        }

        return group;
    }
}