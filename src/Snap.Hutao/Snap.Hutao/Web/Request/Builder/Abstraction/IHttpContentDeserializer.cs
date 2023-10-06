// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Net.Http;

namespace Snap.Hutao.Web.Request.Builder.Abstraction;

public interface IHttpContentDeserializer
{
    Task<object?> DeserializeAsync(HttpContent? httpContent, Type contentType, CancellationToken cancellationToken = default);
}