// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint.Hoyolab;

internal interface IBindingApiEndpoints : IApiTakumiRootAccess, IGameBizAccess
{
    // Used by Chinese only
    public string UserGameRolesByActionTicket(string actionTicket)
    {
        return $"{Root}/binding/api/getUserGameRoles?action_ticket={actionTicket}&game_biz={GameBiz}";
    }

    public string UserGameRolesByCookie()
    {
        return $"{Root}/binding/api/getUserGameRolesByCookie?game_biz={GameBiz}";
    }

    public string BindingGenAuthKey()
    {
        return $"{Root}/binding/api/genAuthKey";
    }
}