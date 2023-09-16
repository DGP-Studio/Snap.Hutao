// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Windows.Media.Casting;

namespace Snap.Hutao.Control.Image.Implementation;

internal class ImageEx : ImageExBase
{
    private static readonly DependencyProperty NineGridProperty = DependencyProperty.Register(nameof(NineGrid), typeof(Thickness), typeof(ImageEx), new PropertyMetadata(default(Thickness)));

    public ImageEx()
        : base()
    {
    }

    public Thickness NineGrid
    {
        get => (Thickness)GetValue(NineGridProperty);
        set => SetValue(NineGridProperty, value);
    }

    public override CompositionBrush GetAlphaMask()
    {
        if (IsInitialized && Image is Microsoft.UI.Xaml.Controls.Image image)
        {
            return image.GetAlphaMask();
        }

        return default!;
    }

    public CastingSource GetAsCastingSource()
    {
        if (IsInitialized && Image is Microsoft.UI.Xaml.Controls.Image image)
        {
            return image.GetAsCastingSource();
        }

        return default!;
    }
}