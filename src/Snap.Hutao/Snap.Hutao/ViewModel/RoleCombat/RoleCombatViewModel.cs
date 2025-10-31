// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.RoleCombat;
using Snap.Hutao.Service.User;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.ViewModel.Complex;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hutao.Response;
using Snap.Hutao.Web.Hutao.RoleCombat;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel.RoleCombat;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class RoleCombatViewModel : Abstraction.ViewModel, IRecipient<UserAndUidChangedMessage>
{
    private readonly IRoleCombatService roleCombatService;
    private readonly IServiceProvider serviceProvider;
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;
    private readonly IUserService userService;
    private readonly IMessenger messenger;

    private RoleCombatMetadataContext? metadataContext;

    [GeneratedConstructor]
    public partial RoleCombatViewModel(IServiceProvider serviceProvider);

    [ObservableProperty]
    public partial IAdvancedCollectionView<RoleCombatView>? RoleCombatEntries { get; set; }

    public partial HutaoRoleCombatDatabaseViewModel HutaoRoleCombatDatabaseViewModel { get; }

    public void Receive(UserAndUidChangedMessage message)
    {
        if (message.UserAndUid is { } userAndUid)
        {
            UpdateRoleCombatCollectionAsync(userAndUid).SafeForget();
        }
        else
        {
            RoleCombatEntries?.MoveCurrentTo(default);
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
            metadataContext = await metadataService.GetContextAsync<RoleCombatMetadataContext>(token).ConfigureAwait(false);
            await UpdateRoleCombatCollectionAsync(userAndUid).ConfigureAwait(false);
        }
        else
        {
            messenger.Send(InfoBarMessage.Warning(SH.MustSelectUserAndUid));
        }

        return true;
    }

    [SuppressMessage("", "SH003")]
    private async Task UpdateRoleCombatCollectionAsync(UserAndUid userAndUid)
    {
        if (metadataContext is null)
        {
            return;
        }

        try
        {
            ObservableCollection<RoleCombatView> collection;
            using (await EnterCriticalSectionAsync().ConfigureAwait(false))
            {
                collection = await roleCombatService
                    .GetRoleCombatViewCollectionAsync(metadataContext, userAndUid)
                    .ConfigureAwait(false);
            }

            IAdvancedCollectionView<RoleCombatView> roleCombatEntries = collection.AsAdvancedCollectionView();

            await taskContext.SwitchToMainThreadAsync();
            RoleCombatEntries = roleCombatEntries;
            RoleCombatEntries.MoveCurrentTo(RoleCombatEntries.Source.FirstOrDefault(s => s.Engaged));
        }
        catch (OperationCanceledException)
        {
        }
    }

    [Command("RefreshCommand")]
    private async Task RefreshAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Refresh role combat record", "RoleCombatViewModel.Command"));

        if (metadataContext is null)
        {
            return;
        }

        if (RoleCombatEntries is not null)
        {
            if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is { } userAndUid)
            {
                try
                {
                    using (await EnterCriticalSectionAsync().ConfigureAwait(false))
                    {
                        await roleCombatService
                            .RefreshRoleCombatAsync(metadataContext, userAndUid)
                            .ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException)
                {
                }

                await taskContext.SwitchToMainThreadAsync();
                RoleCombatEntries.MoveCurrentTo(RoleCombatEntries.Source.FirstOrDefault(s => s.Engaged));
            }
        }
    }

    [Command("UploadRoleCombatRecordCommand")]
    private async Task UploadRoleCombatRecordAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Upload role combat record", "RoleCombatViewModel.Command"));

        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is not { } userAndUid)
        {
            messenger.Send(InfoBarMessage.Warning(SH.MustSelectUserAndUid));
            return;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HutaoRoleCombatClient roleCombatClient = scope.ServiceProvider.GetRequiredService<HutaoRoleCombatClient>();
            if (await roleCombatClient.GetPlayerRecordAsync(userAndUid).ConfigureAwait(false) is { } record)
            {
                HutaoResponse response = await roleCombatClient.UploadRecordAsync(record).ConfigureAwait(false);

                messenger.Send(InfoBarMessage.Any(
                    response is { ReturnCode: 0 } ? InfoBarSeverity.Success : InfoBarSeverity.Warning,
                    response.GetLocalizationMessageOrMessage()));
            }

            // TODO: Handle no records
        }
    }
}