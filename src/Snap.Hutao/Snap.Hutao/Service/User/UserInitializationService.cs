// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Bbs.User;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Service.User;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IUserInitializationService))]
internal sealed partial class UserInitializationService : IUserInitializationService
{
    private readonly IUserFingerprintService userFingerprintService;
    private readonly IServiceProvider serviceProvider;

    public async ValueTask<ViewModel.User.User> ResumeUserAsync(Model.Entity.User inner, CancellationToken token = default)
    {
        ViewModel.User.User user = ViewModel.User.User.From(inner, serviceProvider);

        if (!await InitializeUserAsync(user, token).ConfigureAwait(false))
        {
            user.UserInfo = new() { Nickname = SH.ModelBindingUserInitializationFailed };
            user.UserGameRoles = new();
        }

        return user;
    }

    public async ValueTask<ViewModel.User.User?> CreateUserFromCookieOrDefaultAsync(Cookie cookie, bool isOversea, CancellationToken token = default)
    {
        // 这里只负责创建实体用户，稍后在用户服务中保存到数据库
        Model.Entity.User entity = Model.Entity.User.From(cookie, isOversea);

        entity.Aid = cookie.GetValueOrDefault(Cookie.STUID);
        entity.Mid = isOversea ? entity.Aid : cookie.GetValueOrDefault(Cookie.MID);
        entity.IsOversea = isOversea;

        if (entity.Aid is not null && entity.Mid is not null)
        {
            ViewModel.User.User user = ViewModel.User.User.From(entity, serviceProvider);
            bool initialized = await InitializeUserAsync(user, token).ConfigureAwait(false);

            return initialized ? user : null;
        }
        else
        {
            return null;
        }
    }

    private async ValueTask<bool> InitializeUserAsync(ViewModel.User.User user, CancellationToken token = default)
    {
        if (user.IsInitialized)
        {
            // Prevent multiple initialization.
            return true;
        }

        if (user.SToken is null)
        {
            return false;
        }

        if (!await TrySetUserLTokenAsync(user, token).ConfigureAwait(false))
        {
            return false;
        }

        if (!await TrySetUserCookieTokenAsync(user, token).ConfigureAwait(false))
        {
            return false;
        }

        if (!await TrySetUserUserInfoAsync(user, token).ConfigureAwait(false))
        {
            return false;
        }

        if (!await TrySetUserUserGameRolesAsync(user, token).ConfigureAwait(false))
        {
            return false;
        }

        await userFingerprintService.TryInitializeAsync(user, token).ConfigureAwait(false);

        user.SelectedUserGameRole = user.UserGameRoles.FirstOrFirstOrDefault(role => role.IsChosen);
        return user.IsInitialized = true;
    }

    private async ValueTask<bool> TrySetUserLTokenAsync(ViewModel.User.User user, CancellationToken token)
    {
        if (user.LToken is not null)
        {
            return true;
        }

        Response<LTokenWrapper> lTokenResponse = await serviceProvider
            .GetRequiredService<IOverseaSupportFactory<IPassportClient>>()
            .Create(user.IsOversea)
            .GetLTokenBySTokenAsync(user.Entity, token)
            .ConfigureAwait(false);

        if (lTokenResponse.IsOk())
        {
            user.LToken = new()
            {
                [Cookie.LTUID] = user.Entity.Aid ?? string.Empty,
                [Cookie.LTOKEN] = lTokenResponse.Data.LToken,
            };
            return true;
        }
        else
        {
            return false;
        }
    }

    private async ValueTask<bool> TrySetUserCookieTokenAsync(ViewModel.User.User user, CancellationToken token)
    {
        if (user.CookieToken is not null)
        {
            return true;
        }

        Response<UidCookieToken> cookieTokenResponse = await serviceProvider
            .GetRequiredService<IOverseaSupportFactory<IPassportClient>>()
            .Create(user.IsOversea)
            .GetCookieAccountInfoBySTokenAsync(user.Entity, token)
            .ConfigureAwait(false);

        if (cookieTokenResponse.IsOk())
        {
            user.CookieToken = new()
            {
                [Cookie.ACCOUNT_ID] = user.Entity.Aid ?? string.Empty,
                [Cookie.COOKIE_TOKEN] = cookieTokenResponse.Data.CookieToken,
            };
            return true;
        }
        else
        {
            return false;
        }
    }

    private async ValueTask<bool> TrySetUserUserInfoAsync(ViewModel.User.User user, CancellationToken token)
    {
        Response<UserFullInfoWrapper> response = await serviceProvider
            .GetRequiredService<IOverseaSupportFactory<IUserClient>>()
            .Create(user.IsOversea)
            .GetUserFullInfoAsync(user.Entity, token)
            .ConfigureAwait(false);

        if (response.IsOk())
        {
            user.UserInfo = response.Data.UserInfo;
            return true;
        }
        else
        {
            return false;
        }
    }

    private async ValueTask<bool> TrySetUserUserGameRolesAsync(ViewModel.User.User user, CancellationToken token)
    {
        Response<ListWrapper<UserGameRole>> userGameRolesResponse = await serviceProvider
            .GetRequiredService<BindingClient>()
            .GetUserGameRolesOverseaAwareAsync(user.Entity, token)
            .ConfigureAwait(false);

        if (userGameRolesResponse.IsOk())
        {
            user.UserGameRoles = userGameRolesResponse.Data.List;
            return user.UserGameRoles.Any();
        }
        else
        {
            return false;
        }
    }
}