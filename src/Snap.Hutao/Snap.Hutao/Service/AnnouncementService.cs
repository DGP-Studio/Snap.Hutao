// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace Snap.Hutao.Service;

/// <inheritdoc/>
[Injection(InjectAs.Transient, typeof(IAnnouncementService))]
internal class AnnouncementService : IAnnouncementService
{
    private readonly AnnouncementClient announcementClient;

    /// <summary>
    /// 构造一个新的公告服务
    /// </summary>
    /// <param name="announcementClient">公告提供器</param>
    public AnnouncementService(AnnouncementClient announcementClient)
    {
        this.announcementClient = announcementClient;
    }

    /// <inheritdoc/>
    public async Task<AnnouncementWrapper> GetAnnouncementsAsync(ICommand openAnnouncementUICommand, CancellationToken cancellationToken = default)
    {
        AnnouncementWrapper? wrapper = await announcementClient
            .GetAnnouncementsAsync(cancellationToken)
            .ConfigureAwait(false);
        List<AnnouncementContent> contents = await announcementClient
            .GetAnnouncementContentsAsync(cancellationToken)
            .ConfigureAwait(false);

        Dictionary<int, string> contentMap = contents
            .ToDictionary(id => id.AnnId, content => content.Content);

        Must.NotNull(wrapper!);

        // 将活动公告置于上方
        wrapper.List.Reverse();

        // 将公告内容联入公告列表
        JoinAnnouncements(openAnnouncementUICommand, contentMap, wrapper.List);

        return wrapper;
    }

    private static void JoinAnnouncements(ICommand openAnnouncementUICommand, Dictionary<int, string> contentMap, List<AnnouncementListWrapper> announcementListWrappers)
    {
        // 匹配特殊的时间格式: <t>(.*?)</t>
        Regex timeTagRegrex = new("&lt;t.*?&gt;(.*?)&lt;/t&gt;", RegexOptions.Multiline);

        Regex timeTagInnerRegex = new("(?<=&lt;t.*?&gt;)(.*?)(?=&lt;/t&gt;)");

        announcementListWrappers.ForEach(listWrapper =>
        {
            listWrapper.List?.ForEach(item =>
            {
                if (contentMap.TryGetValue(item.AnnId, out string? rawContent))
                {
                    // remove <t/> tag
                    rawContent = timeTagRegrex.Replace(rawContent!, x => timeTagInnerRegex.Match(x.Value).Value);
                }

                item.Content = rawContent ?? string.Empty;
                item.OpenAnnouncementUICommand = openAnnouncementUICommand;
            });
        });
    }
}