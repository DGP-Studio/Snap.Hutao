// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Service.GachaLog;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;

namespace Snap.Hutao.ViewModel.GachaLog;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Transient)]
internal sealed partial class WishCountdownViewModel : Abstraction.ViewModelSlim
{
    private readonly IGachaLogWishCountdownService gachaLogWishCountdownService;
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;

    [GeneratedConstructor(CallBaseConstructor = true)]
    public partial WishCountdownViewModel(IServiceProvider serviceProvider);

    [ObservableProperty]
    public partial WishCountdownBundle? WishCountdowns { get; set; }

    protected override async Task LoadAsync()
    {
        if (!await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            return;
        }

        GachaLogWishCountdownServiceMetadataContext context = await metadataService.GetContextAsync<GachaLogWishCountdownServiceMetadataContext>().ConfigureAwait(false);
        WishCountdownBundle countdowns = await gachaLogWishCountdownService.GetWishCountdownBundleAsync(context).ConfigureAwait(false);
        await taskContext.SwitchToMainThreadAsync();
        WishCountdowns = countdowns;
        IsInitialized = true;
    }
}