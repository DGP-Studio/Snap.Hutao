// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service;
using System.Collections.Immutable;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty("Text", typeof(string))]
[DependencyProperty("SelectedServerTimeZoneOffset", typeof(NameValue<TimeSpan>))]
[DependencyProperty("IsUidAttached", typeof(bool))]
internal sealed partial class CultivateProjectDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;

    public CultivateProjectDialog(IContentDialogFactory contentDialogFactory)
    {
        this.contentDialogFactory = contentDialogFactory;
        InitializeComponent();

        SelectedServerTimeZoneOffset = ServerTimeZoneOffsets.First();
    }

    public ImmutableArray<NameValue<TimeSpan>> ServerTimeZoneOffsets { get => KnownServerRegionTimeZones.Value; }

    public async ValueTask<ValueResult<bool, CultivateProject>> CreateProjectAsync()
    {
        if (await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false) is ContentDialogResult.Primary)
        {
            await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();
            return new(true, CultivateProject.From(Text, SelectedServerTimeZoneOffset.Value));
        }

        return new(false, default!);
    }
}