// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Diagnostics;

namespace Snap.Hutao.Web.Request.Builder;

internal static class RequestUriBuilderExtension
{
    [DebuggerStepThrough]
    public static T SetRequestUri<T>(this T builder, string? requestUri, UriKind uriKind = UriKind.RelativeOrAbsolute)
        where T : IRequestUriBuilder
    {
        return builder.SetRequestUri(requestUri is null ? null : new Uri(requestUri, uriKind));
    }

    [DebuggerStepThrough]
    public static T SetRequestUri<T>(this T builder, Uri? requestUri)
        where T : IRequestUriBuilder
    {
        return builder.Configure(builder => builder.RequestUri = requestUri);
    }
}