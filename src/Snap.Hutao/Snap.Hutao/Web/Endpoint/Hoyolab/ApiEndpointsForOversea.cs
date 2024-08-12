// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;

namespace Snap.Hutao.Web.Endpoint.Hoyolab;

[Injection(InjectAs.Singleton, typeof(IApiEndpoints), Key = ApiEndpointsKind.Oversea)]
internal sealed class ApiEndpointsForOversea : IApiEndpoints
{
    string IApiTakumiHostAccess.Host { get => "https://api-os-takumi.hoyoverse.com"; }

    string IBbsApiHostAccess.Host { get => "https://bbs-api-os.hoyoverse.com"; }

    string IDownloaderApiRootAccess.DownloaderApiRoot { get => "https://sg-public-api.hoyoverse.com"; }

    string IApiTakumiAuthApiRootAccess.ApiTakumiAuthApiRoot { get => $"{((IApiTakumiHostAccess)this).Host}/account/auth/api"; }

    string IGameBizAccess.GameBiz { get => "hk4e_global"; }

    string IBbsApiEndpoints.BbsReferer { get => throw HutaoException.NotSupported(); }

    string IBbsApiEndpoints.UserFullInfoQuery(string accountId)
    {
        return $"{((IBbsApiHostAccess)this).Host}/community/painter/wapi/user/full";
    }
}