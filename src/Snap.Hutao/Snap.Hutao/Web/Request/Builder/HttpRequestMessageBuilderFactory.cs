// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Request.Builder.Abstraction;

namespace Snap.Hutao.Web.Request.Builder;

[Service(ServiceLifetime.Singleton, typeof(IHttpRequestMessageBuilderFactory))]
[GeneratedConstructor]
internal sealed partial class HttpRequestMessageBuilderFactory : IHttpRequestMessageBuilderFactory
{
    private readonly JsonHttpContentSerializer jsonHttpContentSerializer;
    private readonly IServiceProvider serviceProvider;

    public HttpRequestMessageBuilder Create()
    {
        return new(serviceProvider, jsonHttpContentSerializer);
    }
}