// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Diagnostics;
using System.Net.Http;
using System.Text;

namespace Snap.Hutao.Web.Request.Builder;

internal static class HttpContentBuilderExtension
{
    [DebuggerStepThrough]
    public static T SetFormUrlEncodedContent<T>(this T builder, params (string Key, string Value)[] content)
        where T : IHttpContentBuilder
    {
        return builder.SetFormUrlEncodedContent((IEnumerable<(string, string)>)content);
    }

    [DebuggerStepThrough]
    public static T SetFormUrlEncodedContent<T>(this T builder, IEnumerable<(string Key, string Value)> content)
        where T : IHttpContentBuilder
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(content);
        return builder.SetFormUrlEncodedContent(content.Select(x => new KeyValuePair<string, string>(x.Key, x.Value)));
    }

    [DebuggerStepThrough]
    public static T SetFormUrlEncodedContent<T>(this T builder, params KeyValuePair<string, string>[] content)
        where T : IHttpContentBuilder
    {
        return builder.SetFormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>)content);
    }

    [DebuggerStepThrough]
    public static T SetFormUrlEncodedContent<T>(
        this T builder, IEnumerable<KeyValuePair<string, string>> content)
        where T : IHttpContentBuilder
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(content);
        return builder.SetContent(new FormUrlEncodedContent(content));
    }

    [DebuggerStepThrough]
    public static T SetStringContent<T>(this T builder, string content, Encoding? encoding = null, string mediaType = default!)
        where T : IHttpContentBuilder
    {
        ArgumentNullException.ThrowIfNull(content);
        return builder.SetContent(new StringContent(content, encoding, mediaType));
    }

    [DebuggerStepThrough]
    public static T SetByteArrayContent<T>(this T builder, byte[] content)
        where T : IHttpContentBuilder
    {
        return builder.SetByteArrayContent(content, 0, content?.Length ?? 0);
    }

    [DebuggerStepThrough]
    public static T SetByteArrayContent<T>(this T builder, byte[] content, int offset, int count)
        where T : IHttpContentBuilder
    {
        ArgumentNullException.ThrowIfNull(content);
        return builder.SetContent(new ByteArrayContent(content, offset, count));
    }

    public static TBuilder SetContent<TBuilder, TContent>(this TBuilder builder, IHttpContentSerializer serializer, TContent content, Encoding? encoding = null)
        where TBuilder : IHttpContentBuilder
    {
        // Validate builder here already so that no unnecessary serialization is done.
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(serializer);

        // Don't call the SetContent(object, Type) overload on purpose.
        // We have the extension method for a generic serialize, so we should use it.
        // If the behavior ever changes, only the extension method will have to be updated.
        HttpContent? httpContent = serializer.Serialize(content, encoding);
        return builder.SetContent(httpContent);
    }

    public static T SetContent<T>(this T builder, IHttpContentSerializer serializer, object? content, Type contentType, Encoding? encoding = null)
        where T : IHttpContentBuilder
    {
        // Validate builder here already so that no unnecessary serialization is done.
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(serializer);

        HttpContent? httpContent = serializer.Serialize(content, contentType, encoding);
        return builder.SetContent(httpContent);
    }

    [DebuggerStepThrough]
    public static T SetContent<T>(this T builder, HttpContent? content)
        where T : IHttpContentBuilder
    {
        return builder.Configure(builder => builder.Content = content);
    }
}