// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;
using Snap.Hutao.ViewModel.Home;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 公告页面
/// </summary>
[HighQuality]
internal sealed partial class AnnouncementPage : ScopedPage
{
    /// <summary>
    /// 构造一个新的公告页面
    /// </summary>
    public AnnouncementPage()
    {
        InitializeWith<AnnouncementViewModel>();
        InitializeComponent();
    }
}