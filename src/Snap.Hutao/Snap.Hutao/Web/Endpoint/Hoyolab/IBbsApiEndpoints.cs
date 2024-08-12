// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint.Hoyolab;

internal interface IBbsApiEndpoints : IBbsApiHostAccess
{
    string BbsReferer { get; }

    string UserFullInfoQuery(string accountId);
}