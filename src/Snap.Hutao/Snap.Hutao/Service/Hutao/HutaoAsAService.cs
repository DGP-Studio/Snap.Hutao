// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Web.Hutao.HutaoAsAService;
using Snap.Hutao.Web.Response;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using Windows.Storage;
using HutaoAnnouncement = Snap.Hutao.Web.Hutao.HutaoAsAService.Announcement;

namespace Snap.Hutao.Service.Hutao;

[ConstructorGenerated]
[Injection(InjectAs.Scoped, typeof(IHutaoAsAService))]
internal sealed partial class HutaoAsAService : IHutaoAsAService
{
    private const int AnnouncementDuration = 30;
    private readonly HutaoAsAServiceClient hutaoAsServiceClient;
    private readonly RuntimeOptions runtimeOptions;

    private ObservableCollection<HutaoAnnouncement>? announcements;

    public async ValueTask<ObservableCollection<HutaoAnnouncement>> GetHutaoAnnouncementCollectionAsync(CancellationToken token = default)
    {
        if (announcements is null)
        {
            RelayCommand<HutaoAnnouncement> dismissCommand = new(DismissAnnouncement);

            ApplicationDataCompositeValue excludedIds = LocalSetting.Get(SettingKeys.ExcludedAnnouncementIds, new ApplicationDataCompositeValue());
            List<long> data = excludedIds.Select(kvp => long.Parse(kvp.Key, CultureInfo.InvariantCulture)).ToList();
            Response<List<HutaoAnnouncement>> response = await hutaoAsServiceClient.GetAnnouncementListAsync(data, token).ConfigureAwait(false);

            if (response.IsOk())
            {
                List<HutaoAnnouncement> list = response.Data;
                List<HutaoAnnouncement> removeList = new();

                foreach (HutaoAnnouncement item in list)
                {
                    string versionInAnnouncement = VersionRegex().Match(item.Title).Value;
                    if (versionInAnnouncement.Length != 0 && runtimeOptions.Version >= new Version(versionInAnnouncement))
                    {
                        DismissAnnouncement(item);
                        removeList.Add(item);
                        continue;
                    }

                    item.DismissCommand = dismissCommand;
                }

                announcements = list.Except(removeList).ToObservableCollection();
            }
            else
            {
                return new();
            }
        }

        return announcements;
    }

    private void DismissAnnouncement(HutaoAnnouncement? announcement)
    {
        if (announcement is not null && announcements is not null)
        {
            ApplicationDataCompositeValue excludedIds = LocalSetting.Get(SettingKeys.ExcludedAnnouncementIds, new ApplicationDataCompositeValue());

            foreach ((string key, object value) in excludedIds)
            {
                if (value is DateTimeOffset time && time < DateTimeOffset.Now - TimeSpan.FromDays(AnnouncementDuration))
                {
                    excludedIds.Remove(key);
                }
            }

            excludedIds.TryAdd($"{announcement.Id}", DateTimeOffset.Now + TimeSpan.FromDays(AnnouncementDuration));
            LocalSetting.Set(SettingKeys.ExcludedAnnouncementIds, excludedIds);

            announcements.Remove(announcement);
        }
    }

    [GeneratedRegex("(\\d+)\\.(\\d+)\\.(\\d+)")]
    private partial Regex VersionRegex();
}