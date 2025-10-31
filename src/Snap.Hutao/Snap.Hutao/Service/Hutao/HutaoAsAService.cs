// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Web.Hutao.HutaoAsAService;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Globalization;
using Windows.Storage;
using HutaoAnnouncement = Snap.Hutao.Web.Hutao.HutaoAsAService.Announcement;

namespace Snap.Hutao.Service.Hutao;

[Service(ServiceLifetime.Scoped, typeof(IHutaoAsAService))]
internal sealed partial class HutaoAsAService : IHutaoAsAService
{
    private const int AnnouncementDuration = 30;
    private readonly IServiceScopeFactory serviceScopeFactory;

    private ObservableCollection<HutaoAnnouncement>? announcements;
    private ICommand? dismissCommand;

    [GeneratedConstructor]
    public partial HutaoAsAService(IServiceProvider serviceProvider);

    public async ValueTask<ObservableCollection<HutaoAnnouncement>> GetHutaoAnnouncementCollectionAsync(CancellationToken token = default)
    {
        if (announcements is null)
        {
            // Strong reference
            dismissCommand = new RelayCommand<HutaoAnnouncement>(DismissAnnouncement);

            ApplicationDataCompositeValue excludedIds = LocalSetting.Get<ApplicationDataCompositeValue>(SettingKeys.ExcludedAnnouncementIds, []);
            ImmutableArray<long> data = [.. excludedIds.Select(kvp => long.Parse(kvp.Key, CultureInfo.InvariantCulture))];

            ImmutableArray<HutaoAnnouncement> array;
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                HutaoAsAServiceClient hutaoAsAServiceClient = scope.ServiceProvider.GetRequiredService<HutaoAsAServiceClient>();
                Response<ImmutableArray<HutaoAnnouncement>> response = await hutaoAsAServiceClient.GetAnnouncementListAsync(data, token).ConfigureAwait(false);

                if (!ResponseValidator.TryValidate(response, scope.ServiceProvider, out array))
                {
                    return [];
                }
            }

            foreach (HutaoAnnouncement item in array)
            {
                item.DismissCommand = dismissCommand;
            }

            announcements = array.ToObservableCollection();
        }

        return announcements;
    }

    private void DismissAnnouncement(HutaoAnnouncement? announcement)
    {
        if (announcement is not null && announcements is not null)
        {
            ApplicationDataCompositeValue excludedIds = LocalSetting.Get<ApplicationDataCompositeValue>(SettingKeys.ExcludedAnnouncementIds, []);
            DateTimeOffset minTime = DateTimeOffset.UtcNow - TimeSpan.FromDays(AnnouncementDuration);

            foreach ((string key, object value) in excludedIds)
            {
                if (value is DateTimeOffset time && time < minTime)
                {
                    excludedIds.Remove(key);
                }
            }

            excludedIds.TryAdd($"{announcement.Id}", DateTimeOffset.UtcNow + TimeSpan.FromDays(AnnouncementDuration));
            LocalSetting.Set(SettingKeys.ExcludedAnnouncementIds, excludedIds);

            announcements.Remove(announcement);
        }
    }
}