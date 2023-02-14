// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;
using Snap.Hutao.ViewModel;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 实时便笺页面
/// </summary>
internal sealed partial class DailyNotePage : ScopedPage
{
    /// <summary>
    /// 构造一个新的实时便笺页面
    /// </summary>
    public DailyNotePage()
    {
        InitializeWith<DailyNoteViewModel>();
        InitializeComponent();
    }
}
