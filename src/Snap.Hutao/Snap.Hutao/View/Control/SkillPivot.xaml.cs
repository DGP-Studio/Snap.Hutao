// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections;

namespace Snap.Hutao.View.Control;

/// <summary>
/// 技能展柜
/// </summary>
[HighQuality]
[DependencyProperty("Skills", typeof(IList))]
[DependencyProperty("Selected", typeof(object))]
[DependencyProperty("ItemTemplate", typeof(DataTemplate))]
internal sealed partial class SkillPivot : UserControl
{
    /// <summary>
    /// 创建一个新的技能展柜
    /// </summary>
    public SkillPivot()
    {
        InitializeComponent();
    }
}