// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast;

/// <summary>
/// An inline image displayed in your toast notification.
/// </summary>
public sealed class ToastImage
{
    /// <summary>
    /// Constructs an inline image that can be displayed on a toast notification.
    /// </summary>
    public ToastImage()
    {
    }

    /// <summary>
    /// Specify the image source.
    /// </summary>
    public ToastImageSource Source { get; set; }

    internal ElementToastImage ConvertToElement()
    {
        ElementToastImage el = new()
        {
            Placement = ToastImagePlacement.Inline
        };

        Source?.PopulateElement(el);

        return el;
    }
}