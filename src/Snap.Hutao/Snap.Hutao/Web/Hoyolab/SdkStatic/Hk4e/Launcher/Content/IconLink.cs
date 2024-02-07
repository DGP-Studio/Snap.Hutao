// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher.Resource;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.IO;
using System.Net.Http;
using System.Text;

namespace Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher;

internal sealed class IconLink
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = default!;

    [JsonPropertyName("url")]
    public string Url { get; set; } = default!;
}