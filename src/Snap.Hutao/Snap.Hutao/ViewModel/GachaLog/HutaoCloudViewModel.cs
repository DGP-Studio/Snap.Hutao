// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.Logging;
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

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class HutaoCloudViewModel : Abstraction.ViewModel
{
    private readonly IGachaLogHutaoCloudService hutaoCloudService;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly INavigationService navigationService;
    private readonly HutaoUserOptions hutaoUserOptions;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    [GeneratedConstructor]
    public partial HutaoCloudViewModel(IServiceProvider serviceProvider);

    public ObservableCollection<HutaoCloudEntryOperationViewModel>? UidOperations { get; set => SetProperty(ref field, value); }

    public partial HutaoUserOptions Options { get; }

    internal ICommand? RetrieveCommand { get; set; }

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

    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        await hutaoUserOptions.WaitUserInfoInitializationAsync().ConfigureAwait(false);
        await RefreshUidCollectionAsync().ConfigureAwait(false);
        return true;
    }

    [Command("NavigateToAfdianSkuCommand")]
    private async Task NavigateToAfdianSkuAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Navigate to passport page", "HutaoCloudViewModel.Command"));

        await navigationService.NavigateAsync<HutaoPassportPage>(NavigationExtraData.Default, true).ConfigureAwait(false);
    }

    [Command("UploadCommand")]
    private async Task UploadAsync(GachaArchive? gachaArchive)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Upload records", "HutaoCloudViewModel.Command"));

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
                messenger.Send(InfoBarMessage.Success(message));
                await RefreshUidCollectionAsync().ConfigureAwait(false);
            }
            else
            {
                messenger.Send(InfoBarMessage.Warning(message));
            }
        }
    }

    [Command("DeleteCommand")]
    private async Task DeleteAsync(string? uid)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Delete records", "HutaoCloudViewModel.Command"));

        if (uid is not null)
        {
            (bool isOk, string message) = await hutaoCloudService.DeleteGachaItemsAsync(uid).ConfigureAwait(false);

            if (isOk)
            {
                messenger.Send(InfoBarMessage.Success(message));
                await RefreshUidCollectionAsync().ConfigureAwait(false);
            }
            else
            {
                messenger.Send(InfoBarMessage.Warning(message));
            }
        }
    }

    [Command("NavigateToSpiralAbyssRecordCommand")]
    private void NavigateToSpiralAbyssRecord()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Navigate to spiralabyss page", "HutaoCloudViewModel.Command"));

        navigationService.Navigate<SpiralAbyssRecordPage>(NavigationExtraData.Default, true);
    }

    private async ValueTask RefreshUidCollectionAsync()
    {
        if (Options.IsHutaoCloudAllowed)
        {
            try
            {
                Response<ImmutableArray<GachaEntry>> resp = await hutaoCloudService.GetGachaEntriesAsync().ConfigureAwait(false);

                if (ResponseValidator.TryValidate(resp, messenger, out ImmutableArray<GachaEntry> entries))
                {
                    ObservableCollection<HutaoCloudEntryOperationViewModel> collection = entries
                        .SelectAsArray(static (entry, vm) => new HutaoCloudEntryOperationViewModel(entry, vm.RetrieveCommand, vm.DeleteCommand), this)
                        .ToObservableCollection();

                    await taskContext.SwitchToMainThreadAsync();
                    UidOperations = collection;
                }
            }
            catch (ObjectDisposedException)
            {
                // Ignored
            }
        }
    }
}