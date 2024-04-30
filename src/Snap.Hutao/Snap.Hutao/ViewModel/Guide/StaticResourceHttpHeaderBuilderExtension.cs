// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Setting;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Net.Http.Headers;

namespace Snap.Hutao.ViewModel.Guide;

internal static class StaticResourceHttpHeaderBuilderExtension
{
    public static TBuilder SetStaticResourceControlHeaders<TBuilder>(this TBuilder builder)
        where TBuilder : IHttpHeadersBuilder<HttpHeaders>
    {
        return builder
            .SetHeader("x-quality", $"{UnsafeLocalSetting.Get(SettingKeys.StaticResourceImageQuality, StaticResourceQuality.Raw)}")
            .SetHeader("x-archive", $"{UnsafeLocalSetting.Get(SettingKeys.StaticResourceImageArchive, StaticResourceArchive.Full)}");
    }

    public static TBuilder SetStaticResourceControlHeadersIf<TBuilder>(this TBuilder builder, bool condition)
        where TBuilder : IHttpHeadersBuilder<HttpHeaders>
    {
        return condition ? builder.SetStaticResourceControlHeaders() : builder;
    }
}