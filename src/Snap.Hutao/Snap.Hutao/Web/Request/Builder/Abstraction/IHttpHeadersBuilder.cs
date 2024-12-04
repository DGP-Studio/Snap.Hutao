// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using System.Net.Http.Headers;

namespace Snap.Hutao.Web.Request.Builder.Abstraction;

internal interface IHttpHeadersBuilder<out T> : IBuilder
    where T : HttpHeaders
{
    T Headers { get; }
}