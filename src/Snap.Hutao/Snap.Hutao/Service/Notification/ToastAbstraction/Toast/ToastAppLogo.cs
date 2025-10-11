// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast;

/// <summary>
/// The logo that is displayed on your toast notification.
/// </summary>
internal sealed class ToastAppLogo
{
    /// <summary>
    /// Initializes a new instance of a toast app logo, which can override the logo displayed on your toast notification.
    /// </summary>
    public ToastAppLogo()
    {
    }

    /// <summary>
    /// Specify the image source.
    /// </summary>
    public ToastImageSource? Source { get; set; }

    /// <summary>
    /// Specify how you would like the image to be cropped.
    /// </summary>
    public ToastImageCrop Crop { get; set; } = ElementToastImage.DefaultCrop;

    internal ElementToastImage ConvertToElement()
    {
        ElementToastImage el = new()
        {
            Placement = ToastImagePlacement.AppLogoOverride,
            Crop = Crop
        };

        Source?.PopulateElement(el);
        return el;
    }
}