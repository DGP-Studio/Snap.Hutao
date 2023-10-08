// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Diagnostics;
using System.Net.Http;

namespace Snap.Hutao.Web.Request.Builder;

internal static class HttpMethodBuilderExtension
{
    [DebuggerStepThrough]
    public static T Get<T>(this T builder)
        where T : IHttpMethodBuilder
    {
        return builder.SetMethod(HttpMethod.Get);
    }

    [DebuggerStepThrough]
    public static T Post<T>(this T builder)
        where T : IHttpMethodBuilder
    {
        return builder.SetMethod(HttpMethod.Post);
    }

    [DebuggerStepThrough]
    public static T Put<T>(this T builder)
        where T : IHttpMethodBuilder
    {
        return builder.SetMethod(HttpMethod.Put);
    }

    [DebuggerStepThrough]
    public static T Delete<T>(this T builder)
        where T : IHttpMethodBuilder
    {
        return builder.SetMethod(HttpMethod.Delete);
    }

    [DebuggerStepThrough]
    public static T Options<T>(this T builder)
        where T : IHttpMethodBuilder
    {
        return builder.SetMethod(HttpMethod.Options);
    }

    [DebuggerStepThrough]
    public static T Trace<T>(this T builder)
        where T : IHttpMethodBuilder
    {
        return builder.SetMethod(HttpMethod.Trace);
    }

    [DebuggerStepThrough]
    public static T Head<T>(this T builder)
        where T : IHttpMethodBuilder
    {
        return builder.SetMethod(HttpMethod.Head);
    }

    [DebuggerStepThrough]
    public static T Patch<T>(this T builder)
        where T : IHttpMethodBuilder
    {
        return builder.SetMethod(HttpMethod.Patch);
    }

    [DebuggerStepThrough]
    public static T SetMethod<T>(this T builder, string method)
        where T : IHttpMethodBuilder
    {
        ArgumentNullException.ThrowIfNull(method);
        return builder.SetMethod(new HttpMethod(method));
    }

    /// <summary>
    ///     Sets the <see cref="HttpMethod"/> which is being built.
    /// </summary>
    /// <typeparam name="T">The type of the builder.</typeparam>
    /// <param name="builder">The builder.</param>
    /// <param name="method">
    ///     The HTTP method used by the HTTP request message.
    /// </param>
    /// <returns>The specified <paramref name="builder"/>.</returns>
    /// <exception cref="ArgumentNullException">
    ///     * <paramref name="builder"/>
    ///     * <paramref name="method"/>
    /// </exception>
    [DebuggerStepThrough]
    public static T SetMethod<T>(this T builder, HttpMethod method)
        where T : IHttpMethodBuilder
    {
        ArgumentNullException.ThrowIfNull(method);
        return builder.Configure(builder => builder.Method = method);
    }
}