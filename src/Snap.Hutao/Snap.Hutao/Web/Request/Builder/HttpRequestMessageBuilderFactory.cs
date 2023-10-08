// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Request.Builder.Abstraction;

namespace Snap.Hutao.Web.Request.Builder;

[Injection(InjectAs.Singleton, typeof(IHttpRequestMessageBuilderFactory))]
[ConstructorGenerated]
internal sealed partial class HttpRequestMessageBuilderFactory : IHttpRequestMessageBuilderFactory
{
    private readonly JsonHttpContentSerializer jsonHttpContentSerializer;

    public HttpRequestMessageBuilder Create()
    {
        return new(jsonHttpContentSerializer);
    }
}