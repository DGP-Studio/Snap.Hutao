// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.UI.Behaviors;
using Microsoft.UI.Xaml;
using Snap.Hutao.Core;

namespace Snap.Hutao.Control.Behavior;

/// <summary>
/// 按给定比例自动调整高度的行为
/// </summary>
internal class AutoWidthBehavior : BehaviorBase<FrameworkElement>
{
    private static readonly DependencyProperty TargetWidthProperty = Property<AutoWidthBehavior>.Depend(nameof(TargetWidth), 320D);
    private static readonly DependencyProperty TargetHeightProperty = Property<AutoWidthBehavior>.Depend(nameof(TargetHeight), 1024D);

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
        UpdateElementWidth();
        AssociatedObject.SizeChanged += OnSizeChanged;
    }

    /// <inheritdoc/>
    protected override void OnDetaching()
    {
        AssociatedObject.SizeChanged -= OnSizeChanged;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateElementWidth();
    }

    private void UpdateElementWidth()
    {
        AssociatedObject.Width = (double)AssociatedObject.Height * (TargetWidth / TargetHeight);
    }
}