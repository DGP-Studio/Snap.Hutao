// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Common;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Adaptive;

/// <summary>
/// Controls the horizontal alignment of text.
/// </summary>
internal enum AdaptiveTextAlign
{
    /// <summary>
    /// Alignment is automatically determined by
    /// </summary>
    Default,

    /// <summary>
    /// The system automatically decides the alignment based on the language and culture.
    /// </summary>
    [EnumString("auto")]
    Auto,

    /// <summary>
    /// Horizontally align the text to the left.
    /// </summary>
    [EnumString("left")]
    Left,

    /// <summary>
    /// Horizontally align the text in the center.
    /// </summary>
    [EnumString("center")]
    Center,

    /// <summary>
    /// Horizontally align the text to the right.
    /// </summary>
    [EnumString("right")]
    Right
}