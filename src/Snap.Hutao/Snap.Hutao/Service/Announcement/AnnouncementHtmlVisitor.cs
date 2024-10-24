// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System.Collections.Frozen;
using System.Text;
using System.Text.RegularExpressions;

namespace Snap.Hutao.Service.Announcement;

internal static partial class AnnouncementHtmlVisitor
{
    private const string PermanentKeyword = "后永久开放";
    private const string PersistentKeyword = "版本期间持续开放";
    private const string TransientKeyword = "版本更新后";

    private static readonly FrozenSet<string> PermanentDescriptions = FrozenSet.ToFrozenSet(
    [
        "〓活动时间〓",
        "〓任务开放时间〓",
    ]);

    private static readonly FrozenSet<string> PersistentDescriptions = FrozenSet.ToFrozenSet(
    [
        "〓活动时间〓",
    ]);

    private static readonly FrozenSet<string> TransientDescriptions = FrozenSet.ToFrozenSet(
    [
        "〓活动时间〓",
        "〓祈愿介绍〓",
        "〓折扣时间〓",
        "〓获取奖励时限〓",
    ]);

    private static readonly FrozenSet<string> ValidDescriptions = FrozenSet.ToFrozenSet(
    [
        "〓活动时间〓",
        "〓任务开放时间〓",
        "〓祈愿介绍〓",
        "〓折扣时间〓",
        "〓获取奖励时限〓",
    ]);

    public static async ValueTask<ValueResult<AnnouncementType, string>> VisitActivityAsync(IBrowsingContext context, string content)
    {
        IDocument document = await context.OpenAsync(rsp => rsp.Content(content)).ConfigureAwait(false);
        IHtmlElement? body = document.Body;
        ArgumentNullException.ThrowIfNull(body);

        AnnouncementType type = AnnouncementType.None;

        foreach (IElement element in body.Children)
        {
            if (element is not IHtmlParagraphElement paragraph)
            {
                continue;
            }

            string textContent = paragraph.TextContent.Trim();
            if (textContent.Contains("【上架时间】", StringComparison.CurrentCulture))
            {
                return new(AnnouncementType.Transient, textContent.Replace("【上架时间】", string.Empty, StringComparison.InvariantCulture).Trim());
            }

            if (PermanentDescriptions.Contains(textContent) && content.Contains(PermanentKeyword, StringComparison.InvariantCulture))
            {
                type = AnnouncementType.Permanent;
            }
            else if (PersistentDescriptions.Contains(textContent) && content.Contains(PersistentKeyword, StringComparison.InvariantCulture))
            {
                type = AnnouncementType.Persistent;
            }
            else if (TransientDescriptions.Contains(textContent) && content.Contains(TransientKeyword, StringComparison.InvariantCulture))
            {
                type = AnnouncementType.Transient;
            }
            else
            {
                if (!ValidDescriptions.Contains(textContent))
                {
                    continue;
                }

                ArgumentNullException.ThrowIfNull(paragraph.NextElementSibling);
                if (TimeRegex().Matches(paragraph.NextElementSibling.TextContent).Count < 2)
                {
                    continue;
                }
            }

            if (paragraph.NextElementSibling is IHtmlParagraphElement { /*Children: [IHtmlSpanElement, ..]*/ } nextParagraph)
            {
                return new(type, nextParagraph.TextContent);
            }

            if (paragraph.NextElementSibling is IHtmlDivElement div)
            {
                foreach (IElement element2 in div.Children)
                {
                    if (element2 is not IHtmlTableElement table)
                    {
                        continue;
                    }

                    IHtmlTableRowElement header = table.Rows[0];
                    StringBuilder timeBuilder = new();
                    int actualIndex = -1;
                    foreach (IHtmlTableCellElement cell in header.Cells)
                    {
                        actualIndex += cell.ColumnSpan;
                        if (cell.TextContent is "开启时间")
                        {
                            timeBuilder.Append(table.Rows[1].Cells[actualIndex].TextContent).Append(" ~ ");
                        }

                        if (cell.TextContent is "结束时间")
                        {
                            timeBuilder.Append(table.Rows[1].Cells[actualIndex].TextContent);
                            return new(type, timeBuilder.ToString());
                        }

                        if (cell.TextContent is "祈愿时间")
                        {
                            return new(type, table.Rows[1].Cells[actualIndex].TextContent);
                        }
                    }
                }
            }
        }

        return new(type, string.Empty);
    }

    public static async ValueTask<string> VisitAnnouncementAsync(IBrowsingContext context, string content)
    {
        IDocument document = await context.OpenAsync(rsp => rsp.Content(content)).ConfigureAwait(false);
        IHtmlElement? body = document.Body;
        ArgumentNullException.ThrowIfNull(body);

        foreach (IElement element in body.Children)
        {
            if (element is not IHtmlParagraphElement paragraph)
            {
                continue;
            }

            if (paragraph.TextContent is not "〓更新时间〓")
            {
                continue;
            }

            if (paragraph.NextElementSibling is IHtmlParagraphElement { /*Children: [IHtmlSpanElement, ..]*/ } nextParagraph)
            {
                return TimeRegex().Match(nextParagraph.TextContent).Value;
            }
        }

        return string.Empty;
    }

    [GeneratedRegex(@"\d{4}/\d{2}/\d{2} \d{2}:\d{2}(?::\d{2})?")]
    private static partial Regex TimeRegex();
}