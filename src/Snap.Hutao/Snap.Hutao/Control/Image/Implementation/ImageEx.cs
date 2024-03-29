// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Windows.Media.Casting;

namespace Snap.Hutao.Control.Image.Implementation;

[DependencyProperty("NineGrid", typeof(Thickness))]
internal partial class ImageEx : ImageExBase
{
    public ImageEx()
        : base()
    {
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