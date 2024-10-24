// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using AngleSharp;
using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Core;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;
using Snap.Hutao.Web.Response;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using WebAnnouncement = Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement.Announcement;

namespace Snap.Hutao.Service.Announcement;

[ConstructorGenerated]
[Injection(InjectAs.Scoped, typeof(IAnnouncementService))]
internal sealed partial class AnnouncementService : IAnnouncementService
{
    private const string CacheKey = $"{nameof(AnnouncementService)}.Cache.{nameof(AnnouncementWrapper)}";

    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly ILogger<AnnouncementService> logger;
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
        AnnouncementWrapper? wrapper;
        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            AnnouncementClient announcementClient = scope.ServiceProvider.GetRequiredService<AnnouncementClient>();

            Response<AnnouncementWrapper> announcementWrapperResponse = await announcementClient
                .GetAnnouncementsAsync(languageCode, region, cancellationToken)
                .ConfigureAwait(false);

            if (!ResponseValidator.TryValidate(announcementWrapperResponse, scope.ServiceProvider, out wrapper))
            {
                return default!;
            }

            Response<ListWrapper<AnnouncementContent>> announcementContentResponse = await announcementClient
                .GetAnnouncementContentsAsync(languageCode, region, cancellationToken)
                .ConfigureAwait(false);

            if (!ResponseValidator.TryValidate(announcementContentResponse, scope.ServiceProvider, out ListWrapper<AnnouncementContent>? contentsWrapper))
            {
                return default!;
            }

            contents = contentsWrapper.List;
        }

        Dictionary<int, string> contentMap = contents.ToDictionary(id => id.AnnId, content => content.Content);

        // 将活动公告置于前方
        wrapper.List.Reverse();

        PreprocessAnnouncements(contentMap, wrapper.List);
        try
        {
            await AdjustAnnouncementTimeAsync(wrapper.List, new(wrapper.TimeZone, 0, 0)).ConfigureAwait(false);
        }
        catch
        {
        }

        return memoryCache.Set(CacheKey, wrapper, TimeSpan.FromMinutes(30));
    }

    private static void PreprocessAnnouncements(Dictionary<int, string> contentMap, List<AnnouncementListWrapper> announcementListWrappers)
    {
        // 将公告内容联入公告列表
        foreach (ref readonly AnnouncementListWrapper listWrapper in CollectionsMarshal.AsSpan(announcementListWrappers))
        {
            foreach (ref readonly WebAnnouncement item in CollectionsMarshal.AsSpan(listWrapper.List))
            {
                item.Content = contentMap.GetValueOrDefault(item.AnnId, string.Empty);
            }
        }

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

    [GeneratedRegex("(\\d\\.\\d)")]
    private static partial Regex VersionRegex();

    private async ValueTask AdjustAnnouncementTimeAsync(List<AnnouncementListWrapper> announcementListWrappers, TimeSpan offset)
    {
        // 活动公告
        List<WebAnnouncement> activities = announcementListWrappers
            .Single(wrapper => wrapper.TypeId == 1)
            .List;

        // 游戏公告
        List<WebAnnouncement> announcements = announcementListWrappers
            .Single(wrapper => wrapper.TypeId == 2)
            .List;

        // "x.x" -> DTO
        Dictionary<string, DateTimeOffset> versionStartTimes = [];

        // Workaround for 5.0 Permanent Activity
        versionStartTimes.TryAdd("5.0", UnsafeDateTimeOffset.ParseDateTime("2024/08/28 06:00".AsSpan(), offset));

        IBrowsingContext context = BrowsingContext.New(Configuration.Default);

        // 更新公告
        if (announcements.SingleOrDefault(ann => AnnouncementRegex.VersionUpdateTitleRegex.IsMatch(ann.Title)) is { } versionUpdate)
        {
            string time = await AnnouncementHtmlVisitor.VisitAnnouncementAsync(context, versionUpdate.Content).ConfigureAwait(false);
            DateTimeOffset versionUpdateTime = UnsafeDateTimeOffset.ParseDateTime(time, offset);
            versionStartTimes.TryAdd(VersionRegex().Match(versionUpdate.Title).Groups[1].Value, versionUpdateTime);
        }

        // 更新预告
        if (announcements.SingleOrDefault(ann => AnnouncementRegex.VersionUpdatePreviewTitleRegex.IsMatch(ann.Title)) is { } versionUpdatePreview)
        {
            if (AnnouncementRegex.VersionUpdatePreviewTimeRegex.Match(versionUpdatePreview.Content) is not { Success: true } versionUpdatePreviewMatch)
            {
                return;
            }

            DateTimeOffset versionUpdatePreviewTime = UnsafeDateTimeOffset.ParseDateTime(versionUpdatePreviewMatch.Groups[1].ValueSpan, offset);
            versionStartTimes.TryAdd(VersionRegex().Match(versionUpdatePreview.Title).Groups[1].Value, versionUpdatePreviewTime);
        }

        foreach (WebAnnouncement announcement in activities)
        {
            (AnnouncementType type, string text) = await AnnouncementHtmlVisitor.VisitActivityAsync(context, announcement.Content).ConfigureAwait(false);
            logger.LogInformation("{Title} '{Time}'", announcement.Subtitle, text);

            DateTimeOffset versionStartTime;

            switch (type)
            {
                case AnnouncementType.Permanent:
                    if (VersionRegex().Match(text) is { Success: true } permanent)
                    {
                        if (versionStartTimes.TryGetValue(permanent.Groups[1].Value, out versionStartTime))
                        {
                            announcement.StartTime = versionStartTime;
                            continue;
                        }
                    }

                    text = text.Replace("后永久开放", string.Empty, StringComparison.InvariantCulture);
                    announcement.StartTime = UnsafeDateTimeOffset.ParseDateTime(text, offset);
                    continue;
                case AnnouncementType.Persistent:
                    if (VersionRegex().Match(text) is { Success: true } persistent)
                    {
                        if (versionStartTimes.TryGetValue(persistent.Groups[1].Value, out versionStartTime))
                        {
                            announcement.StartTime = versionStartTime;
                            announcement.EndTime = versionStartTime + TimeSpan.FromDays(42);
                        }
                    }

                    continue;
                case AnnouncementType.Transient:
                    string[] transientParts = text.Split("~");
                    if (VersionRegex().Match(transientParts[0]) is { Success: true } transient)
                    {
                        if (versionStartTimes.TryGetValue(transient.Groups[1].Value, out versionStartTime))
                        {
                            announcement.StartTime = versionStartTime;
                            announcement.EndTime = UnsafeDateTimeOffset.ParseDateTime(transientParts[1], offset);
                        }
                    }

                    continue;
            }

            string[] parts = text.Split("~");
            if (parts.Length is not 2)
            {
                continue;
            }

            announcement.StartTime = UnsafeDateTimeOffset.ParseDateTime(parts[0], offset);
            announcement.EndTime = UnsafeDateTimeOffset.ParseDateTime(parts[1], offset);
        }
    }
}