// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Snap.Hutao.Web.Request.Builder;

internal sealed class HttpRequestMessageBuilder :
    IBuilder,
    IHttpRequestMessageBuilder,
    IHttpHeadersBuilder<HttpHeaders>,
    IHttpHeadersBuilder<HttpRequestHeaders>,
    IHttpHeadersBuilder<HttpContentHeaders>,
    IHttpContentBuilder,
    IHttpContentHeadersBuilder,
    IHttpRequestOptionsBuilder,
    IHttpProtocolVersionBuilder,
    IRequestUriBuilder,
    IHttpMethodBuilder
{
    private readonly HttpContentSerializer httpContentSerializer;
    private readonly IServiceProvider serviceProvider;
    private HttpRequestMessage httpRequestMessage;

    public HttpRequestMessageBuilder(IServiceProvider serviceProvider, HttpContentSerializer httpContentSerializer, HttpRequestMessage? httpRequestMessage = default)
    {
        this.serviceProvider = serviceProvider;
        this.httpContentSerializer = httpContentSerializer;
        this.httpRequestMessage = httpRequestMessage ?? new();
    }

    public HttpRequestMessage HttpRequestMessage
    {
        get => httpRequestMessage;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            httpRequestMessage = value;
        }
    }

    public IServiceProvider ServiceProvider { get => serviceProvider; }

    public HttpContentSerializer HttpContentSerializer { get => httpContentSerializer; }

    HttpContentHeaders IHttpHeadersBuilder<HttpContentHeaders>.Headers
    {
        get
        {
            ArgumentNullException.ThrowIfNull(Content);
            return Content.Headers;
        }
    }

    HttpHeaders IHttpHeadersBuilder<HttpHeaders>.Headers
    {
        get => Headers;
    }

    public HttpRequestHeaders Headers
    {
        get => HttpRequestMessage.Headers;
    }

    public HttpRequestOptions Options
    {
        get => HttpRequestMessage.Options;
    }

    public HttpContent? Content
    {
        get => HttpRequestMessage.Content;
        set => HttpRequestMessage.Content = value;
    }

    public Version Version
    {
        get => HttpRequestMessage.Version;
        set => HttpRequestMessage.Version = value;
    }

    public Uri? RequestUri
    {
        get => HttpRequestMessage.RequestUri;
        set => HttpRequestMessage.RequestUri = value;
    }

    public HttpMethod Method
    {
        get => HttpRequestMessage.Method;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            HttpRequestMessage.Method = value;
        }
    }

    public override string ToString()
    {
        return HttpRequestMessage.ToString();
    }
}