// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using AngleSharp;
using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Core;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;
using WebAnnouncement = Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement.Announcement;

namespace Snap.Hutao.Service.Announcement;

[ConstructorGenerated]
[Service(ServiceLifetime.Scoped, typeof(IAnnouncementService))]
internal sealed partial class AnnouncementService : IAnnouncementService
{
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly ILogger<AnnouncementService> logger;
    private readonly ITaskContext taskContext;
    private readonly IMemoryCache memoryCache;

    [GeneratedRegex(@"(\d\.\d)")]
    private static partial Regex VersionRegex { get; }

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

    private static DateTimeOffset ParseTime(IReadOnlyDictionary<string, DateTimeOffset> versionStartTimes, TimeSpan offset, string text)
    {
        if (VersionRegex.Match(text) is { Success: true } version)
        {
            if (versionStartTimes.TryGetValue(version.Groups[1].Value, out DateTimeOffset versionStartTime))
            {
                return versionStartTime;
            }
        }

        return UnsafeDateTimeOffset.ParseDateTime(text, offset);
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
        wrapper.List = wrapper.List.Reverse();

        PreprocessAnnouncements(contentMap, wrapper.List);
        try
        {
            await AdjustAnnouncementTimeAsync(wrapper.List, TimeSpan.FromHours(wrapper.TimeZone)).ConfigureAwait(false);
        }
        catch
        {
            // ignored
        }

        return wrapper;
    }

    private async ValueTask AdjustAnnouncementTimeAsync(ImmutableArray<AnnouncementListWrapper> announcementListWrappers, TimeSpan offset)
    {
        // 活动公告
        ImmutableArray<WebAnnouncement> activities = announcementListWrappers
            .Single(wrapper => wrapper.TypeId is 1)
            .List;

        // 游戏公告
        ImmutableArray<WebAnnouncement> announcements = announcementListWrappers
            .Single(wrapper => wrapper.TypeId is 2)
            .List;

        // "x.x" -> DTO
        Dictionary<string, DateTimeOffset> versionStartTimes = [];

        // Workaround for some long-term activities
        // Why does the project team extend the fucking end time for the fucking activity?
        versionStartTimes.TryAdd("5.0", new(2024, 8, 28, 6, 0, 0, TimeSpan.FromHours(8)));
        versionStartTimes.TryAdd("5.5", new(2025, 3, 26, 6, 0, 0, TimeSpan.FromHours(8)));

        IBrowsingContext context = BrowsingContext.New(Configuration.Default);

        // 更新公告
        if (announcements.SingleOrDefault(ann => AnnouncementRegex.VersionUpdateTitleRegex.IsMatch(ann.Title)) is { } versionUpdate)
        {
            string time = await AnnouncementHtmlVisitor.VisitAnnouncementAsync(context, versionUpdate.Content).ConfigureAwait(false);
            DateTimeOffset versionUpdateTime = UnsafeDateTimeOffset.ParseDateTime(time, offset);
            versionStartTimes.TryAdd(VersionRegex.Match(versionUpdate.Title).Groups[1].Value, versionUpdateTime);
        }

        // 更新预告
        if (announcements.SingleOrDefault(ann => AnnouncementRegex.VersionUpdatePreviewTitleRegex.IsMatch(ann.Title)) is { } versionUpdatePreview)
        {
            string time = await AnnouncementHtmlVisitor.VisitUpdatePreviewAsync(context, versionUpdatePreview.Content).ConfigureAwait(false);
            DateTimeOffset versionUpdatePreviewTime = UnsafeDateTimeOffset.ParseDateTime(time, offset);
            versionStartTimes.TryAdd(VersionRegex.Match(versionUpdatePreview.Title).Groups[1].Value, versionUpdatePreviewTime);
        }

        foreach (WebAnnouncement announcement in activities)
        {
            ImmutableArray<string> times = await AnnouncementHtmlVisitor.VisitActivityAsync(context, announcement.Content).ConfigureAwait(false);
            logger.LogInformation("{Title} '{Time}'", announcement.Subtitle, string.Join(",", times));

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (times is [])
            {
                continue;
            }

            if (times is [{ } time] && time.Contains(SH.ServiceAnnouncementPermanentKeyword, StringComparison.CurrentCulture))
            {
                announcement.StartTime = ParseTime(versionStartTimes, offset, time);
                continue;
            }

            ImmutableArray<DateTimeOffset> timeOffsets = [.. times.Select(text => ParseTime(versionStartTimes, offset, text)).Order()];
            DateTimeOffset startTime = timeOffsets.First();
            DateTimeOffset endTime = timeOffsets.Last();

            if (startTime == endTime)
            {
                endTime += TimeSpan.FromDays(42);
            }

            announcement.StartTime = startTime;
            announcement.EndTime = endTime;
        }
    }
}