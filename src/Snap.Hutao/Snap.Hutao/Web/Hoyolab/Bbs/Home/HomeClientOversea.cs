// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Web.Hoyolab.Bbs.Home;

[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.XRpc3)]
internal sealed partial class HomeClientOversea : IHomeClient
{
    // TODO: Implement IHomeClient.GetNewHomeInfoAsync
    public ValueTask<Response<NewHomeNewInfo>> GetNewHomeInfoAsync(int gid, CancellationToken token = default)
    {
        return ValueTask.FromException<Response<NewHomeNewInfo>>(HutaoException.NotSupported());
    }
}