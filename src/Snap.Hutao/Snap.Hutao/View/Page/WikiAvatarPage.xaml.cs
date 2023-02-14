// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;
using Snap.Hutao.ViewModel;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 角色资料页
/// </summary>
internal sealed partial class WikiAvatarPage : ScopedPage
{
    /// <summary>
    /// 构造一个新的角色资料页
    /// </summary>
    public WikiAvatarPage()
    {
        InitializeWith<WikiAvatarViewModel>();
        InitializeComponent();
    }
}
