// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Net.Http;
using System.Text;

namespace Snap.Hutao.Web.Request.Builder.Abstraction;

internal interface IHttpContentSerializer
{
    HttpContent? Serialize(object? content, Type contentType, Encoding? encoding);
}