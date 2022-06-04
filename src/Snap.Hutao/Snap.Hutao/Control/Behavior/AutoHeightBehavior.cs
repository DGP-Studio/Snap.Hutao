// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.UI.Behaviors;
using Microsoft.UI.Xaml;
using Microsoft.Xaml.Interactivity;
using Snap.Hutao.Core;

namespace Snap.Hutao.Control.Behavior;

/// <summary>
/// 按给定比例自动调整高度的行为
/// </summary>
internal class AutoHeightBehavior : BehaviorBase<FrameworkElement>
{
    private static readonly DependencyProperty TargetWidthProperty = Property<AutoHeightBehavior>.Depend(nameof(TargetWidth), 1080D);
    private static readonly DependencyProperty TargetHeightProperty = Property<AutoHeightBehavior>.Depend(nameof(TargetHeight), 390D);

    /// <summary>
    /// 目标宽度
    /// </summary>
    public double TargetWidth
    {
        get => (double)GetValue(TargetWidthProperty);

        set => SetValue(TargetWidthProperty, value);
    }

    /// <summary>
    /// 目标高度
    /// </summary>
    public double TargetHeight
    {
        get => (double)GetValue(TargetHeightProperty);

        set => SetValue(TargetHeightProperty, value);
    }

    /// <inheritdoc/>
    protected override void OnAssociatedObjectLoaded()
    {
        AssociatedObject.SizeChanged += OnSizeChanged;
        UpdateElementHeight();
    }

    /// <inheritdoc/>
    protected override void OnDetaching()
    {
        AssociatedObject.SizeChanged -= OnSizeChanged;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateElementHeight();
    }

    private void UpdateElementHeight()
    {
        AssociatedObject.Height = (double)AssociatedObject.ActualWidth * (TargetHeight / TargetWidth);
    }
}