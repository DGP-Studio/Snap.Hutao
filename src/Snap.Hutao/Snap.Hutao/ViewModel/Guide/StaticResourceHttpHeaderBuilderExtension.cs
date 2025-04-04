// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Web.Endpoint.Hutao;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Net.Http.Headers;

namespace Snap.Hutao.ViewModel.Guide;

internal static class StaticResourceHttpHeaderBuilderExtension
{
    public static TBuilder SetStaticResourceControlHeaders<TBuilder>(this TBuilder builder)
        where TBuilder : class, IHttpHeadersBuilder<HttpHeaders>
    {
        return builder
            .SetHeader("x-hutao-quality", $"{UnsafeLocalSetting.Get(SettingKeys.StaticResourceImageQuality, StaticResourceQuality.Raw)}")
            .SetHeader("x-hutao-archive", $"{UnsafeLocalSetting.Get(SettingKeys.StaticResourceImageArchive, StaticResourceArchive.Full)}");
    }

    public static TBuilder SetStaticResourceControlHeadersIf<TBuilder>(this TBuilder builder, bool condition)
        where TBuilder : class, IHttpHeadersBuilder<HttpHeaders>
    {
        return condition ? builder.SetStaticResourceControlHeaders() : builder;
    }

    public static TBuilder SetStaticResourceControlHeadersIfRequired<TBuilder>(this TBuilder builder)
        where TBuilder : class, IHttpHeadersBuilder<HttpHeaders>, IRequestUriBuilder
    {
        return builder.RequestUri?.GetLeftPart(UriPartial.Authority).Equals(StaticResourcesEndpoints.Root, StringComparison.OrdinalIgnoreCase) is true
            ? builder.SetStaticResourceControlHeaders()
            : builder;
    }

    public static async ValueTask SetStaticResourceAuthorizationHeaderIfRequired(this HttpRequestMessageBuilder builder, HutaoUserOptions hutaoUserOptions)
    {
        if (await hutaoUserOptions.GetImageTokenAsync().ConfigureAwait(false) is { } token)
        {
            builder.Headers.Authorization = new("Bearer", token);
        }
    }
}