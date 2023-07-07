// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.UI.Behaviors;
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
    /// <inheritdoc/>
    protected override void OnAssociatedObjectLoaded()
    {
        UpdateElement();
        AssociatedObject.SizeChanged += OnSizeChanged;
    }

    /// <inheritdoc/>
    protected override void OnDetaching()
    {
        AssociatedObject.SizeChanged -= OnSizeChanged;
        base.OnDetaching();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateElement();
    }

    private void UpdateElement()
    {
        AssociatedObject.Width = (double)AssociatedObject.Height * (TargetWidth / TargetHeight);
    }
}