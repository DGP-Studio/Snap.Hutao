// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab;

namespace Snap.Hutao.Web.Endpoint.Hoyolab;

[Service(ServiceLifetime.Singleton, typeof(IApiEndpoints), Key = ApiEndpointsKind.Oversea)]
internal sealed class ApiEndpointsForOversea : ApiEndpointsImplementationForOversea;