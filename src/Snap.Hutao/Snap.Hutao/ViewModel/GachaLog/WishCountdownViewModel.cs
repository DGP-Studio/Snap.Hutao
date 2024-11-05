// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.GachaLog;

namespace Snap.Hutao.ViewModel.GachaLog;

[Injection(InjectAs.Singleton)]
[ConstructorGenerated(CallBaseConstructor = true)]
internal sealed partial class WishCountdownViewModel : Abstraction.ViewModelSlim
{
    private readonly IGachaLogWishCountdownService gachaLogWishCountdownService;
    private readonly ITaskContext taskContext;

    private WishCountdowns? wishCountdowns;

    public WishCountdowns? WishCountdowns { get => wishCountdowns; set => SetProperty(ref wishCountdowns, value); }

    protected override async Task LoadAsync()
    {
        await taskContext.SwitchToBackgroundAsync();
        (bool isOk, WishCountdowns countdowns) = await gachaLogWishCountdownService.GetWishCountdownsAsync().ConfigureAwait(false);

        if (isOk)
        {
            await taskContext.SwitchToMainThreadAsync();
            WishCountdowns = countdowns;
            IsInitialized = true;
        }
    }
}
