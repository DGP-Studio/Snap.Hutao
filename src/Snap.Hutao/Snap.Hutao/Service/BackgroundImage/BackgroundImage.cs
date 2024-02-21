// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Media.Imaging;
using Windows.UI;

namespace Snap.Hutao.Service.BackgroundImage;

internal sealed class BackgroundImage
{
    public string Path { get; set; } = default!;

    public BitmapImage ImageSource { get; set; } = default!;

    public Color AccentColor { get; set; }

    public double Luminance { get; set; }
}