// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Animations;
using Microsoft.UI.Composition;

namespace Snap.Hutao.UI.Xaml.Media.Animation;

internal sealed class OpacityZoomOutAnimation : ImplicitAnimation<float, float>
{
    public OpacityZoomOutAnimation()
    {
        Duration = Constants.ImageOpacityFadeInOut;
        EasingMode = Microsoft.UI.Xaml.Media.Animation.EasingMode.EaseOut;
        EasingType = CommunityToolkit.WinUI.Animations.EasingType.Circle;
        To = 0;
    }

    protected override string ExplicitTarget
    {
        get => nameof(Visual.Opacity);
    }

    protected override (float?, float?) GetParsedValues()
    {
        return (To, From);
    }
}