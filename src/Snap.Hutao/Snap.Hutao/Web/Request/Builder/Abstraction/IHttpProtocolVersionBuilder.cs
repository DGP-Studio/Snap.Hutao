// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Request.Builder.Abstraction;

internal interface IHttpProtocolVersionBuilder : IBuilder
{
    Version Version { get; set; }
}