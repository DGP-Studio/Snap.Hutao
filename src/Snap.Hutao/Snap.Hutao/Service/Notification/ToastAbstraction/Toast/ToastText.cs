// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast;

/// <summary>
/// Defines a text element to be displayed on the toast notification.
/// </summary>
public sealed class ToastText
{
    /// <summary>
    /// Constructs a new text element that can be displayed on a toast notification.
    /// </summary>
    public ToastText()
    {
    }

    /// <summary>
    /// The text to display.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// The target locale of the XML payload, specified as a BCP-47 language tags such as "en-US" or "fr-FR". The locale specified here overrides any other specified locale, such as that in binding or visual. If this value is a literal string, this attribute defaults to the user's UI language. If this value is a string reference, this attribute defaults to the locale chosen by Windows Runtime in resolving the string.
    /// </summary>
    public string Language { get; set; }

    internal ElementToastText ConvertToElement()
    {
        return new()
        {
            Text = Text,
            Lang = Language
        };
    }
}