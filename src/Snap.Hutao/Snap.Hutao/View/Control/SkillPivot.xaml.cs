// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control;
using Snap.Hutao.Model.Binding;
using Snap.Hutao.Model.Metadata.Avatar;
using System.Collections;

namespace Snap.Hutao.View.Control;

/// <summary>
/// 技能展柜
/// </summary>
[HighQuality]
internal sealed partial class SkillPivot : UserControl
{
    private static readonly DependencyProperty SkillsProperty = Property<SkillPivot>.Depend<List<Skill>>(nameof(Skills));
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
    public List<Skill> Skills
    {
        get => (List<Skill>)GetValue(SkillsProperty);
        set => SetValue(SkillsProperty, value);
    }

    /// <summary>
    /// 选中的项
    /// </summary>
    public object Selected
    {
        get => GetValue(SelectedProperty);
        set => SetValue(SelectedProperty, value);
    }

    /// <summary>
    /// 项目模板
    /// </summary>
    public DataTemplate ItemTemplate
    {
        get => (DataTemplate)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }
}
