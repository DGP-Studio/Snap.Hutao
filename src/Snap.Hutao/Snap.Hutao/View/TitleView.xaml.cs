// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.ViewModel;

namespace Snap.Hutao.View;

/// <summary>
/// 标题视图
/// </summary>
[HighQuality]
internal sealed partial class TitleView : UserControl
{
    public TitleView()
    {
        DataContext = Ioc.Default.GetRequiredService<TitleViewModel>();
        InitializeComponent();
    }

    public FrameworkElement DragArea
    {
        get => DragableGrid;
    }
}