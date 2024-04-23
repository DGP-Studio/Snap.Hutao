// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Core;
using Snap.Hutao.Service.Announcement;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;
using Snap.Hutao.Web.Response;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using WebAnnouncement = Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement.Announcement;

namespace Snap.Hutao.Service;

/// <inheritdoc/>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped, typeof(IAnnouncementService))]
internal sealed partial class AnnouncementService : IAnnouncementService
{
    private static readonly string CacheKey = $"{nameof(AnnouncementService)}.Cache.{nameof(AnnouncementWrapper)}";

    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly ITaskContext taskContext;
    private readonly IMemoryCache memoryCache;

    public async ValueTask<AnnouncementWrapper> GetAnnouncementWrapperAsync(string languageCode, Region region, CancellationToken cancellationToken = default)
    {
        // 缓存中存在记录，直接返回
        if (memoryCache.TryGetRequiredValue($"{CacheKey}.{languageCode}.{region}", out AnnouncementWrapper? cache))
        {
            return cache;
        }

        await taskContext.SwitchToBackgroundAsync();

        List<AnnouncementContent>? contents;
        AnnouncementWrapper wrapper;
        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            AnnouncementClient announcementClient = scope.ServiceProvider.GetRequiredService<AnnouncementClient>();

            Response<AnnouncementWrapper> announcementWrapperResponse = await announcementClient
                .GetAnnouncementsAsync(languageCode, region, cancellationToken)
                .ConfigureAwait(false);

            if (!announcementWrapperResponse.IsOk())
            {
                return default!;
            }

            wrapper = announcementWrapperResponse.Data;

            Response<ListWrapper<AnnouncementContent>> announcementContentResponse = await announcementClient
                .GetAnnouncementContentsAsync(languageCode, region, cancellationToken)
                .ConfigureAwait(false);

            if (!announcementContentResponse.IsOk())
            {
                return default!;
            }

            contents = announcementContentResponse.Data.List;
        }

        Dictionary<int, string> contentMap = contents.ToDictionary(id => id.AnnId, content => content.Content);

        // 将活动公告置于前方
        wrapper.List.Reverse();

        PreprocessAnnouncements(contentMap, wrapper.List, new(wrapper.TimeZone, 0, 0));

