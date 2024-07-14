// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Model.InterChange.GachaLog;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty("UIGF", typeof(UIGF))]
[DependencyProperty("Selections", typeof(List<UIGFUidSelection>))]
internal sealed partial class UIGFImportDialog : ContentDialog
{
    private readonly ITaskContext taskContext;

    public UIGFImportDialog(IServiceProvider serviceProvider, UIGF uigf)
    {
        InitializeComponent();
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();

        UIGF = uigf;
        Selections = uigf.Hk4e?.SelectList(item => new UIGFUidSelection(item.Uid));
    }

    public async ValueTask<ValueResult<bool, HashSet<string>>> GetSelectedUidsAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        if (await ShowAsync() is ContentDialogResult.Primary)
        {
            HashSet<string> uids = Selections.Where(item => item.IsSelected).Select(item => item.Uid).ToHashSet();
            return new(true, uids);
        }

        return new(false, default!);
    }
}