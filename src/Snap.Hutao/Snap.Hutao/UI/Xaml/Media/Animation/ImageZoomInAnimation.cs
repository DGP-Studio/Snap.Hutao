// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Animations;
using Microsoft.UI.Composition;
using System.Numerics;

namespace Snap.Hutao.UI.Xaml.Media.Animation;

internal sealed class ImageZoomInAnimation : ImplicitAnimation<string, Vector3>
{
    public ImageZoomInAnimation()
    {
        Duration = Constants.ImageZoom;
        EasingMode = Microsoft.UI.Xaml.Media.Animation.EasingMode.EaseOut;
        EasingType = CommunityToolkit.WinUI.Animations.EasingType.Circle;
        To = Constants.OnePointOne;
    }

    protected override string ExplicitTarget
    {
        get => nameof(Visual.Scale);
    }

    protected override (Vector3?, Vector3?) GetParsedValues()
    {
        return (To?.ToVector3(), From?.ToVector3());
    }
}