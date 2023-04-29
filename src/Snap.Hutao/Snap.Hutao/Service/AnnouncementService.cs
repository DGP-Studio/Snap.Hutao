// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;
using Snap.Hutao.Web.Response;
using System.Text.RegularExpressions;

namespace Snap.Hutao.Service;

/// <inheritdoc/>
[HighQuality]
[Injection(InjectAs.Transient, typeof(IAnnouncementService))]
internal sealed partial class AnnouncementService : IAnnouncementService
{
    private static readonly string CacheKey = $"{nameof(AnnouncementService)}.Cache.{nameof(AnnouncementWrapper)}";

    private readonly ITaskContext taskContext;
    private readonly AnnouncementClient announcementClient;
    private readonly IMemoryCache memoryCache;

    /// <summary>
    /// 构造一个新的公告服务
    /// </summary>
    /// <param name="taskContext">任务上下文</param>
    /// <param name="announcementClient">公告提供器</param>
    /// <param name="memoryCache">缓存</param>
    public AnnouncementService(ITaskContext taskContext, AnnouncementClient announcementClient, IMemoryCache memoryCache)
    {
        this.taskContext = taskContext;
        this.announcementClient = announcementClient;
        this.memoryCache = memoryCache;
    }

    /// <inheritdoc/>
    public async ValueTask<AnnouncementWrapper> GetAnnouncementsAsync(CancellationToken cancellationToken = default)
    {
        // 缓存中存在记录，直接返回
        if (memoryCache.TryGetValue(CacheKey, out object? cache))
        {
            return (AnnouncementWrapper)cache!;
        }

        await taskContext.SwitchToBackgroundAsync();
        Response<AnnouncementWrapper> announcementWrapperResponse = await announcementClient
            .GetAnnouncementsAsync(cancellationToken)
            .ConfigureAwait(false);

        if (announcementWrapperResponse.IsOk())
        {
            AnnouncementWrapper wrapper = announcementWrapperResponse.Data;
            Response<ListWrapper<AnnouncementContent>> announcementContentResponse = await announcementClient
                .GetAnnouncementContentsAsync(cancellationToken)
                .ConfigureAwait(false);

            if (announcementContentResponse.IsOk())
            {
                List<AnnouncementContent> contents = announcementContentResponse.Data.List;

                Dictionary<int, string> contentMap = contents
                    .ToDictionary(id => id.AnnId, content => content.Content);

                // 将活动公告置于前方
                wrapper.List.Reverse();

                // 将公告内容联入公告列表
                JoinAnnouncements(contentMap, wrapper.List);

                return memoryCache.Set(CacheKey, wrapper, TimeSpan.FromMinutes(30));
            }
        }

        return null!;
    }

    private static void JoinAnnouncements(Dictionary<int, string> contentMap, List<AnnouncementListWrapper> announcementListWrappers)
    {
        Regex timeTagRegrex = XmlTagRegex();

        announcementListWrappers.ForEach(listWrapper =>
        {
            listWrapper.List.ForEach(item =>
            {
                if (contentMap.TryGetValue(item.AnnId, out string? rawContent))
                {
                    // remove <t/> tag
                    rawContent = timeTagRegrex.Replace(rawContent!, x => x.Groups[1].Value);
                }

                item.Content = rawContent ?? string.Empty;
            });
        });
    }

    /// <summary>
    /// 匹配特殊的时间格式: <t>(.*?)</t>
    /// </summary>
    /// <returns>正则</returns>
    [GeneratedRegex("&lt;t.*?&gt;(.*?)&lt;/t&gt;", RegexOptions.Multiline)]
    private static partial Regex XmlTagRegex();
}