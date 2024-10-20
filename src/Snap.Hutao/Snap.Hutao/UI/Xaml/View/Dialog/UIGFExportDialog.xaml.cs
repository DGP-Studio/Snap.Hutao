// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty("Selections", typeof(List<UIGFUidSelection>))]
internal sealed partial class UIGFExportDialog : ContentDialog
{
    private readonly ITaskContext taskContext;

    public UIGFExportDialog(IServiceProvider serviceProvider, List<uint> uids)
    {
        InitializeComponent();
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();

        Selections = uids.SelectList(item => new UIGFUidSelection(item));
    }

    public async ValueTask<ValueResult<bool, List<uint>>> GetSelectedUidsAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        if (await ShowAsync() is ContentDialogResult.Primary)
        {
            List<uint> uids = Selections.Where(item => item.IsSelected).Select(item => item.Uid).ToList();
            return new(true, uids);
        }

        return new(false, default!);
    }
}
