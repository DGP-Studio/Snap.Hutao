// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.ViewModel.AvatarProperty;
using System.Collections.Immutable;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty("Avatars", typeof(IAdvancedCollectionView<AvatarView>))]
[ConstructorGenerated(InitializeComponent = true)]
internal sealed partial class AvatarPropertyMultiAvatarCultivateSelectDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;

    public ImmutableArray<AvatarView> SelectedAvatars { get; private set; } = [];

    public async ValueTask<bool> SelectAvatarsAsync()
    {
        return await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false) is ContentDialogResult.Primary;
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SelectedAvatars = [.. ((ListViewBase)sender).SelectedItems.Cast<AvatarView>()];
    }
}