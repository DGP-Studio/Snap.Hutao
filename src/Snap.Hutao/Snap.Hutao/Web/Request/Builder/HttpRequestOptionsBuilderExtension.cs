// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Diagnostics;
using System.Net.Http;

namespace Snap.Hutao.Web.Request.Builder;

internal static class HttpRequestOptionsBuilderExtension
{
    [DebuggerStepThrough]
    public static TBuilder SetOptions<TBuilder, TValue>(this TBuilder builder, HttpRequestOptionsKey<TValue> key, TValue value)
        where TBuilder : class, IHttpRequestOptionsBuilder
    {
        return builder.Configure(builder => builder.Options.Set(key, value));
    }
}