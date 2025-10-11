// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Adaptive;
using Snap.Hutao.Service.Notification.ToastAbstraction.Adaptive.Element;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast;

/// <summary>
/// The logo that is displayed on your toast notification.
/// </summary>
public sealed class ToastGenericAppLogo : IBaseImage
{
    /// <summary>
    /// Initializes a new instance of a toast app logo, which can override the logo displayed on your toast notification.
    /// </summary>
    public ToastGenericAppLogo()
    {
    }

    /// <summary>
    /// The URI of the image. Can be from your application package, application data, or the internet. Internet images must be less than 200 KB in size.
    /// </summary>
    public string Source { get; set => BaseImageHelper.SetSource(ref field, value); }

    /// <summary>
    /// A description of the image, for users of assistive technologies.
    /// </summary>
    public string? AlternateText { get; set; }

    /// <summary>
    /// Set to true to allow Windows to append a query string to the image URI supplied in the tile notification. Use this attribute if your server hosts images and can handle query strings, either by retrieving an image variant based on the query strings or by ignoring the query string and returning the image as specified without the query string. This query string specifies scale, contrast setting, and language.
    /// </summary>
    public bool? AddImageQuery { get; set; }

    /// <summary>
    /// Specify how you would like the image to be cropped.
    /// </summary>
    public ToastGenericAppLogoCrop HintCrop { get; set; }

    internal ElementAdaptiveImage ConvertToElement()
    {
        ElementAdaptiveImage el = BaseImageHelper.CreateBaseElement(this);

        el.Placement = AdaptiveImagePlacement.AppLogoOverride;
        el.Crop = GetAdaptiveImageCrop();

        return el;
    }

    private AdaptiveImageCrop GetAdaptiveImageCrop()
    {
        return HintCrop switch
        {
            ToastGenericAppLogoCrop.Circle => AdaptiveImageCrop.Circle,
            ToastGenericAppLogoCrop.None => AdaptiveImageCrop.None,
            _ => AdaptiveImageCrop.Default
        };
    }
}