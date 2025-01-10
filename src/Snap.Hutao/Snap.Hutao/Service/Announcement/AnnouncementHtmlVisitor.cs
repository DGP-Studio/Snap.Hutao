// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Snap.Hutao.Service.Announcement;

internal static partial class AnnouncementHtmlVisitor
{
    [GeneratedRegex(@".*?\d\.\d.*?[~-]|.*?\d\.\d.*?$|\d{4}/\d{2}/\d{2} \d{2}:\d{2}(?::\d{2})?")]
    private static partial Regex TimeOrVersionRegex { get; }

    public static async ValueTask<ImmutableArray<string>> VisitActivityAsync(IBrowsingContext context, string content)
    {
        IDocument document = await context.OpenAsync(rsp => rsp.Content(content)).ConfigureAwait(false);
        IHtmlElement? body = document.Body;
        ArgumentNullException.ThrowIfNull(body);

        return body.Children
            .Where(e => e is IHtmlParagraphElement && AnnouncementRegex.ValidDescriptionsRegex.IsMatch(e.TextContent))
            .OfType<IHtmlParagraphElement>()
            .Select(ParseElementToTimeStrings)
            .MaxBy(r => r.Length)
            .EmptyIfDefault();

        ImmutableArray<string> ParseElementToTimeStrings(IHtmlParagraphElement paragraph)
        {
            string textContent = paragraph.TextContent.Trim();

            // All in span, special case
            if (textContent.Contains(SH.ServiceAnnouncementAdventurersBoosterBundlesDurationDescription, StringComparison.CurrentCulture))
            {
                return [.. TimeOrVersionRegex.Matches(textContent).Select(r => r.Value)];
            }

            if (paragraph.NextElementSibling is null)
            {
                return [];
            }

            string nextTextContent = paragraph.NextElementSibling.TextContent.Trim();
            return [.. TimeOrVersionRegex.Matches(nextTextContent).Select(r => r.Value)];
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

#pragma warning disable CA1309
            if (!paragraph.TextContent.Equals(SH.ServiceAnnouncementVersionUpdateTimeDescription, StringComparison.CurrentCulture))
#pragma warning restore CA1309
            {
                continue;
            }

            if (paragraph.NextElementSibling is IHtmlParagraphElement nextParagraph)
            {
                return TimeOrVersionRegex.Match(nextParagraph.TextContent).Value;
            }
        }

        return string.Empty;
    }

    public static async ValueTask<string> VisitUpdatePreviewAsync(IBrowsingContext context, string content)
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

            if (TimeOrVersionRegex.Match(paragraph.TextContent) is { Success: true } match)
            {
                return match.Value;
            }
        }

        return string.Empty;
    }
}