// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast;

/// <summary>
/// Defines a toast image source and related properties.
/// </summary>
internal sealed class ToastImageSource
{
    /// <summary>
    /// Constructs an image source with all the required properties.
    /// </summary>
    /// <param name="src">The URI of the image source. Can be from your application package, application data, or the internet.</param>
    public ToastImageSource(string src)
    {
        ArgumentNullException.ThrowIfNull(src);
        Src = src;
    }

    /// <summary>
    /// The URI of the image source. Can be from your application package, application data, or the internet.
    /// </summary>
    public string Src { get; }

    /// <summary>
    /// A description of the image, for users of assistive technologies.
    /// </summary>
    public string? Alt { get; set; }

    /// <summary>
    /// Set to true to allow Windows to append a query string to the image URI supplied in the tile notification. Use this attribute if your server hosts images and can handle query strings, either by retrieving an image variant based on the query strings or by ignoring the query string and returning the image as specified without the query string. This query string specifies scale, contrast setting, and language.
    /// </summary>
    public bool AddImageQuery { get; set; } = ElementToastImage.DefaultAddImageQuery;

    internal ElementToastImage ConvertToElement()
    {
        ElementToastImage image = new();

        PopulateElement(image);

        return image;
    }

    internal void PopulateElement(ElementToastImage image)
    {
        image.Src = Src;
        image.Alt = Alt;
        image.AddImageQuery = AddImageQuery;
    }

    /// <summary>
    /// Returns the value of the Src property.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return Src;
    }
}