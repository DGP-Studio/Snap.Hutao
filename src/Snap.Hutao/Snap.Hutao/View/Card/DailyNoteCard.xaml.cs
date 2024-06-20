// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.UI.Xaml;

namespace Snap.Hutao.View.Card;

/// <summary>
/// 实时便笺卡片
/// </summary>
internal sealed partial class DailyNoteCard : Button
{
    /// <summary>
    /// 构造一个新的实时便笺卡片
    /// </summary>
    public DailyNoteCard()
    {
        this.InitializeDataContext<ViewModel.DailyNote.DailyNoteViewModelSlim>();
        InitializeComponent();
    }
}