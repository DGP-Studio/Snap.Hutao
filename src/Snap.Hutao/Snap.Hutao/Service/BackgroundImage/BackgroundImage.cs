// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Media.Imaging;
using Snap.Hutao.Control.Media;
using Snap.Hutao.Core;
using System.IO;
using System.Runtime.InteropServices;
using Windows.Graphics.Imaging;
using Windows.UI;

namespace Snap.Hutao.Service.BackgroundImage;

internal sealed class BackgroundImage
{
    public BitmapImage ImageSource { get; set; } = default!;

    public Color AccentColor { get; set; }

    public double Luminance { get; set; }
}