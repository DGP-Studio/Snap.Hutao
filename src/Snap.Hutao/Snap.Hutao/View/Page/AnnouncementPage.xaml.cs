// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control.Cancellable;
using Snap.Hutao.ViewModel;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 公告页面
/// </summary>
public sealed partial class AnnouncementPage : CancellablePage
{
    /// <summary>
    /// 构造一个新的公告页面
    /// </summary>
    public AnnouncementPage()
    {
        Initialize(Ioc.Default.GetRequiredService<AnnouncementViewModel>());
        InitializeComponent();
    }
}
