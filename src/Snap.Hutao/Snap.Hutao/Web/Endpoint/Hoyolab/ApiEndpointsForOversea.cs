// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint.Hoyolab;

[Injection(InjectAs.Singleton, typeof(IApiEndpoints), Key = ApiEndpointsKind.Oversea)]
internal sealed class ApiEndpointsForOversea : IApiEndpoints
{
    string IApiTakumiRootAccess.Root { get => "https://api-os-takumi.hoyoverse.com"; }

    string IApiTakumiAuthApiRootAccess.RootAuthApi { get => $"{((IApiTakumiRootAccess)this).Root}/account/auth/api"; }

    string IGameBizAccess.GameBiz { get => "hk4e_global"; }
}