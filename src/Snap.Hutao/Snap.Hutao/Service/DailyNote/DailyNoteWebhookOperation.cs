// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Net.Http;
using WebDailyNote = Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote.DailyNote;

namespace Snap.Hutao.Service.DailyNote;

[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class DailyNoteWebhookOperation
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILogger<DailyNoteWebhookOperation> logger;
    private readonly DailyNoteOptions dailyNoteOptions;
    private readonly HttpClient httpClient;

    public void TryPostDailyNoteToWebhook(PlayerUid playerUid, WebDailyNote dailyNote)
    {
        string? targetUrl = dailyNoteOptions.WebhookUrl;
        if (string.IsNullOrEmpty(targetUrl) || !Uri.TryCreate(targetUrl, UriKind.Absolute, out Uri? targetUri))
        {
            return;
        }

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(targetUri)
            .SetHeader("x-uid", $"{playerUid}")
            .PostJson(dailyNote);

        builder.Send(httpClient, logger);
    }
}