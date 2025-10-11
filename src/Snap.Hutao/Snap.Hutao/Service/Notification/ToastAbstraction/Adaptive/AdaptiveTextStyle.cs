// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Common;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Adaptive;

/// <summary>
/// Text style controls font size, weight, and opacity.
/// </summary>
internal enum AdaptiveTextStyle
{
    /// <summary>
    /// Style is determined by the renderer.
    /// </summary>
    Default,

    /// <summary>
    /// Default value. Paragraph font size, normal weight and opacity.
    /// </summary>
    [EnumString("caption")]
    Caption,

    /// <summary>
    /// Same as Caption but with subtle opacity.
    /// </summary>
    [EnumString("captionSubtle")]
    CaptionSubtle,

    /// <summary>
    /// H5 font size.
    /// </summary>
    [EnumString("body")]
    Body,

    /// <summary>
    /// Same as Body but with subtle opacity.
    /// </summary>
    [EnumString("bodySubtle")]
    BodySubtle,

    /// <summary>
    /// H5 font size, bold weight. Essentially the bold version of Body.
    /// </summary>
    [EnumString("base")]
    Base,

    /// <summary>
    /// Same as Base but with subtle opacity.
    /// </summary>
    [EnumString("baseSubtle")]
    BaseSubtle,

    /// <summary>
    /// H4 font size.
    /// </summary>
    [EnumString("subtitle")]
    Subtitle,

    /// <summary>
    /// Same as Subtitle but with subtle opacity.
    /// </summary>
    [EnumString("subtitleSubtle")]
    SubtitleSubtle,

    /// <summary>
    /// H3 font size.
    /// </summary>
    [EnumString("title")]
    Title,

    /// <summary>
    /// Same as Title but with subtle opacity.
    /// </summary>
    [EnumString("titleSubtle")]
    TitleSubtle,

    /// <summary>
    /// Same as Title but with top/bottom padding removed.
    /// </summary>
    [EnumString("titleNumeral")]
    TitleNumeral,

    /// <summary>
    /// H2 font size.
    /// </summary>
    [EnumString("subheader")]
    Subheader,

    /// <summary>
    /// Same as Subheader but with subtle opacity.
    /// </summary>
    [EnumString("subheaderSubtle")]
    SubheaderSubtle,

    /// <summary>
    /// Same as Subheader but with top/bottom padding removed.
    /// </summary>
    [EnumString("subheaderNumeral")]
    SubheaderNumeral,

    /// <summary>
    /// H1 font size.
    /// </summary>
    [EnumString("header")]
    Header,

    /// <summary>
    /// Same as Header but with subtle opacity.
    /// </summary>
    [EnumString("headerSubtle")]
    HeaderSubtle,

    /// <summary>
    /// Same as Header but with top/bottom padding removed.
    /// </summary>
    [EnumString("headerNumeral")]
    HeaderNumeral
}