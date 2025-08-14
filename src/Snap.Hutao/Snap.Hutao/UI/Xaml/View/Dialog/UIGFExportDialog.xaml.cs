// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Factory.ContentDialog;
using System.Collections.Immutable;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty<IReadOnlyList<UIGFUidSelection>>("Selections")]
internal sealed partial class UIGFExportDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;

    public UIGFExportDialog(IServiceProvider serviceProvider, ImmutableArray<uint> uids)
    {
        InitializeComponent();
        contentDialogFactory = serviceProvider.GetRequiredService<IContentDialogFactory>();

        Selections = uids.SelectAsArray(static item => new UIGFUidSelection(item));
    }

    public async ValueTask<ValueResult<bool, ImmutableArray<uint>>> GetSelectedUidsAsync()
    {
        if (await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false) is ContentDialogResult.Primary)
        {
            await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();
            ImmutableArray<uint> uids = [.. Selections.Where(item => item.IsSelected).Select(item => item.Uid)];
            return new(true, uids);
        }

        return new(false, default!);
    }
}