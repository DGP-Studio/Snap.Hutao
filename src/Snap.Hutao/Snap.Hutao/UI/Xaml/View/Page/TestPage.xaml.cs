﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Control;
using Snap.Hutao.ViewModel;

namespace Snap.Hutao.UI.Xaml.View.Page;

/// <summary>
/// 测试页面
/// </summary>
[HighQuality]
internal sealed partial class TestPage : ScopedPage
{
    /// <summary>
    /// 构造一个新的测试页面
    /// </summary>
    public TestPage()
    {
        InitializeWith<TestViewModel>();
        InitializeComponent();
    }
}