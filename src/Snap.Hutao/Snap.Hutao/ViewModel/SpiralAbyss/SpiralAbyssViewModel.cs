// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.SpiralAbyss;
using Snap.Hutao.Service.User;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.UI.Xaml.View.Page;
using Snap.Hutao.ViewModel.Complex;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hutao.Response;
using Snap.Hutao.Web.Hutao.SpiralAbyss;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel.SpiralAbyss;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class SpiralAbyssViewModel : Abstraction.ViewModel, IRecipient<UserAndUidChangedMessage>
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly ISpiralAbyssService spiralAbyssService;
    private readonly INavigationService navigationService;
    private readonly HutaoUserOptions hutaoUserOptions;
    private readonly IServiceProvider serviceProvider;
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;
    private readonly IUserService userService;
    private readonly IMessenger messenger;

    private SpiralAbyssMetadataContext? metadataContext;

    [GeneratedConstructor]
    public partial SpiralAbyssViewModel(IServiceProvider serviceProvider);

    public IAdvancedCollectionView<SpiralAbyssView>? SpiralAbyssEntries
    {
        get;
        set
        {
            AdvancedCollectionViewCurrentChanged.Detach(field, OnCurrentSpiralAbyssEntryChanged);
            SetProperty(ref field, value);
            AdvancedCollectionViewCurrentChanged.Attach(field, OnCurrentSpiralAbyssEntryChanged);
        }
    }

    public partial HutaoSpiralAbyssDatabaseViewModel HutaoSpiralAbyssDatabaseViewModel { get; }

    public void Receive(UserAndUidChangedMessage message)
    {
        if (message.UserAndUid is { } userAndUid)
        {
            UpdateSpiralAbyssCollectionAsync(userAndUid).SafeForget();
        }
        else
        {
            SpiralAbyssEntries?.MoveCurrentTo(default);
        }
    }

    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        if (!await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            return false;
        }

        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is { } userAndUid)
        {
            metadataContext = await metadataService.GetContextAsync<SpiralAbyssMetadataContext>(token).ConfigureAwait(false);
            await UpdateSpiralAbyssCollectionAsync(userAndUid).ConfigureAwait(false);
        }
        else
        {
            messenger.Send(InfoBarMessage.Warning(SH.MustSelectUserAndUid));
        }

        return true;
    }

    private void OnCurrentSpiralAbyssEntryChanged(object? sender, object? e)
    {
        SpiralAbyssEntries?.CurrentItem?.Floors.MoveCurrentToFirst();
    }

    [SuppressMessage("", "SH003")]
    private async Task UpdateSpiralAbyssCollectionAsync(UserAndUid userAndUid)
    {
        if (metadataContext is null)
        {
            return;
        }

        try
        {
            ObservableCollection<SpiralAbyssView> collection;
            using (await EnterCriticalSectionAsync().ConfigureAwait(false))
            {
                collection = await spiralAbyssService
                    .GetSpiralAbyssViewCollectionAsync(metadataContext, userAndUid)
                    .ConfigureAwait(false);
            }

            IAdvancedCollectionView<SpiralAbyssView> spiralAbyssEntries = collection.AsAdvancedCollectionView();

            await taskContext.SwitchToMainThreadAsync();
            SpiralAbyssEntries = spiralAbyssEntries;
            SpiralAbyssEntries.MoveCurrentTo(SpiralAbyssEntries.Source.FirstOrDefault(s => s.Engaged));
        }
        catch (OperationCanceledException)
        {
        }
    }

    [Command("RefreshCommand")]
    private async Task RefreshAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Refresh spiral abyss record", "SpiralAbyssRecordViewModel.Command"));

        if (metadataContext is null)
        {
            return;
        }

        if (SpiralAbyssEntries is not null)
        {
            if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is { } userAndUid)
            {
                try
                {
                    using (await EnterCriticalSectionAsync().ConfigureAwait(false))
                    {
                        await spiralAbyssService
                            .RefreshSpiralAbyssAsync(metadataContext, userAndUid)
                            .ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException)
                {
                }

                await taskContext.SwitchToMainThreadAsync();
                SpiralAbyssEntries.MoveCurrentTo(SpiralAbyssEntries.Source.FirstOrDefault(s => s.Engaged));
            }
        }
    }

    [Command("UploadSpiralAbyssRecordCommand")]
    private async Task UploadSpiralAbyssRecordAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Upload spiral abyss record", "SpiralAbyssRecordViewModel.Command"));

        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is not { } userAndUid)
        {
            messenger.Send(InfoBarMessage.Warning(SH.MustSelectUserAndUid));
            return;
        }

        if (!hutaoUserOptions.IsLoggedIn)
        {
            SpiralAbyssUploadRecordHomaNotLoginDialog dialog = await contentDialogFactory
                .CreateInstanceAsync<SpiralAbyssUploadRecordHomaNotLoginDialog>(serviceProvider)
                .ConfigureAwait(false);

            ContentDialogResult result = await contentDialogFactory.EnqueueAndShowAsync(dialog).ShowTask.ConfigureAwait(false);

            switch (result)
            {
                case ContentDialogResult.Primary:
                    await navigationService.NavigateAsync<HutaoPassportPage>(NavigationExtraData.Default, true).ConfigureAwait(false);
                    return;

                case ContentDialogResult.Secondary:
                    break;

                case ContentDialogResult.None:
                    return;
            }
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HutaoSpiralAbyssClient spiralAbyssClient = scope.ServiceProvider.GetRequiredService<HutaoSpiralAbyssClient>();
            if (await spiralAbyssClient.GetPlayerRecordAsync(userAndUid).ConfigureAwait(false) is { } record)
            {
                HutaoResponse response = await spiralAbyssClient.UploadRecordAsync(record).ConfigureAwait(false);

                if (response is ILocalizableResponse localizableResponse)
                {
                    messenger.Send(InfoBarMessage.Any(
                        response is { ReturnCode: 0 } ? InfoBarSeverity.Success : InfoBarSeverity.Warning,
                        localizableResponse.GetLocalizationMessage()));
                }
            }

            // TODO: Handle no records
        }
    }
}