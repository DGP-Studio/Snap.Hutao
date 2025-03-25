// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Web.Request.Builder;
using System.Net.Http;
using System.Runtime.ExceptionServices;

namespace Snap.Hutao.Core.IO.Http;

[Injection(InjectAs.Transient)]
internal sealed partial class RetryHttpHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ExceptionDispatchInfo? exception = default;
        int requestCount = 0;
        while (requestCount < 3)
        {
            try
            {
                return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                exception = ExceptionDispatchInfo.Capture(ex);
                request.Resurrect();
            }

            requestCount++;
        }

        exception?.Throw();
        throw HutaoException.InvalidOperation("Unexpected Request retry state");
    }
}