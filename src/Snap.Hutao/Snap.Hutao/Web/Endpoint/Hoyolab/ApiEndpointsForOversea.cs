// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint.Hoyolab;

[Injection(InjectAs.Singleton, typeof(IApiEndpoints), Key = ApiEndpointsKind.Oversea)]
internal sealed class ApiEndpointsForOversea : ApiEndpointsImplmentationForOversea;