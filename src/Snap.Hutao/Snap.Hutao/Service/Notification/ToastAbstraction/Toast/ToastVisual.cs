// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast;

/// <summary>
/// Defines the visual aspects of a toast notification.
/// </summary>
internal sealed class ToastVisual
{
    /// <summary>
    /// Initializes a new instance that defines the visual aspects of a toast notification.
    /// </summary>
    public ToastVisual()
    {
    }

    /// <summary>
    /// DEPRECATED: The version of the tile XML schema this particular payload was developed for. Windows 10 ignores this property.
    /// </summary>
    [Obsolete("This is not used by Windows 10. The Version property serves no purpose.")]
    public int? Version { get; set; }

    /// <summary>
    /// The target locale of the XML payload, specified as BCP-47 language tags such as "en-US" or "fr-FR". This locale is overridden by any locale specified in binding or text. If this value is a literal string, this attribute defaults to the user's UI language. If this value is a string reference, this attribute defaults to the locale chosen by Windows Runtime in resolving the string.
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// A default base URI that is combined with relative URIs in image source attributes.
    /// </summary>
    public Uri BaseUri { get; set; }

    /// <summary>
    /// Set to "true" to allow Windows to append a query string to the image URI supplied in the toast notification. Use this attribute if your server hosts images and can handle query strings, either by retrieving an image variant based on the query strings or by ignoring the query string and returning the image as specified without the query string. This query string specifies scale, contrast setting, and language.
    /// </summary>
    public bool? AddImageQuery { get; set; }

    /// <summary>
    /// The generic toast binding, which can be rendered on all devices. This binding is required and cannot be null.
    /// </summary>
    public ToastBindingGeneric BindingGeneric { get; set; }

    internal ElementToastVisual ConvertToElement()
    {
        ElementToastVisual visual = new()
        {
            Language = Language,
            BaseUri = BaseUri,
            AddImageQuery = AddImageQuery
        };

        ElementToastBinding binding = BindingGeneric.ConvertToElement();
        visual.Bindings.Add(binding);

        return visual;
    }
}