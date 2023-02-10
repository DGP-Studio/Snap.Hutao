// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;
using Snap.Hutao.ViewModel;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 设置页面
/// </summary>
public sealed partial class SettingPage : ScopedPage
{
    /// <summary>
    /// 构造新的设置页面
    /// </summary>
    public SettingPage()
    {
        InitializeWith<SettingViewModel>();
        InitializeComponent();
    }
}