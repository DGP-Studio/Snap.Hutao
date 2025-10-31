// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.ViewModel.AvatarProperty;
using System.Collections.Immutable;
using WinRT;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty<IAdvancedCollectionView<AvatarView>>("Avatars")]
internal sealed partial class AvatarPropertyMultiAvatarCultivateSelectDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;

    [GeneratedConstructor(InitializeComponent = true)]
    public partial AvatarPropertyMultiAvatarCultivateSelectDialog(IServiceProvider serviceProvider);

    public ImmutableArray<AvatarView> SelectedAvatars { get; private set; } = [];

    public async ValueTask<bool> SelectAvatarsAsync()
    {
        return await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false) is ContentDialogResult.Primary;
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SelectedAvatars = [.. sender.As<ListViewBase>().SelectedItems.Cast<AvatarView>()];
    }
}