// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Factory.ContentDialog;
using System.Collections.Immutable;
using WinRT;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty<IReadOnlyList<UIGFUidSelection>>("ItemsSource")]
internal sealed partial class UIGFExportDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;
    private ImmutableArray<uint> selectedUids = [];

    public UIGFExportDialog(IServiceProvider serviceProvider, ImmutableArray<uint> uids)
    {
        InitializeComponent();
        contentDialogFactory = serviceProvider.GetRequiredService<IContentDialogFactory>();

        ItemsSource = uids.SelectAsArray(UIGFUidSelection.Create);
    }

    public async ValueTask<ValueResult<bool, ImmutableArray<uint>>> GetSelectedUidsAsync()
    {
        if (await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false) is ContentDialogResult.Primary)
        {
            await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();
            SelectionListView.SelectAll();
            return new(true, selectedUids);
        }

        return new(false, selectedUids);
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        selectedUids = UIGFUidSelection.GetSelectedUidArray(sender.As<ListViewBase>());
    }
}