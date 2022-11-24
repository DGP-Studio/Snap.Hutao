// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.User;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 使用Stokn提供祈愿Url
/// </summary>
[Injection(InjectAs.Transient, typeof(IGachaLogUrlProvider))]
internal class GachaLogUrlStokenProvider : IGachaLogUrlProvider
{
    private readonly IUserService userService;
    private readonly BindingClient2 bindingClient2;

    /// <summary>
    /// 构造一个新的提供器
    /// </summary>
    /// <param name="userService">用户服务</param>
    /// <param name="bindingClient2">绑定客户端</param>
    public GachaLogUrlStokenProvider(IUserService userService, BindingClient2 bindingClient2)
    {
        this.userService = userService;
        this.bindingClient2 = bindingClient2;
    }

    /// <inheritdoc/>
    public string Name { get => nameof(GachaLogUrlStokenProvider); }

    /// <inheritdoc/>
    public async Task<ValueResult<bool, string>> GetQueryAsync()
    {
        Model.Binding.User.User? user = userService.Current;
        if (user != null && user.SelectedUserGameRole != null)
        {
            if (user.Stoken != null)
            {
                PlayerUid uid = (PlayerUid)user.SelectedUserGameRole;
                GenAuthKeyData data = GenAuthKeyData.CreateForWebViewGacha(uid);

                GameAuthKey? authkey = await bindingClient2.GenerateAuthenticationKeyAsync(user.Entity, data).ConfigureAwait(false);
                if (authkey != null)
                {
                    return new(true, GachaLogConfigration.AsQuery(data, authkey));
                }
                else
                {
                    return new(false, "请求验证密钥失败");
                }
            }
            else
            {
                return new(false, "当前用户的 Cookie 不包含 Stoken");
            }
        }
        else
        {
            return new(false, "尚未选择要刷新的用户以及角色");
        }
    }
}