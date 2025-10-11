// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Adaptive.Element;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Adaptive;

/// <summary>
/// An adaptive text element.
/// </summary>
internal sealed class AdaptiveText : IAdaptiveSubgroupChild, IAdaptiveChild, IBaseText
{
    /// <summary>
    /// Initializes a new Adaptive text element.
    /// </summary>
    public AdaptiveText()
    {
    }

    /// <summary>
    /// The text to display.
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// The target locale of the XML payload, specified as a BCP-47 language tags such as "en-US" or "fr-FR". The locale specified here overrides any other specified locale, such as that in binding or visual. If this value is a literal string, this attribute defaults to the user's UI language. If this value is a string reference, this attribute defaults to the locale chosen by Windows Runtime in resolving the string.
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// The style controls the text's font size, weight, and opacity.
    /// </summary>
    public AdaptiveTextStyle HintStyle { get; set; }

    /// <summary>
    /// Set this to true to enable text wrapping. False by default.
    /// </summary>
    public bool? HintWrap { get; set; }

    /// <summary>
    /// The maximum number of lines the text element is allowed to display.
    /// </summary>
    public int? HintMaxLines
    {
        get;
        set
        {
            if (value is not null)
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value.Value, 1, nameof(HintMaxLines));
            }

            field = value;
        }
    }

    /// <summary>
    /// The minimum number of lines the text element must display.
    /// </summary>
    public int? HintMinLines
    {
        get;
        set
        {
            if (value is not null)
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value.Value, 1, nameof(HintMinLines));
            }

            field = value;
        }
    }

    /// <summary>
    /// The horizontal alignment of the text.
    /// </summary>
    public AdaptiveTextAlign HintAlign { get; set; }

    internal ElementAdaptiveText ConvertToElement()
    {
        return new()
        {
            Text = Text,
            Lang = Language,
            Style = HintStyle,
            Wrap = HintWrap,
            MaxLines = HintMaxLines,
            MinLines = HintMinLines,
            Align = HintAlign
        };
    }

    /// <summary>
    /// Returns the value of the Text property.
    /// </summary>
    /// <returns></returns>
    public override string? ToString()
    {
        return Text;
    }
}