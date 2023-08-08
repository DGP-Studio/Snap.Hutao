// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
// Some part of this file came from:
// https://github.com/xunkong/desktop/tree/main/src/Desktop/Desktop/Pages/CharacterInfoPage.xaml.cs

namespace Snap.Hutao.Control.Media;

/// <summary>
/// Defines a color in Hue/Saturation/Lightness (HSL) space.
/// </summary>
internal struct Hsl32
{
    /// <summary>
    /// The Hue in 0..360 range.
    /// </summary>
    public double H;

    /// <summary>
    /// The Saturation in 0..1 range.
    /// </summary>
    public double S;

    /// <summary>
    /// The Lightness in 0..1 range.
    /// </summary>
    public double L;

    /// <summary>
    /// The Alpha/opacity in 0..1 range.
    /// </summary>
    public double A;
}