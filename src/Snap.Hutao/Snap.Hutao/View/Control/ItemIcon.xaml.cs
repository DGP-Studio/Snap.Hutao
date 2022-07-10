// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.View.Control;

/// <summary>
/// 物品图标
/// </summary>
public sealed partial class ItemIcon : UserControl
{
    private static readonly DependencyProperty QualityProperty = Property<ItemIcon>.Depend(nameof(Quality), ItemQuality.QUALITY_NONE);
    private static readonly DependencyProperty IconProperty = Property<ItemIcon>.Depend<Uri>(nameof(Icon));

    /// <summary>
    /// 构造一个新的物品图标
    /// </summary>
    public ItemIcon()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 等阶
    /// </summary>
    public ItemQuality Quality
    {
        get => (ItemQuality)GetValue(QualityProperty);
        set => SetValue(QualityProperty, value);
    }

    /// <summary>
    /// 图标
    /// </summary>
    public Uri Icon
    {
        get => (Uri)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}
