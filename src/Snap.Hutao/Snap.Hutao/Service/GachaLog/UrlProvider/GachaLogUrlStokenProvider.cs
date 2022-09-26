// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Threading;
using Snap.Hutao.Service.Abstraction;
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
        Model.Binding.User? user = userService.CurrentUser;
        if (user != null)
        {
            if (user.Cookie!.Contains(CookieKeys.STOKEN) && user.SelectedUserGameRole != null)
            {
                PlayerUid uid = (PlayerUid)user.SelectedUserGameRole;
                GenAuthKeyData data = GenAuthKeyData.CreateForWebViewGacha(uid);

                GameAuthKey? authkey = await bindingClient2.GenerateAuthenticationKeyAsync(user, data).ConfigureAwait(false);
                if (authkey != null)
                {
                    return new(true, GachaLogConfigration.AsQuery(data, authkey));
                }
            }
        }

        return new(false, null!);
    }
}