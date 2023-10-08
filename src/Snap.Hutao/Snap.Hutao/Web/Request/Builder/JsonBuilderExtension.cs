// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Text;

namespace Snap.Hutao.Web.Request.Builder;

internal static partial class JsonBuilderExtension
{
    public static TBuilder SetJsonContent<TBuilder, TContent>(this TBuilder builder, TContent content, Encoding? encoding = null, JsonHttpContentSerializer? serializer = null)
        where TBuilder : IHttpContentBuilder
    {
        return builder.SetContent(serializer ?? builder.HttpContentSerializer, content, encoding);
    }

    public static T SetJsonContent<T>(this T builder, object? content, Type contentType, Encoding? encoding = null, JsonHttpContentSerializer? serializer = null)
        where T : IHttpContentBuilder
    {
        return builder.SetContent(serializer ?? builder.HttpContentSerializer, content, contentType, encoding);
    }

    public static TBuilder PostJson<TBuilder, TContent>(this TBuilder builder, TContent content, Encoding? encoding = null, JsonHttpContentSerializer? serializer = null)
        where TBuilder : IHttpMethodBuilder, IHttpContentBuilder
    {
        return builder.Post().SetJsonContent(content, encoding, serializer);
    }

    public static T PostJson<T>(this T builder, object? content, Type contentType, Encoding? encoding = null, JsonHttpContentSerializer? serializer = null)
        where T : IHttpMethodBuilder, IHttpContentBuilder
    {
        return builder.Post().SetJsonContent(content, contentType, encoding, serializer);
    }

    public static TBuilder PutJson<TBuilder, TContent>(this TBuilder builder, TContent content, Encoding? encoding = null, JsonHttpContentSerializer? serializer = null)
        where TBuilder : IHttpMethodBuilder, IHttpContentBuilder
    {
        return builder.Put().SetJsonContent(content, encoding, serializer);
    }

    public static T PutJson<T>(this T builder, object? content, Type contentType, Encoding? encoding = null, JsonHttpContentSerializer? serializer = null)
        where T : IHttpMethodBuilder, IHttpContentBuilder
    {
        return builder.Put().SetJsonContent(content, contentType, encoding, serializer);
    }

    public static TBuilder PatchJson<TBuilder, TContent>(this TBuilder builder, TContent content, Encoding? encoding = null, JsonHttpContentSerializer? serializer = null)
        where TBuilder : IHttpMethodBuilder, IHttpContentBuilder
    {
        return builder.Patch().SetJsonContent(content, encoding, serializer);
    }

    public static T PatchJson<T>(this T builder, object? content, Type contentType, Encoding? encoding = null, JsonHttpContentSerializer? serializer = null)
        where T : IHttpMethodBuilder, IHttpContentBuilder
    {
        return builder.Patch().SetJsonContent(content, contentType, encoding, serializer);
    }
}