// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Factory.ContentDialog;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty("Selections", typeof(List<UIGFUidSelection>))]
internal sealed partial class UIGFExportDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;

    public UIGFExportDialog(IServiceProvider serviceProvider, List<uint> uids)
    {
        InitializeComponent();
        contentDialogFactory = serviceProvider.GetRequiredService<IContentDialogFactory>();

        Selections = uids.SelectList(item => new UIGFUidSelection(item));
    }

    public async ValueTask<ValueResult<bool, List<uint>>> GetSelectedUidsAsync()
    {
        if (await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false) is ContentDialogResult.Primary)
        {
            await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();
            List<uint> uids = Selections.Where(item => item.IsSelected).Select(item => item.Uid).ToList();
            return new(true, uids);
        }

        return new(false, default!);
    }
}
