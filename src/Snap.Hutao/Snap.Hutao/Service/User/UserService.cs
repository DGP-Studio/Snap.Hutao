// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Response;
using System.Collections.ObjectModel;
using BindingUser = Snap.Hutao.ViewModel.User.User;

namespace Snap.Hutao.Service.User;

/// <summary>
/// 用户服务
/// </summary>
[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IUserService))]
internal sealed partial class UserService : IUserService, IUserServiceUnsafe
{
    private readonly IUserCollectionService userCollectionService;
    private readonly IServiceProvider serviceProvider;
    private readonly IUserDbService userDbService;
    private readonly ITaskContext taskContext;

    public BindingUser? Current
    {
        get => userCollectionService.CurrentUser;
        set => userCollectionService.CurrentUser = value;
    }

    public ValueTask RemoveUserAsync(BindingUser user)
    {
        return userCollectionService.RemoveUserAsync(user);
    }

    public async ValueTask UnsafeRemoveUsersAsync()
    {
        await taskContext.SwitchToBackgroundAsync();
        await userDbService.RemoveUsersAsync().ConfigureAwait(false);
    }

    public ValueTask<ObservableCollection<BindingUser>> GetUserCollectionAsync()
    {
        return userCollectionService.GetUserCollectionAsync();
    }

    public ValueTask<ObservableCollection<UserAndUid>> GetRoleCollectionAsync()
    {
        return userCollectionService.GetUserAndUidCollectionAsync();
    }

    public UserGameRole? GetUserGameRoleByUid(string uid)
    {
        return userCollectionService.GetUserGameRoleByUid(uid);
    }

    public async ValueTask<ValueResult<UserOptionResult, string>> ProcessInputCookieAsync(InputCookie inputCookie)
    {
        await taskContext.SwitchToBackgroundAsync();
        (Cookie cookie, bool isOversea, string? deviceFp) = inputCookie;

        string? midOrAid = cookie.GetValueOrDefault(isOversea ? Cookie.STUID : Cookie.MID);

        if (string.IsNullOrEmpty(midOrAid))
        {
            return new(UserOptionResult.CookieInvalid, SH.ServiceUserProcessCookieNoMid);
        }

        // 检查 mid 对应用户是否存在
        if (!userCollectionService.TryGetUserByMid(midOrAid, out BindingUser? user))
        {
            return await userCollectionService.TryCreateAndAddUserFromInputCookieAsync(inputCookie).ConfigureAwait(false);
        }

        if (!cookie.TryGetSToken(isOversea, out Cookie? stoken))
        {
            return new(UserOptionResult.CookieInvalid, SH.ServiceUserProcessCookieNoSToken);
        }

        user.SToken = stoken;
        user.LToken = cookie.TryGetLToken(out Cookie? ltoken) ? ltoken : user.LToken;
        user.CookieToken = cookie.TryGetCookieToken(out Cookie? cookieToken) ? cookieToken : user.CookieToken;
        user.TryUpdateFingerprint(deviceFp);

        await userDbService.UpdateUserAsync(user.Entity).ConfigureAwait(false);
        return new(UserOptionResult.CookieUpdated, midOrAid);
    }

    public async ValueTask<bool> RefreshCookieTokenAsync(Model.Entity.User user)
    {
        // TODO: 提醒其他组件此用户的Cookie已更改
        Response<UidCookieToken> cookieTokenResponse = await serviceProvider
            .GetRequiredService<IOverseaSupportFactory<IPassportClient>>()
            .Create(user.IsOversea)
            .GetCookieAccountInfoBySTokenAsync(user)
            .ConfigureAwait(false);

        if (!cookieTokenResponse.IsOk())
        {
            return false;
        }

        string cookieToken = cookieTokenResponse.Data.CookieToken;

        // Check null and create a new one to avoid System.NullReferenceException
        user.CookieToken ??= new();

        // Sync ui and database
        user.CookieToken[Cookie.COOKIE_TOKEN] = cookieToken;
        await userDbService.UpdateUserAsync(user).ConfigureAwait(false);

        return true;
    }
}