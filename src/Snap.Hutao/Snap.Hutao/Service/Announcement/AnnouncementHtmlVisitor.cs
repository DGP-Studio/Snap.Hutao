// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System.Text;
using System.Text.RegularExpressions;

namespace Snap.Hutao.Service;

internal static partial class AnnouncementHtmlVisitor
{
    public static async ValueTask<string> VisitActivityAsync(IBrowsingContext context, string content)
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

            if (paragraph.TextContent is not ("〓活动时间〓" or "〓祈愿介绍〓" or "〓任务开放时间〓" or "〓折扣时间〓" or "〓重置时间〓"))
            {
                continue;
            }

            if (paragraph.NextElementSibling is IHtmlParagraphElement { Children: [IHtmlSpanElement, ..] } nextParagraph)
            {
                return nextParagraph.TextContent;
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
                            return timeBuilder.ToString();
                        }

                        if (cell.TextContent is "祈愿时间")
                        {
                            return table.Rows[1].Cells[actualIndex].TextContent;
                        }
                    }
                }
            }
        }

        return string.Empty;
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

            if (paragraph.NextElementSibling is IHtmlParagraphElement { Children: [IHtmlSpanElement, ..] } nextParagraph)
            {
                return TimeRegex().Match(nextParagraph.TextContent).Value;
            }
        }

        return string.Empty;
    }

    [GeneratedRegex(@"\d{4}/\d{2}/\d{2} \d{2}:\d{2}:\d{2}")]
    private static partial Regex TimeRegex();
}