// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint.Hoyolab;

[Service(ServiceLifetime.Singleton, typeof(IApiEndpoints), Key = ApiEndpointsKind.Chinese)]
internal sealed class ApiEndpointsForChinese : ApiEndpointsImplementationForChinese;