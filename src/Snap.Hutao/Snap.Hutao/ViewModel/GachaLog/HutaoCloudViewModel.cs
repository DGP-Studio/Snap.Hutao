// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.GachaLog;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.View.Page;
using Snap.Hutao.Web.Hutao.GachaLog;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel.GachaLog;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class HutaoCloudViewModel : Abstraction.ViewModel
{
    private readonly IGachaLogHutaoCloudService hutaoCloudService;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly INavigationService navigationService;
    private readonly HutaoUserOptions hutaoUserOptions;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;

    public ObservableCollection<HutaoCloudEntryOperationViewModel>? UidOperations { get; set => SetProperty(ref field, value); }

    public partial HutaoUserOptions Options { get; }

    internal ICommand RetrieveCommand { get; set; }

    internal async ValueTask<ValueResult<bool, Guid>> RetrieveAsync(string uid)
    {
        ContentDialog dialog = await contentDialogFactory
            .CreateForIndeterminateProgressAsync(SH.ViewModelGachaLogRetrieveFromHutaoCloudProgress)
            .ConfigureAwait(false);

        using (await contentDialogFactory.BlockAsync(dialog).ConfigureAwait(false))
        {
            return await hutaoCloudService.RetrieveGachaArchiveIdAsync(uid).ConfigureAwait(false);
        }
    }

    protected override async ValueTask<bool> LoadOverrideAsync()
    {
        await hutaoUserOptions.WaitUserInfoInitializationAsync().ConfigureAwait(false);
        await RefreshUidCollectionAsync().ConfigureAwait(false);
        return true;
    }

    [Command("NavigateToAfdianSkuCommand")]
    private async Task NavigateToAfdianSkuAsync()
    {
        await navigationService.NavigateAsync<HutaoPassportPage>(INavigationCompletionSource.Default, true);
    }

    [Command("UploadCommand")]
    private async Task UploadAsync(GachaArchive? gachaArchive)
    {
        if (gachaArchive is not null)
        {
            ContentDialog dialog = await contentDialogFactory
                .CreateForIndeterminateProgressAsync(SH.ViewModelGachaLogUploadToHutaoCloudProgress)
                .ConfigureAwait(false);

            bool isOk;
            string message;

            using (await contentDialogFactory.BlockAsync(dialog).ConfigureAwait(false))
            {
                (isOk, message) = await hutaoCloudService.UploadGachaItemsAsync(gachaArchive).ConfigureAwait(false);
            }

            if (isOk)
            {
                infoBarService.Success(message);
                await RefreshUidCollectionAsync().ConfigureAwait(false);
            }
            else
            {
                infoBarService.Warning(message);
            }
        }
    }

    [Command("DeleteCommand")]
    private async Task DeleteAsync(string? uid)
    {
        if (uid is not null)
        {
            (bool isOk, string message) = await hutaoCloudService.DeleteGachaItemsAsync(uid).ConfigureAwait(false);

            if (isOk)
            {
                infoBarService.Success(message);
                await RefreshUidCollectionAsync().ConfigureAwait(false);
            }
            else
            {
                infoBarService.Warning(message);
            }
        }
    }

    [Command("NavigateToSpiralAbyssRecordCommand")]
    private void NavigateToSpiralAbyssRecord()
    {
        navigationService.Navigate<SpiralAbyssRecordPage>(INavigationCompletionSource.Default, true);
    }

    private async ValueTask RefreshUidCollectionAsync()
    {
        if (Options.IsHutaoCloudAllowed)
        {
            Response<ImmutableArray<GachaEntry>> resp = await hutaoCloudService.GetGachaEntriesAsync().ConfigureAwait(false);

            if (ResponseValidator.TryValidate(resp, infoBarService, out ImmutableArray<GachaEntry> entries))
            {
                ObservableCollection<HutaoCloudEntryOperationViewModel> collection = entries
                    .SelectAsArray(entry => new HutaoCloudEntryOperationViewModel(entry, RetrieveCommand, DeleteCommand))
                    .ToObservableCollection();

                await taskContext.SwitchToMainThreadAsync();
                UidOperations = collection;
            }
        }
    }
}