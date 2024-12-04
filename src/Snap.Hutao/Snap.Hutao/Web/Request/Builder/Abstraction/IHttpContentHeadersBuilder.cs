// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Net.Http.Headers;

namespace Snap.Hutao.Web.Request.Builder.Abstraction;

internal interface IHttpContentHeadersBuilder : IHttpContentBuilder, IHttpHeadersBuilder<HttpContentHeaders>;