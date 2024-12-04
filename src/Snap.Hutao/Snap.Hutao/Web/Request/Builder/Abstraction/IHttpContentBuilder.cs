// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using System.Net.Http;

namespace Snap.Hutao.Web.Request.Builder.Abstraction;

internal interface IHttpContentBuilder : IBuilder
{
    HttpContent? Content { get; set; }

    HttpContentSerializer HttpContentSerializer { get; }
}