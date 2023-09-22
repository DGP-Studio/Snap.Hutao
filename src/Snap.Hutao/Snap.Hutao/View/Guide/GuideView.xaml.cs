// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.ViewModel.Guide;

namespace Snap.Hutao.View.Guide;

/// <summary>
/// 指引视图
/// </summary>
internal sealed partial class GuideView : UserControl
{
    public GuideView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<GuideViewModel>();
    }
}
