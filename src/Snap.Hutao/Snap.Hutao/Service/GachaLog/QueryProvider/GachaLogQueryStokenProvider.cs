// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding.User;
using Snap.Hutao.Service.User;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Service.GachaLog.QueryProvider;

/// <summary>
/// 使用Stokn提供祈愿Url
/// </summary>
[HighQuality]
[Injection(InjectAs.Transient, typeof(IGachaLogQueryProvider))]
internal sealed class GachaLogQueryStokenProvider : IGachaLogQueryProvider
{
    private readonly IUserService userService;
    private readonly BindingClient2 bindingClient2;

    /// <summary>
    /// 构造一个新的提供器
    /// </summary>
    /// <param name="userService">用户服务</param>
    /// <param name="bindingClient2">绑定客户端</param>
    public GachaLogQueryStokenProvider(IUserService userService, BindingClient2 bindingClient2)
    {
        this.userService = userService;
        this.bindingClient2 = bindingClient2;
    }

    /// <inheritdoc/>
    public string Name { get => nameof(GachaLogQueryStokenProvider); }

    /// <inheritdoc/>
    public async Task<ValueResult<bool, GachaLogQuery>> GetQueryAsync()
    {
        if (UserAndUid.TryFromUser(userService.Current, out UserAndUid? userAndUid))
        {
            if (userAndUid.Uid.Region != "cn_gf01" && userAndUid.Uid.Region != "cn_qd01")
            {
                return new(false, "Unsupported for hoyoverse account");
            }

            GenAuthKeyData data = GenAuthKeyData.CreateForWebViewGacha(userAndUid.Uid);
            Response<GameAuthKey> authkeyResponse = await bindingClient2.GenerateAuthenticationKeyAsync(userAndUid.User, data).ConfigureAwait(false);

            if (authkeyResponse.IsOk())
            {
                return new(true, new(GachaLogQueryOptions.AsQuery(data, authkeyResponse.Data), false));
            }
            else
            {
                return new(false, SH.ServiceGachaLogUrlProviderAuthkeyRequestFailed);
            }
        }
        else
        {
            return new(false, SH.MustSelectUserAndUid);
        }
    }
}