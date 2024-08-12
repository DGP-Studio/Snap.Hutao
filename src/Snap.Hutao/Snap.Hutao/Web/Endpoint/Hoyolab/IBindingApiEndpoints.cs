// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint.Hoyolab;

internal interface IBindingApiEndpoints : IApiTakumiHostAccess, IGameBizAccess
{
    // Used by Chinese only
    public string UserGameRolesByActionTicket(string actionTicket)
    {
        return $"{Host}/binding/api/getUserGameRoles?action_ticket={actionTicket}&game_biz={GameBiz}";
    }

    public string UserGameRolesByCookie()
    {
        return $"{Host}/binding/api/getUserGameRolesByCookie?game_biz={GameBiz}";
    }

    public string BindingGenAuthKey()
    {
        return $"{Host}/binding/api/genAuthKey";
    }
}