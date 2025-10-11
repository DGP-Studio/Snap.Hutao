// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Common;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Adaptive;

/// <summary>
/// TextStacking specifies the vertical alignment of content.
/// </summary>
internal enum AdaptiveSubgroupTextStacking
{
    /// <summary>
    /// Renderer automatically selects the default vertical alignment.
    /// </summary>
    Default,

    /// <summary>
    /// Vertical align to the top.
    /// </summary>
    [EnumString("top")]
    Top,

    /// <summary>
    /// Vertical align to the center.
    /// </summary>
    [EnumString("center")]
    Center,

    /// <summary>
    /// Vertical align to the bottom.
    /// </summary>
    [EnumString("bottom")]
    Bottom
}