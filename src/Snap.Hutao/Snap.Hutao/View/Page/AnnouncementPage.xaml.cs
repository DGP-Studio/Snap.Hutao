// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Navigation;
using Snap.Hutao.Control;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.ViewModel;

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
