// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Behaviors;
using Microsoft.UI.Xaml;

namespace Snap.Hutao.Control.Behavior;

/// <summary>
/// 按给定比例自动调整高度的行为
/// </summary>
[HighQuality]
[DependencyProperty("TargetWidth", typeof(double), 1.0D)]
[DependencyProperty("TargetHeight", typeof(double), 1.0D)]
internal sealed partial class AutoWidthBehavior : BehaviorBase<FrameworkElement>
{
    private readonly SizeChangedEventHandler sizeChangedEventHandler;

    public AutoWidthBehavior()
    {
        sizeChangedEventHandler = OnSizeChanged;
    }

    /// <inheritdoc/>
    protected override bool Initialize()
    {
        UpdateElement();
        AssociatedObject.SizeChanged += sizeChangedEventHandler;
        return true;
    }

    /// <inheritdoc/>
    protected override bool Uninitialize()
    {
        AssociatedObject.SizeChanged -= sizeChangedEventHandler;
        return true;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateElement();
    }

    private void UpdateElement()
    {
        AssociatedObject.Width = AssociatedObject.Height * (TargetWidth / TargetHeight);
    }
}