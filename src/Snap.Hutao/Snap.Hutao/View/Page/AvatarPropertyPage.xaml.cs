// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Control;
using Snap.Hutao.ViewModel.AvatarProperty;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 角色属性页
/// </summary>
[HighQuality]
internal sealed partial class AvatarPropertyPage : ScopedPage
{
    /// <summary>
    /// 初始化一个新的角色属性页
    /// </summary>
    public AvatarPropertyPage()
    {
        InitializeWith<AvatarPropertyViewModel>();
        InitializeComponent();
    }
}
