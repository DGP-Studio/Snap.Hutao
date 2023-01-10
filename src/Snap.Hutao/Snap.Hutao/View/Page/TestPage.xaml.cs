// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;
using Snap.Hutao.ViewModel;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 测试页面
/// </summary>
public sealed partial class TestPage : ScopedPage
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