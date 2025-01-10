// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Media.Imaging;
using Windows.UI;

namespace Snap.Hutao.Service.BackgroundImage;

internal sealed class BackgroundImage
{
    public required string Path { get; init; }

    public required BitmapImage ImageSource { get; init; }

    public required Color AccentColor { get; init; }

    public required double Luminance { get; init; }
}