        return memoryCache.Set(CacheKey, wrapper, TimeSpan.FromMinutes(30));
    }

    private static void PreprocessAnnouncements(Dictionary<int, string> contentMap, List<AnnouncementListWrapper> announcementListWrappers, in TimeSpan offset)
    {
        // 将公告内容联入公告列表
        foreach (ref readonly AnnouncementListWrapper listWrapper in CollectionsMarshal.AsSpan(announcementListWrappers))
        {
            foreach (ref readonly WebAnnouncement item in CollectionsMarshal.AsSpan(listWrapper.List))
            {
                item.Content = contentMap.GetValueOrDefault(item.AnnId, string.Empty);
            }
        }

        AdjustAnnouncementTime(announcementListWrappers, offset);

        foreach (ref readonly AnnouncementListWrapper listWrapper in CollectionsMarshal.AsSpan(announcementListWrappers))
        {
            foreach (ref readonly WebAnnouncement item in CollectionsMarshal.AsSpan(listWrapper.List))
            {
                item.Subtitle = new StringBuilder(item.Subtitle)
                    .Replace("\r<br>", string.Empty)
                    .Replace("<br />", string.Empty)
                    .ToString();
                item.Content = AnnouncementRegex
                    .XmlTimeTagRegex()
                    .Replace(item.Content, x => x.Groups[1].Value);
            }
        }
    }

    private static void AdjustAnnouncementTime(List<AnnouncementListWrapper> announcementListWrappers, in TimeSpan offset)
    {
        // 活动公告
        List<WebAnnouncement> activities = announcementListWrappers
            .Single(wrapper => wrapper.TypeId == 1)
            .List;

        Dictionary<string, DateTimeOffset> versionStartTimeDict = new Dictionary<string, DateTimeOffset>();

        // x.x版本更新说明
        try
        {
            WebAnnouncement versionUpdate = announcementListWrappers
                .Single(wrapper => wrapper.TypeId == 2)
                .List
                .Single(ann => AnnouncementRegex.VersionUpdateTitleRegex.IsMatch(ann.Title));

            if (AnnouncementRegex.VersionUpdateTimeRegex.Match(versionUpdate.Content) is not { Success: true } versionUpdateMatch)
            {
                return;
            }

            DateTimeOffset versionUpdateTime = UnsafeDateTimeOffset.ParseDateTime(versionUpdateMatch.Groups[1].ValueSpan, offset);
            versionStartTimeDict.Add(new Regex("(\\d\\.\\d)").Match(versionUpdate.Title).Groups[1].Value, versionUpdateTime);
        }
        catch (Exception)
        {
        }

        // x.x版本更新维护预告
        try
        {
            WebAnnouncement versionUpdatePreview = announcementListWrappers
                .Single(wrapper => wrapper.TypeId == 2)
                .List
                .Single(ann => AnnouncementRegex.VersionUpdatePreviewTitleRegex.IsMatch(ann.Title));

            if (AnnouncementRegex.VersionUpdatePreviewTimeRegex.Match(versionUpdatePreview.Content) is not { Success: true } versionUpdatePreviewMatch)
            {
                return;
            }

            DateTimeOffset versionUpdatePreviewTime = UnsafeDateTimeOffset.ParseDateTime(versionUpdatePreviewMatch.Groups[1].ValueSpan, offset);
            versionStartTimeDict.TryAdd(new Regex("(\\d\\.\\d)").Match(versionUpdatePreview.Title).Groups[1].Value, versionUpdatePreviewTime);
        }
        catch (Exception)
        {
        }

        foreach (ref readonly WebAnnouncement announcement in CollectionsMarshal.AsSpan(activities))
        {
            DateTimeOffset versionStartTime;

            if (AnnouncementRegex.PermanentActivityAfterUpdateTimeRegex.Match(announcement.Content) is { Success: true } permanent)
            {
                if (versionStartTimeDict.TryGetValue(permanent.Groups[1].Value, out versionStartTime))
                {
                    announcement.StartTime = versionStartTime;
                    continue;
                }
            }

            if (AnnouncementRegex.PersistentActivityAfterUpdateTimeRegex.Match(announcement.Content) is { Success: true } persistent)
            {
                if (versionStartTimeDict.TryGetValue(persistent.Groups[1].Value, out versionStartTime))
                {
                    announcement.StartTime = versionStartTime;
                    announcement.EndTime = versionStartTime + TimeSpan.FromDays(42);
                    continue;
                }
            }

            if (AnnouncementRegex.TransientActivityAfterUpdateTimeRegex.Match(announcement.Content) is { Success: true } transient)
            {
                if (versionStartTimeDict.TryGetValue(transient.Groups[1].Value, out versionStartTime))
                {
                    announcement.StartTime = versionStartTime;
                    announcement.EndTime = UnsafeDateTimeOffset.ParseDateTime(transient.Groups[2].ValueSpan, offset);
                    continue;
                }
            }

            MatchCollection matches = AnnouncementRegex.XmlTimeTagRegex().Matches(announcement.Content);
            if (matches.Count < 2)
            {
                continue;
            }

            List<DateTimeOffset> dateTimes = [];
            foreach (Match timeMatch in (IList<Match>)matches)
            {
                dateTimes.Add(UnsafeDateTimeOffset.ParseDateTime(timeMatch.Groups[1].ValueSpan, offset));
            }

            DateTimeOffset min = DateTimeOffset.MaxValue;
            DateTimeOffset max = DateTimeOffset.MinValue;

            foreach (DateTimeOffset time in dateTimes)
            {
                if (time < min)
                {
                    min = time;
                }

                if (time > max)
                {
                    max = time;
                }
            }

            announcement.StartTime = min;
            announcement.EndTime = max;
        }
    }
}