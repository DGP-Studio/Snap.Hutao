// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Adaptive.Element;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Adaptive;

/// <summary>
/// An inline image.
/// </summary>
internal sealed class AdaptiveImage : IBaseImage, IAdaptiveChild, IAdaptiveSubgroupChild
{
    /// <summary>
    /// Initializes a new inline image.
    /// </summary>
    public AdaptiveImage()
    {
    }

    /// <summary>
    /// Control the desired cropping of the image.
    /// </summary>
    public AdaptiveImageCrop HintCrop { get; set; }

    /// <summary>
    /// By default, images have an 8px margin around them. You can remove this margin by setting this property to true.
    /// </summary>
    public bool? HintRemoveMargin { get; set; }

    /// <summary>
    /// The horizontal alignment of the image.
    /// </summary>
    public AdaptiveImageAlign HintAlign { get; set; }

    /// <summary>
    /// Required. The URI of the image. Can be from your application package, application data, or the internet. Internet images must be less than 200 KB in size.
    /// </summary>
    public required string Source { get; set => BaseImageHelper.SetSource(ref field, value); }

    /// <summary>
    /// A description of the image, for users of assistive technologies.
    /// </summary>
    public string? AlternateText { get; set; }

    /// <summary>
    /// Set to true to allow Windows to append a query string to the image URI supplied in the tile notification. Use this attribute if your server hosts images and can handle query strings, either by retrieving an image variant based on the query strings or by ignoring the query string and returning the image as specified without the query string. This query string specifies scale, contrast setting, and language.
    /// </summary>
    public bool? AddImageQuery { get; set;}

    internal ElementAdaptiveImage ConvertToElement()
    {
        ElementAdaptiveImage image = BaseImageHelper.CreateBaseElement(this);

        image.Crop = HintCrop;
        image.RemoveMargin = HintRemoveMargin;
        image.Align = HintAlign;
        image.Placement = AdaptiveImagePlacement.Inline;

        return image;
    }

    /// <summary>
    /// Returns the image's source string.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return Source;
    }
}