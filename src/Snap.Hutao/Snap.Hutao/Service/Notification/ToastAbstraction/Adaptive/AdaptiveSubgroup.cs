// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
using Snap.Hutao.Service.Notification.ToastAbstraction.Adaptive.Element;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Adaptive;

/// <summary>
/// Subgroups are vertical columns that can contain text and images.
/// </summary>
internal sealed class AdaptiveSubgroup
{
    /// <summary>
    /// Initializes a new subgroup. Subgroups are vertical columns that can contain text and images.
    /// </summary>
    public AdaptiveSubgroup()
    {
    }

    /// <summary>
    /// <see cref="AdaptiveText"/> and <see cref="AdaptiveImage"/> are valid children of subgroups.
    /// </summary>
    public IList<IAdaptiveSubgroupChild> Children { get; } = [];

    /// <summary>
    /// Control the width of this subgroup column by specifying the weight, relative to the other subgroups.
    /// </summary>
    public int? HintWeight
    {
        get;
        set
        {
            if (value is not null)
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value.Value, 1, nameof(HintWeight));
            }

            field = value;
        }
    }

    /// <summary>
    /// Control the vertical alignment of this subgroup's content.
    /// </summary>
    public AdaptiveSubgroupTextStacking HintTextStacking { get; set; }

    internal ElementAdaptiveSubgroup ConvertToElement()
    {
        ElementAdaptiveSubgroup subgroup = new()
        {
            Weight = HintWeight,
            TextStacking = HintTextStacking
        };

        foreach (IAdaptiveSubgroupChild child in Children)
        {
            subgroup.Children.Add(ConvertToSubgroupChildElement(child));
        }

        return subgroup;
    }

    private static IElementAdaptiveSubgroupChild ConvertToSubgroupChildElement(IAdaptiveSubgroupChild child)
    {
        return child switch
        {
            AdaptiveText text => text.ConvertToElement(),
            AdaptiveImage image => image.ConvertToElement(),
            _ => throw new NotImplementedException($"Unknown child: {child.GetType()}")
        };
    }
}