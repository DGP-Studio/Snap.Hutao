// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Bbs.Home;

[HttpClient(HttpClientConfiguration.XRpc3)]
internal sealed partial class HomeClientOversea : IHomeClient
{
    [GeneratedConstructor]
    public partial HomeClientOversea(IServiceProvider serviceProvider, HttpClient httpClient);

    public ValueTask<Response<NewHomeNewInfo>> GetNewHomeInfoAsync(int gid, CancellationToken token = default)
    {
        return ValueTask.FromException<Response<NewHomeNewInfo>>(HutaoException.NotSupported());
    }
}