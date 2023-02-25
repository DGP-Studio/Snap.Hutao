// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;
using Snap.Hutao.ViewModel;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 怪物资料页面
/// </summary>
internal sealed partial class WikiMonsterPage : ScopedPage
{
    /// <summary>
    /// 构造一个新的怪物资料页面
    /// </summary>
    public WikiMonsterPage()
    {
        InitializeComponent();
        InitializeWith<WikiMonsterViewModel>();
    }
}
