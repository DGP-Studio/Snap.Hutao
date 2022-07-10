// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 角色资料页
/// </summary>
public sealed partial class WikiAvatarPage : Microsoft.UI.Xaml.Controls.Page
{
    /// <summary>
    /// 构造一个新的角色资料页
    /// </summary>
    public WikiAvatarPage()
    {
        DataContext = Ioc.Default.GetRequiredService<WikiAvatarViewModel>();
        InitializeComponent();
    }
}
