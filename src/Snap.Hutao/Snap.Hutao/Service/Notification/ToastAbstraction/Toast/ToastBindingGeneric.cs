// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Adaptive;
using Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast;

/// <summary>
/// Generic toast binding, where you provide text, images, and other visual elements for your toast notification.
/// </summary>
public sealed class ToastBindingGeneric
{
    /// <summary>
    /// Constructs a generic toast binding, where you provide text, images, and other visual elements for your toast notification.
    /// </summary>
    public ToastBindingGeneric()
    {
    }

    /// <summary>
    /// The contents of the body of the toast, which can include <see cref="AdaptiveText"/>, <see cref="AdaptiveImage"/>, and <see cref="AdaptiveGroup"/> (added in 14332). Also, <see cref="AdaptiveText"/> elements must come before any other elements. If an <see cref="AdaptiveText"/> element is placed after any other element, an exception will be thrown when you try to retrieve the toast XML content. And finally, certain <see cref="AdaptiveText"/> properties like Style aren't supported on the root children text elements, and only work inside an <see cref="AdaptiveGroup"/>.
    /// </summary>
    public IList<IAdaptiveChild> Children { get; private set; } = new List<IAdaptiveChild>();

    /// <summary>
    /// An optional override of the logo displayed on the toast notification.
    /// </summary>
    public ToastGenericAppLogo? AppLogoOverride { get; set; }

    /// <summary>
    /// New in RS1: An optional hero image (a visually impactful image displayed on the toast notification).
    /// </summary>
    public ToastGenericHeroImage? HeroImage { get; set; }

    /// <summary>
    /// New in RS1: An optional text element that is displayed as attribution text.
    /// </summary>
    public ToastGenericAttributionText? Attribution { get; set; }

    /// <summary>
    /// The target locale of the XML payload, specified as BCP-47 language tags such as "en-US" or "fr-FR". This locale is overridden by any locale specified in binding or text. If this value is a literal string, this attribute defaults to the user's UI language. If this value is a string reference, this attribute defaults to the locale chosen by Windows Runtime in resolving the string.
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// A default base URI that is combined with relative URIs in image source attributes.
    /// </summary>
    public Uri? BaseUri { get; set; }

    /// <summary>
    /// Set to "true" to allow Windows to append a query string to the image URI supplied in the toast notification. Use this attribute if your server hosts images and can handle query strings, either by retrieving an image variant based on the query strings or by ignoring the query string and returning the image as specified without the query string. This query string specifies scale, contrast setting, and language.
    /// </summary>
    public bool? AddImageQuery { get; set; }

    internal ElementToastBinding ConvertToElement()
    {
        ElementToastBinding binding = new(ToastTemplateType.ToastGeneric)
        {
            BaseUri = BaseUri,
            AddImageQuery = AddImageQuery,
            Language = Language
        };

        // Add children
        foreach (IAdaptiveChild child in Children)
        {
            binding.Children.Add(AdaptiveHelper.ConvertToElement(child));
        }

        // Add attribution
        if (Attribution is not null)
        {
            binding.Children.Add(Attribution.ConvertToElement());
        }

        // If there's hero, add it
        if (HeroImage is not null)
        {
            binding.Children.Add(HeroImage.ConvertToElement());
        }

        // If there's app logo, add it
        if (AppLogoOverride is not null)
        {
            binding.Children.Add(AppLogoOverride.ConvertToElement());
        }

        return binding;
    }
}