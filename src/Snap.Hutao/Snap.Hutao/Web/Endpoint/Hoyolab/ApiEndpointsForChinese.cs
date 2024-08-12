// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint.Hoyolab;

[Injection(InjectAs.Singleton, typeof(IApiEndpoints), Key = ApiEndpointsKind.Chinese)]
internal sealed class ApiEndpointsForChinese : IApiEndpoints
{
    string IApiTakumiRootAccess.Root { get => "https://api-takumi.mihoyo.com"; }

    string IApiTakumiAuthApiRootAccess.RootAuthApi { get => $"{((IApiTakumiRootAccess)this).Root}/auth/api"; }

    string IGameBizAccess.GameBiz { get => "hk4e_cn"; }
}