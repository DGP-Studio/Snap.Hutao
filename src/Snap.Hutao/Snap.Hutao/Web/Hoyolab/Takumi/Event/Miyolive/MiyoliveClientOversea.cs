// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Miyolive;

[ConstructorGenerated]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class MiyoliveClientOversea : IMiyoliveClient
{
    public ValueTask<Response<CodeListWrapper>> RefreshCodeAsync(string actId, CancellationToken token = default)
    {
        return ValueTask.FromException<Response<CodeListWrapper>>(HutaoException.NotSupported());
    }
}