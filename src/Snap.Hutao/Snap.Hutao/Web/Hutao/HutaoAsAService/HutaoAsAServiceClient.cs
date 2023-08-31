// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hutao.HutaoAsAService;

/// <summary>
/// HaaS Client
/// </summary>
[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class HutaoAsAServiceClient
{
    private readonly MetadataOptions metadataOptions;
    private readonly HutaoUserOptions hutaoUserOptions;
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<HutaoAsAServiceClient> logger;

    public async ValueTask<Response<List<Announcement>>> GetAnnouncementListAsync(List<long> excluedeIds, CancellationToken token = default)
    {
        Response<List<Announcement>>? resp = await httpClient
            .TryCatchPostAsJsonAsync<List<long>, Response<List<Announcement>>>(HutaoEndpoints.Announcement(metadataOptions.LocaleName), excluedeIds, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response.Response> UploadAnnouncementAsync(UploadAnnouncement uploadAnnouncement, CancellationToken token = default)
    {
        await httpClient.TrySetTokenAsync(hutaoUserOptions).ConfigureAwait(false);

        Response.Response? resp = await httpClient
            .TryCatchPostAsJsonAsync<UploadAnnouncement, Response.Response>(HutaoEndpoints.AnnouncementUpload, uploadAnnouncement, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response.Response> GachaLogCompensationAsync(int days, CancellationToken token = default)
    {
        await httpClient.TrySetTokenAsync(hutaoUserOptions).ConfigureAwait(false);

        Response.Response? resp = await httpClient
            .TryCatchGetFromJsonAsync<Response.Response>(HutaoEndpoints.GachaLogCompensation(days), options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response.Response> GachaLogDesignationAsync(string userName, int days, CancellationToken token = default)
    {
        await httpClient.TrySetTokenAsync(hutaoUserOptions).ConfigureAwait(false);

        Response.Response? resp = await httpClient
            .TryCatchGetFromJsonAsync<Response.Response>(HutaoEndpoints.GachaLogDesignation(userName, days), options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}