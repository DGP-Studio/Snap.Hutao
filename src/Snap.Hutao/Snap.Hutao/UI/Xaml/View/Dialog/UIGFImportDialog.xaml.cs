// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model.InterChange.GachaLog;
using System.Collections.Immutable;
using WinRT;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty<UIGF>("UIGF")]
[DependencyProperty<ImmutableArray<UIGFUidSelection>>("ItemsSource", NotNull = true)]
internal sealed partial class UIGFImportDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;
    private ImmutableArray<uint> selectedUids = [];

    public UIGFImportDialog(IServiceProvider serviceProvider, UIGF uigf)
    {
        InitializeComponent();
        contentDialogFactory = serviceProvider.GetRequiredService<IContentDialogFactory>();

        UIGF = uigf;
        ItemsSource = uigf.Hk4e.SelectAsArray(static item => UIGFUidSelection.Create(item.Uid));
    }

    public async ValueTask<ValueResult<bool, HashSet<uint>>> GetSelectedUidsAsync()
    {
        if (await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false) is ContentDialogResult.Primary)
        {
            await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();
            return new(true, selectedUids.ToHashSet());
        }

        return new(false, default!);
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        selectedUids = UIGFUidSelection.GetSelectedUidArray(sender.As<ListViewBase>());
    }
}