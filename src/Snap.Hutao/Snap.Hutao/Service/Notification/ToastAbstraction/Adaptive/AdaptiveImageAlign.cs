// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Common;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Adaptive;

/// <summary>
/// Specifies the horizontal alignment for an image.
/// </summary>
internal enum AdaptiveImageAlign
{
    /// <summary>
    /// Default value, alignment behavior determined by renderer.
    /// </summary>
    Default,

    /// <summary>
    /// Image stretches to fill available width (and potentially available height too, depending on where the image is).
    /// </summary>
    [EnumString("stretch")]
    Stretch,

    /// <summary>
    /// Align the image to the left, displaying the image at its native resolution.
    /// </summary>
    [EnumString("left")]
    Left,

    /// <summary>
    /// Align the image in the center horizontally, displaying the image at its native resolution.
    /// </summary>
    [EnumString("center")]
    Center,

    /// <summary>
    /// Align the image to the right, displaying the image at its native resolution.
    /// </summary>
    [EnumString("right")]
    Right
}