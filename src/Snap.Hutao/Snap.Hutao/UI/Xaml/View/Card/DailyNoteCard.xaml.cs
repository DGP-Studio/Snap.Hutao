// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.UI.Xaml.View.Card;

internal sealed partial class DailyNoteCard : Button
{
    public DailyNoteCard(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        this.InitializeDataContext<ViewModel.DailyNote.DailyNoteViewModelSlim>(serviceProvider);
    }
}