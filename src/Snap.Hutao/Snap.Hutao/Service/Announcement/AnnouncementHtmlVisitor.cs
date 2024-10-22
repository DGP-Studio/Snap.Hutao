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

    private static readonly FrozenSet<string> ValidDescriptions = FrozenSet.ToFrozenSet(
    [
        "〓活动时间〓",
        "〓任务开放时间〓",
        "〓祈愿介绍〓",
        "〓折扣时间〓",
        "〓获取奖励时限〓",
        "【上架时间】",
    ]);

    public static async ValueTask<List<string>> VisitActivityAsync(IBrowsingContext context, string content)
    {
        IDocument document = await context.OpenAsync(rsp => rsp.Content(content)).ConfigureAwait(false);
        IHtmlElement? body = document.Body;
        ArgumentNullException.ThrowIfNull(body);

        return body.Children
            .Where(e => e is IHtmlParagraphElement)
            .Where(e => ValidDescriptions.Any(d => e.TextContent.Contains(d, StringComparison.InvariantCulture)))
            .Select(e => ParseElementToTimeStrings((IHtmlParagraphElement)e))
            .MaxBy(r => r.Count) ?? [];

        List<string> ParseElementToTimeStrings(IHtmlParagraphElement paragraph)
        {
            string textContent = paragraph.TextContent.Trim();

            // All in span, special case
            if (textContent.Contains("【上架时间】", StringComparison.CurrentCulture))
            {
                string timeRange = textContent.Replace("【上架时间】", string.Empty, StringComparison.InvariantCulture).Trim();
                return timeRange.Split("~").ToList();
            }

            if (paragraph.NextElementSibling is null)
            {
                return [];
            }

            string nextTextContent = paragraph.NextElementSibling.TextContent.Trim();

            return TimeOrVersionRegex().Matches(nextTextContent).Select(r => r.Value).ToList();
        }
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
                return TimeOrVersionRegex().Match(nextParagraph.TextContent).Value;
            }
        }

        return string.Empty;
    }

    [GeneratedRegex(@"\d\.\d版本\S*|\d{4}/\d{2}/\d{2} \d{2}:\d{2}(?::\d{2})?")]
    private static partial Regex TimeOrVersionRegex();
}