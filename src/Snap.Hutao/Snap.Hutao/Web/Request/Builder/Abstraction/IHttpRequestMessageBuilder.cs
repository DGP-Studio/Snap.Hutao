// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Net.Http;

namespace Snap.Hutao.Web.Request.Builder.Abstraction;

internal interface IHttpRequestMessageBuilder : IBuilder
{
    HttpRequestMessage HttpRequestMessage { get; set; }
}