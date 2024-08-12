// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint.Hoyolab;

[Injection(InjectAs.Singleton, typeof(IApiEndpoints), Key = ApiEndpointsKind.Chinese)]
internal sealed class ApiEndpointsForChinese : IApiEndpoints
{
    string IApiTakumiHostAccess.Host { get => "https://api-takumi.mihoyo.com"; }

    string IBbsApiHostAccess.Host { get => "https://bbs-api.mihoyo.com"; }

    string IDownloaderApiRootAccess.DownloaderApiRoot { get => "https://downloader-api.mihoyo.com/downloader"; }

    string IApiTakumiAuthApiRootAccess.ApiTakumiAuthApiRoot { get => $"{((IApiTakumiHostAccess)this).Host}/auth/api"; }

    string IGameBizAccess.GameBiz { get => "hk4e_cn"; }

    string IBbsApiEndpoints.BbsReferer { get => $"https://bbs.mihoyo.com/"; }

    string IBbsApiEndpoints.UserFullInfoQuery(string accountId)
    {
        return $"{((IBbsApiHostAccess)this).Host}/user/wapi/getUserFullInfo?uid={accountId}&gids=2";
    }
}