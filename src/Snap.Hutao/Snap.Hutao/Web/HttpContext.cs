// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Net.Http;
using System.Runtime.ExceptionServices;

namespace Snap.Hutao.Web;

internal sealed class HttpContext : IDisposable
{
    public required HttpClient HttpClient { get; init; }

    public required ILogger Logger { get; init; }

    public required CancellationToken RequestAborted { get; init; }

    public HttpRequestMessage? Request { get; set; }

    public HttpResponseMessage? Response { get; set; }

    public ExceptionDispatchInfo? Exception { get; set; }

    public void Dispose()
    {
        Request?.Dispose();
        Response?.Dispose();
    }
}