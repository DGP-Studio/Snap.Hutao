// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Common;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast;

/// <summary>
/// Specify the desired cropping of the image.
/// </summary>
public enum ToastAppLogoCrop
{
    /// <summary>
    /// Default value. Image is not cropped.
    /// </summary>
    None,

    /// <summary>
    /// Image is cropped to a circle shape.
    /// </summary>
    [EnumString("circle")]
    Circle
}