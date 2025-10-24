// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;
using System.Text;
using WebAnnouncement = Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement.Announcement;

namespace Snap.Hutao.Service.Announcement;

[ConstructorGenerated]
[Service(ServiceLifetime.Scoped, typeof(IAnnouncementService))]
internal sealed partial class AnnouncementService : IAnnouncementService
{
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly ITaskContext taskContext;
    private readonly IMemoryCache memoryCache;

    [SuppressMessage("", "SH003")]
    public async ValueTask<AnnouncementWrapper?> GetAnnouncementWrapperAsync(string languageCode, Region region, CancellationToken token = default)
    {
        AnnouncementWrapper? wrapper = await memoryCache.GetOrCreateAsync($"{nameof(AnnouncementService)}.{nameof(AnnouncementWrapper)}.{languageCode}.{region}", entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(30L));
            return PrivateGetAnnouncementWrapperAsync(languageCode, region, token);
        }).ConfigureAwait(false);

        return wrapper;
    }

    private static void PreprocessAnnouncements(Dictionary<int, string> contentMap, ImmutableArray<AnnouncementListWrapper> announcementListWrappers)
    {
        // 将公告内容联入公告列表
        foreach (ref readonly AnnouncementListWrapper listWrapper in announcementListWrappers.AsSpan())
        {
            foreach (ref readonly WebAnnouncement item in listWrapper.List.AsSpan())
            {
                item.Content = contentMap.GetValueOrDefault(item.AnnId, string.Empty);
            }
        }

        foreach (ref readonly AnnouncementListWrapper listWrapper in announcementListWrappers.AsSpan())
        {
            foreach (ref readonly WebAnnouncement item in listWrapper.List.AsSpan())
            {
                item.Subtitle = new StringBuilder(item.Subtitle)
                    .Replace("\r<br>", string.Empty)
                    .Replace("<br />", string.Empty)
                    .ToString();
                item.Content = AnnouncementRegex
                    .XmlTimeTagRegex
                    .Replace(item.Content, x => x.Groups[1].Value);
            }
        }
    }

    [SuppressMessage("", "SH003")]
    private async Task<AnnouncementWrapper?> PrivateGetAnnouncementWrapperAsync(string languageCode, Region region, CancellationToken cancellationToken = default)
    {
        await taskContext.SwitchToBackgroundAsync();

        ImmutableArray<AnnouncementContent> contents;
        AnnouncementWrapper? wrapper;
        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            AnnouncementClient announcementClient = scope.ServiceProvider.GetRequiredService<AnnouncementClient>();

            Response<AnnouncementWrapper> announcementWrapperResponse = await announcementClient
                .GetAnnouncementsAsync(languageCode, region, cancellationToken)
                .ConfigureAwait(false);

            if (!ResponseValidator.TryValidate(announcementWrapperResponse, scope.ServiceProvider, out wrapper))
            {
                return default;
            }

            Response<ListWrapper<AnnouncementContent>> announcementContentResponse = await announcementClient
                .GetAnnouncementContentsAsync(languageCode, region, cancellationToken)
                .ConfigureAwait(false);

            if (!ResponseValidator.TryValidate(announcementContentResponse, scope.ServiceProvider, out ListWrapper<AnnouncementContent>? contentsWrapper))
            {
                return default;
            }

            contents = contentsWrapper.List;
        }

        Dictionary<int, string> contentMap = contents.ToDictionaryIgnoringDuplicateKeys(id => id.AnnId, content => content.Content);

        // 将活动公告置于前方
        // wrapper.List = wrapper.List.Reverse();

        PreprocessAnnouncements(contentMap, wrapper.List);
        return wrapper;
    }
}