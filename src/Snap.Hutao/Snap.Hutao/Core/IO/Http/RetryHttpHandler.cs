// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Web.Request.Builder;
using System.Net.Http;
using System.Runtime.ExceptionServices;

namespace Snap.Hutao.Core.IO.Http;

[Service(ServiceLifetime.Transient)]
internal sealed partial class RetryHttpHandler : DelegatingHandler
{
    public static HttpRequestOptionsKey<bool> DisableRetry { get; } = new("DisableRetry");

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Options.TryGetValue(DisableRetry, out bool skipRetry) && skipRetry)
        {
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        ExceptionDispatchInfo? dispatch = default;
        int requestCount = 0;
        while (requestCount < 3)
        {
            try
            {
                return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                dispatch = ExceptionDispatchInfo.Capture(ex);
                request.Resurrect();
            }

            requestCount++;
        }

        dispatch?.Throw();
        throw HutaoException.InvalidOperation("Unexpected request retry state");
    }
}