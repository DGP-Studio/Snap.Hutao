// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;
using Snap.Hutao.ViewModel;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 角色属性页
/// </summary>
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
