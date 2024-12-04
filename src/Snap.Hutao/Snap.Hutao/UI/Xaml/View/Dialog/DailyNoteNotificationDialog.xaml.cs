// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Model.Entity;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

internal sealed partial class DailyNoteNotificationDialog : ContentDialog
{
    public DailyNoteNotificationDialog(IServiceProvider serviceProvider, DailyNoteEntry entry)
    {
        InitializeComponent();

        DataContext = entry;
    }
}
