// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;
using Snap.Hutao.Web.Response;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Snap.Hutao.Service;

/// <inheritdoc/>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IAnnouncementService))]
internal sealed partial class AnnouncementService : IAnnouncementService
{
    private static readonly string CacheKey = $"{nameof(AnnouncementService)}.Cache.{nameof(AnnouncementWrapper)}";

    private readonly AnnouncementClient announcementClient;
    private readonly ITaskContext taskContext;
    private readonly IMemoryCache memoryCache;

    /// <inheritdoc/>
    public async ValueTask<AnnouncementWrapper> GetAnnouncementWrapperAsync(string languageCode, Region region, CancellationToken cancellationToken = default)
    {
        // 缓存中存在记录，直接返回
        if (memoryCache.TryGetRequiredValue($"{CacheKey}.{languageCode}.{region}", out AnnouncementWrapper? cache))
        {
            return cache;
        }

        await taskContext.SwitchToBackgroundAsync();
        Response<AnnouncementWrapper> announcementWrapperResponse = await announcementClient
            .GetAnnouncementsAsync(languageCode, region, cancellationToken)
            .ConfigureAwait(false);

        if (!announcementWrapperResponse.IsOk())
        {
            return default!;
        }

        AnnouncementWrapper wrapper = announcementWrapperResponse.Data;
        Response<ListWrapper<AnnouncementContent>> announcementContentResponse = await announcementClient
            .GetAnnouncementContentsAsync(languageCode, region, cancellationToken)
            .ConfigureAwait(false);

        if (!announcementContentResponse.IsOk())
        {
            return default!;
        }

        List<AnnouncementContent> contents = announcementContentResponse.Data.List;

        Dictionary<int, string> contentMap = contents
            .ToDictionary(id => id.AnnId, content => content.Content);

        // 将活动公告置于前方
        wrapper.List.Reverse();

        PreprocessAnnouncements(contentMap, wrapper.List);

        return memoryCache.Set(CacheKey, wrapper, TimeSpan.FromMinutes(30));
    }

    private static void PreprocessAnnouncements(Dictionary<int, string> contentMap, List<AnnouncementListWrapper> announcementListWrappers)
    {
        // 将公告内容联入公告列表
        foreach (ref readonly AnnouncementListWrapper listWrapper in CollectionsMarshal.AsSpan(announcementListWrappers))
        {
            foreach (ref readonly Announcement item in CollectionsMarshal.AsSpan(listWrapper.List))
            {
                contentMap.TryGetValue(item.AnnId, out string? rawContent);
                item.Content = rawContent ?? string.Empty;
            }
        }

        AdjustAnnouncementTime(announcementListWrappers);

        foreach (ref readonly AnnouncementListWrapper listWrapper in CollectionsMarshal.AsSpan(announcementListWrappers))
        {
            foreach (ref readonly Announcement item in CollectionsMarshal.AsSpan(listWrapper.List))
            {
                item.Subtitle = new StringBuilder(item.Subtitle).Replace("\r<br>", string.Empty).ToString();
                item.Content = AnnouncementRegex.XmlTimeTagRegex.Replace(item.Content, x => x.Groups[1].Value);
            }
        }
    }

    private static void AdjustAnnouncementTime(List<AnnouncementListWrapper> announcementListWrappers)
    {
        // 活动公告
        List<Announcement> activities = announcementListWrappers
            .Single(wrapper => wrapper.TypeId == 1)
            .List;

        // 更新公告
        Announcement versionUpdate = announcementListWrappers
            .Single(wrapper => wrapper.TypeId == 2)
            .List
            .Single(ann => AnnouncementRegex.VersionUpdateTitleRegex.IsMatch(ann.Title));

        if (AnnouncementRegex.VersionUpdateTimeRegex.Match(versionUpdate.Content) is not { Success: true } match)
        {
            return;
        }

        DateTimeOffset versionUpdateTime = DateTimeOffset.Parse(match.Groups[1].ValueSpan, CultureInfo.InvariantCulture);

        foreach (ref readonly Announcement announcement in CollectionsMarshal.AsSpan(activities))
        {
            if (AnnouncementRegex.PermanentActivityAfterUpdateTimeRegex.Match(announcement.Content) is { Success: true } permanent)
            {
                announcement.StartTime = versionUpdateTime;
                continue;
            }

            if (AnnouncementRegex.PersistentActivityAfterUpdateTimeRegex.Match(announcement.Content) is { Success: true } persistent)
            {
                announcement.StartTime = versionUpdateTime;
                announcement.EndTime = versionUpdateTime + TimeSpan.FromDays(42);
                continue;
            }

            if (AnnouncementRegex.TransientActivityAfterUpdateTimeRegex.Match(announcement.Content) is { Success: true } transient)
            {
                announcement.StartTime = versionUpdateTime;
                announcement.EndTime = DateTimeOffset.Parse(transient.Groups[2].ValueSpan, CultureInfo.InvariantCulture);
                continue;
            }

            MatchCollection matches = AnnouncementRegex.XmlTimeTagRegex.Matches(announcement.Content);
            if (matches.Count < 2)
            {
                continue;
            }

            List<DateTimeOffset> dateTimes = matches.Select(match => DateTimeOffset.Parse(match.Groups[1].ValueSpan, CultureInfo.InvariantCulture)).ToList();
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