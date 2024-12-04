// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using System.Net.Http;

namespace Snap.Hutao.Web.Request.Builder.Abstraction;

internal interface IHttpRequestOptionsBuilder : IBuilder
{
    HttpRequestOptions Options { get; }
}