// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model.InterChange.GachaLog;
using System.Collections.Immutable;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty<UIGF>("UIGF")]
[DependencyProperty<ImmutableArray<UIGFUidSelection>>("Selections", NotNull = true)]
internal sealed partial class UIGFImportDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;

    public UIGFImportDialog(IServiceProvider serviceProvider, UIGF uigf)
    {
        InitializeComponent();
        contentDialogFactory = serviceProvider.GetRequiredService<IContentDialogFactory>();

        UIGF = uigf;
        Selections = uigf.Hk4e.SelectAsArray(static item => new UIGFUidSelection(item.Uid));
    }

    public async ValueTask<ValueResult<bool, HashSet<uint>>> GetSelectedUidsAsync()
    {
        if (await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false) is ContentDialogResult.Primary)
        {
            await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();
            HashSet<uint> uids = Selections.Where(item => item.IsSelected).Select(item => item.Uid).ToHashSet();
            return new(true, uids);
        }

        return new(false, default!);
    }
}