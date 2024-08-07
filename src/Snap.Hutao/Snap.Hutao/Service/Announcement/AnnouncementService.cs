// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
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
    private const string CacheKey = $"{nameof(AnnouncementService)}.Cache.{nameof(AnnouncementWrapper)}";

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

        PreprocessAnnouncements(contentMap, wrapper.List);
        await AdjustAnnouncementTimeAsync(wrapper.List, new(wrapper.TimeZone, 0, 0)).ConfigureAwait(false);

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

    private static async ValueTask AdjustAnnouncementTimeAsync(List<AnnouncementListWrapper> announcementListWrappers, TimeSpan offset)
    {
        // 活动公告
        List<WebAnnouncement> activities = announcementListWrappers
            .Single(wrapper => wrapper.TypeId == 1)
            .List;

        // 游戏公告
        List<WebAnnouncement> announcements = announcementListWrappers
            .Single(wrapper => wrapper.TypeId == 2)
            .List;

        Dictionary<string, DateTimeOffset> versionStartTimes = [];

        //// 更新公告
        //if (announcements.SingleOrDefault(ann => AnnouncementRegex.VersionUpdateTitleRegex.IsMatch(ann.Title)) is { } versionUpdate)
        //{
        //    if (AnnouncementRegex.VersionUpdateTimeRegex.Match(versionUpdate.Content) is not { Success: true } versionUpdateMatch)
        //    {
        //        return;
        //    }

        //    DateTimeOffset versionUpdateTime = UnsafeDateTimeOffset.ParseDateTime(versionUpdateMatch.Groups[1].ValueSpan, offset);
        //    versionStartTimes.TryAdd(VersionRegex().Match(versionUpdate.Title).Groups[1].Value, versionUpdateTime);
        //}

        //// 更新预告
        //if (announcements.SingleOrDefault(ann => AnnouncementRegex.VersionUpdatePreviewTitleRegex.IsMatch(ann.Title)) is { } versionUpdatePreview)
        //{
        //    if (AnnouncementRegex.VersionUpdatePreviewTimeRegex.Match(versionUpdatePreview.Content) is not { Success: true } versionUpdatePreviewMatch)
        //    {
        //        return;
        //    }

        //    DateTimeOffset versionUpdatePreviewTime = UnsafeDateTimeOffset.ParseDateTime(versionUpdatePreviewMatch.Groups[1].ValueSpan, offset);
        //    versionStartTimes.TryAdd(VersionRegex().Match(versionUpdatePreview.Title).Groups[1].Value, versionUpdatePreviewTime);
        //}

        IBrowsingContext context = BrowsingContext.New(Configuration.Default);

        foreach (WebAnnouncement announcement in activities)
        {
            IDocument document = await context.OpenAsync(rsp => rsp.Content(announcement.Content)).ConfigureAwait(false);
            IHtmlParser? parser = context.GetService<IHtmlParser>();
            IHtmlElement? body = document.Body;
            ArgumentNullException.ThrowIfNull(body);
            string text = body.TextContent;
            _ = 1;
            foreach (IElement element in body.Children)
            {
                if (element is IHtmlParagraphElement paragraph)
                {
                    foreach (IElement element2 in paragraph.Children)
                    {
                        if (element2 is IHtmlSpanElement span)
                        {
                            if (span.TextContent is "〓活动时间〓")
                            {
                                ArgumentNullException.ThrowIfNull(paragraph.NextElementSibling);
                                foreach (IElement element3 in paragraph.NextElementSibling.Children)
                                {
                                    if (element3 is IHtmlSpanElement span2)
                                    {
                                        _ = span2.TextContent;
                                    }
                                }
                            }
                            else if (span.TextContent is "〓祈愿介绍〓")
                            {
                                ArgumentNullException.ThrowIfNull(paragraph.NextElementSibling);
                                if (paragraph.NextElementSibling is IHtmlDivElement div)
                                {
                                    if (div.Children.Single() is IHtmlTableElement table)
                                    {
                                        foreach (IHtmlTableRowElement tableRow in table.Rows)
                                        {
                                            foreach (IHtmlTableCellElement cell in tableRow.Cells)
                                            {

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            DateTimeOffset versionStartTime;

            if (AnnouncementRegex.PermanentActivityAfterUpdateTimeRegex.Match(announcement.Content) is { Success: true } permanent)
            {
                if (versionStartTimes.TryGetValue(permanent.Groups[1].Value, out versionStartTime))
                {
                    announcement.StartTime = versionStartTime;
                    continue;
                }

                announcement.StartTime = UnsafeDateTimeOffset.ParseDateTime(permanent.Groups[2].ValueSpan, offset);
            }

            if (AnnouncementRegex.PersistentActivityAfterUpdateTimeRegex.Match(announcement.Content) is { Success: true } persistent)
            {
                if (versionStartTimes.TryGetValue(persistent.Groups[1].Value, out versionStartTime))
                {
                    announcement.StartTime = versionStartTime;
                    announcement.EndTime = versionStartTime + TimeSpan.FromDays(42);
                    continue;
                }

                if (versionStartTimes.TryGetValue(persistent.Groups[2].Value, out versionStartTime))
                {
                    announcement.StartTime = versionStartTime;
                    announcement.EndTime = versionStartTime + TimeSpan.FromDays(42);
                    continue;
                }
            }

            if (AnnouncementRegex.TransientActivityAfterUpdateTimeRegex.Match(announcement.Content) is { Success: true } transient)
            {
                if (versionStartTimes.TryGetValue(transient.Groups[1].Value, out versionStartTime))
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

    [GeneratedRegex("(\\d\\.\\d)")]
    private static partial Regex VersionRegex();
}