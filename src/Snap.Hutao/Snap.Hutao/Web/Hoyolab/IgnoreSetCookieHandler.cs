// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab;

/// <summary>
/// 忽略 Set-Cookie 头
/// </summary>
[Injection(InjectAs.Transient)]
internal class IgnoreSetCookieHandler : DelegatingHandler
{
    /// <inheritdoc/>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage message = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        message.Headers.Remove("Set-Cookie");
        return message;
    }
}