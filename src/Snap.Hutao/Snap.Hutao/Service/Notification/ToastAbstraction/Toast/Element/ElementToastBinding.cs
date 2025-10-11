// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Common;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

[NotificationXmlElement("binding")]
internal sealed class ElementToastBinding
{
    public ElementToastBinding(ToastTemplateType template)
    {
        Template = template;
    }

    [NotificationXmlAttribute("template")]
    public ToastTemplateType Template { get; private set; }

    /// <summary>
    /// Set to true to allow Windows to append a query string to the image URI supplied in the tile notification. Use this attribute if your server hosts images and can handle query strings, either by retrieving an image variant based on the query strings or by ignoring the query string and returning the image as specified without the query string. This query string specifies scale, contrast setting, and language; for instance, a value of
    ///
    /// "www.website.com/images/hello.png"
    ///
    /// included in the notification becomes
    ///
    /// "www.website.com/images/hello.png?ms-scale=100&amp;ms-contrast=standard&amp;ms-lang=en-us"
    /// </summary>
    [NotificationXmlAttribute("addImageQuery")]
    public bool? AddImageQuery { get; set; }

    /// <summary>
    /// A default base URI that is combined with relative URIs in image source attributes.
    /// </summary>
    [NotificationXmlAttribute("baseUri")]
    public Uri? BaseUri { get; set; }

    /// <summary>
    /// The target locale of the XML payload, specified as a BCP-47 language tags such as "en-US" or "fr-FR". The locale specified here overrides that in visual, but can be overriden by that in text. If this value is a literal string, this attribute defaults to the user's UI language. If this value is a string reference, this attribute defaults to the locale chosen by Windows Runtime in resolving the string. See Remarks for when this value isn't specified.
    /// </summary>
    [NotificationXmlAttribute("lang")]
    public string? Language { get; set; }

    public IList<IElementToastBindingChild> Children { get; private set; } = [];
}