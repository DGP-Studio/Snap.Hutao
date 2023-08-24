// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.View.Control;

/// <summary>
/// 物品图标
/// </summary>
[HighQuality]
[DependencyProperty("Quality", typeof(QualityType), QualityType.QUALITY_NONE)]
[DependencyProperty("Icon", typeof(Uri))]
[DependencyProperty("Badge", typeof(Uri))]
internal sealed partial class ItemIcon : UserControl
{
    /// <summary>
    /// 构造一个新的物品图标
    /// </summary>
    public ItemIcon()
    {
        InitializeComponent();
    }
}
