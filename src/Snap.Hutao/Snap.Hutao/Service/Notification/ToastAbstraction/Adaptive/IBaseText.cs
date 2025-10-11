// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Adaptive;

/// <summary>
/// Defines the basic properties of a text element.
/// </summary>
internal interface IBaseText
{
    /// <summary>
    /// The text to display.
    /// </summary>
    string? Text { get; set; }

    /// <summary>
    /// The target locale of the XML payload, specified as a BCP-47 language tags such as "en-US" or "fr-FR". The locale specified here overrides any other specified locale, such as that in binding or visual. If this value is a literal string, this attribute defaults to the user's UI language. If this value is a string reference, this attribute defaults to the locale chosen by Windows Runtime in resolving the string.
    /// </summary>
    string? Language { get; set; }
}