// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint.Hoyolab;

internal interface IAuthApiEndpoints : IApiTakumiAuthApiRootAccess
{
    string AuthActionTicket(string actionType, string stoken, string uid)
    {
        return $"{ApiTakumiAuthApiRoot}/getActionTicketBySToken?action_type={actionType}&stoken={Uri.EscapeDataString(stoken)}&uid={uid}";
    }

    string AuthMultiToken(string loginTicket, string loginUid)
    {
        return $"{ApiTakumiAuthApiRoot}/getMultiTokenByLoginTicket?login_ticket={loginTicket}&uid={loginUid}&token_types=3";
    }
}