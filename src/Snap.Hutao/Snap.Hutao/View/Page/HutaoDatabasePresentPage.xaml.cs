// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;
using Snap.Hutao.ViewModel;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 用于展示用途的胡桃数据库页面
/// 仅用于发布相关的统计数据
/// </summary>
public sealed partial class HutaoDatabasePresentPage : ScopedPage
{
    /// <summary>
    /// 构造一个新的胡桃数据库页面
    /// </summary>
    public HutaoDatabasePresentPage()
    {
        InitializeComponent();
        InitializeWith<HutaoDatabaseViewModel>();
    }
}
