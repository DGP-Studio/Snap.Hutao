// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Common;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Adaptive;

/// <summary>
/// Specify the desired cropping of the image.
/// </summary>
internal enum AdaptiveImageCrop
{
    /// <summary>
    /// Default value, cropping behavior determined by renderer.
    /// </summary>
    Default,

    /// <summary>
    /// Image is not cropped.
    /// </summary>
    [EnumString("none")]
    None,

    /// <summary>
    /// Image is cropped to a circle shape.
    /// </summary>
    [EnumString("circle")]
    Circle
}