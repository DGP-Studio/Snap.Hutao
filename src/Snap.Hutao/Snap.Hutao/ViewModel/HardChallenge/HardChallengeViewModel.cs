// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Service.HardChallenge;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.ViewModel.User;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel.HardChallenge;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class HardChallengeViewModel : Abstraction.ViewModel, IRecipient<UserAndUidChangedMessage>
{
    private readonly IHardChallengeService hardChallengeService;
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;
    private readonly IUserService userService;
    private readonly IMessenger messenger;

    private HardChallengeMetadataContext? metadataContext;

    [GeneratedConstructor]
    public partial HardChallengeViewModel(IServiceProvider serviceProvider);

    public IAdvancedCollectionView<HardChallengeView>? HardChallengeEntries
    {
        get;
        set
        {
            AdvancedCollectionViewCurrentChanged.Detach(field, OnCurrentHardChallengeEntryChanged);
            SetProperty(ref field, value);
            AdvancedCollectionViewCurrentChanged.Attach(field, OnCurrentHardChallengeEntryChanged);
        }
    }

    [ObservableProperty]
    public partial ImmutableArray<AvatarView> BlingAvatars { get; set; }

    public void Receive(UserAndUidChangedMessage message)
    {
        if (message.UserAndUid is { } userAndUid)
        {
            UpdateHardChallengeCollectionAsync(userAndUid).SafeForget();
        }
        else
        {
            HardChallengeEntries?.MoveCurrentTo(default);
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
            metadataContext = await metadataService.GetContextAsync<HardChallengeMetadataContext>(token).ConfigureAwait(false);
            await UpdateHardChallengeCollectionAsync(userAndUid).ConfigureAwait(false);
        }
        else
        {
            messenger.Send(InfoBarMessage.Warning(SH.MustSelectUserAndUid));
        }

        return true;
    }

    private void OnCurrentHardChallengeEntryChanged(object? sender, object? e)
    {
        HardChallengeEntries?.CurrentItem?.DataEntries?.MoveCurrentToFirst();
    }

    [SuppressMessage("", "SH003")]
    private async Task UpdateHardChallengeCollectionAsync(UserAndUid userAndUid)
    {
        if (metadataContext is null)
        {
            return;
        }

        try
        {
            using (await EnterCriticalSectionAsync().ConfigureAwait(false))
            {
                ObservableCollection<HardChallengeView> collection = await hardChallengeService
                    .GetHardChallengeViewCollectionAsync(metadataContext, userAndUid)
                    .ConfigureAwait(false);

                ImmutableArray<AvatarView> blingAvatars = await hardChallengeService
                    .GetBlingAvatarsAsync(metadataContext, userAndUid)
                    .ConfigureAwait(false);

                IAdvancedCollectionView<HardChallengeView> hardChallengeEntries = collection.AsAdvancedCollectionView();

                await taskContext.SwitchToMainThreadAsync();
                HardChallengeEntries = hardChallengeEntries;
                HardChallengeEntries.MoveCurrentTo(HardChallengeEntries.Source.FirstOrDefault(s => s.Engaged));

                BlingAvatars = blingAvatars;
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    [Command("RefreshCommand")]
    private async Task RefreshAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Refresh hard challenge record", "HardChallengeViewModel.Command"));

        if (metadataContext is null)
        {
            return;
        }

        if (HardChallengeEntries is not null)
        {
            if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is { } userAndUid)
            {
                try
                {
                    using (await EnterCriticalSectionAsync().ConfigureAwait(false))
                    {
                        await hardChallengeService
                            .RefreshHardChallengeAsync(metadataContext, userAndUid)
                            .ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException)
                {
                }

                await taskContext.SwitchToMainThreadAsync();
                HardChallengeEntries.MoveCurrentTo(HardChallengeEntries.Source.FirstOrDefault(s => s.Engaged));
            }
        }
    }
}