// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Response;
using BindingUser = Snap.Hutao.ViewModel.User.User;
using EntityUser = Snap.Hutao.Model.Entity.User;

namespace Snap.Hutao.Service.User;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IUserService))]
internal sealed partial class UserService : IUserService, IUserServiceUnsafe
{
    private readonly IProfilePictureService profilePictureService;
    private readonly IUserCollectionService userCollectionService;
    private readonly IServiceProvider serviceProvider;
    private readonly IUserRepository userDbService;
    private readonly ITaskContext taskContext;

    public ValueTask RemoveUserAsync(BindingUser user)
    {
        return userCollectionService.RemoveUserAsync(user);
    }

    public async ValueTask UnsafeRemoveAllUsersAsync()
    {
        await taskContext.SwitchToBackgroundAsync();
        userDbService.RemoveAllUsers();
    }

    public ValueTask<AdvancedDbCollectionView<BindingUser, EntityUser>> GetUsersAsync()
    {
        return userCollectionService.GetUsersAsync();
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
        if (await this.GetUserByMidAsync(midOrAid).ConfigureAwait(false) is not { } user)
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

        userDbService.UpdateUser(user.Entity);
        return new(UserOptionResult.CookieUpdated, midOrAid);
    }

    public async ValueTask<bool> RefreshCookieTokenAsync(EntityUser user)
    {
        // TODO: 提醒其他组件此用户的Cookie已更改
        Response<UidCookieToken> cookieTokenResponse;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IPassportClient passportClient = serviceProvider
                .GetRequiredService<IOverseaSupportFactory<IPassportClient>>()
                .Create(user.IsOversea);

            cookieTokenResponse = await passportClient
                .GetCookieAccountInfoBySTokenAsync(user)
                .ConfigureAwait(false);
        }

        if (!cookieTokenResponse.IsOk())
        {
            return false;
        }

        string cookieToken = cookieTokenResponse.Data.CookieToken;

        // Check null and create a new one to avoid System.NullReferenceException
        user.CookieToken ??= new();

        user.CookieToken[Cookie.COOKIE_TOKEN] = cookieToken;
        userDbService.UpdateUser(user);

        return true;
    }

    public async ValueTask RefreshProfilePictureAsync(UserGameRole userGameRole)
    {
        await profilePictureService.RefreshUserGameRoleAsync(userGameRole).ConfigureAwait(false);
    }
}