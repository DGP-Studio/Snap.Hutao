// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core;
using System.Collections;

namespace Snap.Hutao.View.Control;

/// <summary>
/// 技能展柜
/// </summary>
public sealed partial class SkillPivot : UserControl
{
    private static readonly DependencyProperty SkillsProperty = Property<SkillPivot>.Depend<IList>(nameof(Skills));
    private static readonly DependencyProperty SelectedProperty = Property<SkillPivot>.Depend<object>(nameof(Selected));
    private static readonly DependencyProperty ItemTemplateProperty = Property<SkillPivot>.Depend<DataTemplate>(nameof(ItemTemplate));

    /// <summary>
    /// 创建一个新的技能展柜
    /// </summary>
    public SkillPivot()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 技能列表
    /// </summary>
    public IList Skills
    {
        get { return (IList)GetValue(SkillsProperty); }
        set { SetValue(SkillsProperty, value); }
    }

    /// <summary>
    /// 选中的项
    /// </summary>
    public object Selected
    {
        get { return GetValue(SelectedProperty); }
        set { SetValue(SelectedProperty, value); }
    }

    /// <summary>
    /// 项目模板
    /// </summary>
    public DataTemplate ItemTemplate
    {
        get { return (DataTemplate)GetValue(ItemTemplateProperty); }
        set { SetValue(ItemTemplateProperty, value); }
    }
}
