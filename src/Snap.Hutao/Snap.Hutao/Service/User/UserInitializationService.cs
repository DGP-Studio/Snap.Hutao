// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Model.Entity.Extension;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Bbs.User;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Service.User;

[Service(ServiceLifetime.Singleton, typeof(IUserInitializationService))]
internal sealed partial class UserInitializationService : IUserInitializationService
{
    private readonly IUserFingerprintService userFingerprintService;
    private readonly IProfilePictureService profilePictureService;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;

    [GeneratedConstructor]
    public partial UserInitializationService(IServiceProvider serviceProvider);

    public ValueTask<ViewModel.User.User> ResumeUserAsync(Model.Entity.User entity, CancellationToken token = default)
    {
        ViewModel.User.User user = ViewModel.User.User.From(entity, serviceProvider);
        return ResumeUserAsync(user, token);
    }

    public async ValueTask<ViewModel.User.User> ResumeUserAsync(ViewModel.User.User user, CancellationToken token = default)
    {
        if (!await InitializeUserAsync(user, token).ConfigureAwait(false))
        {
            user.UserInfo = new()
            {
                Uid = SH.ModelBindingUserInitializationFailed,
                Nickname = SH.ModelBindingUserInitializationFailed,
            };

            await taskContext.SwitchToMainThreadAsync();
            user.UserGameRoles = new AdvancedCollectionView<UserGameRole>([]);
        }

        return user;
    }

    public async ValueTask<ViewModel.User.User?> CreateUserFromInputCookieOrDefaultAsync(InputCookie inputCookie, CancellationToken token = default)
    {
        // 这里只负责创建实体用户，稍后在用户服务中保存到数据库
        (Cookie cookie, bool isOversea, string? deviceFp) = inputCookie;
        Model.Entity.User entity = Model.Entity.User.From(cookie);

        entity.Aid = cookie.GetValueOrDefault(Cookie.STUID);
        entity.Mid = cookie.GetValueOrDefault(Cookie.MID);
        entity.IsOversea = isOversea;
        entity.TryUpdateFingerprint(deviceFp);

        if (entity.Aid is not null && entity.Mid is not null)
        {
            ViewModel.User.User user = ViewModel.User.User.From(entity, serviceProvider);
            bool initialized = await InitializeUserAsync(user, token).ConfigureAwait(false);

            return initialized ? user : null;
        }

        return null;
    }

    private static async ValueTask<bool> TrySetUserLTokenAsync(IServiceProvider serviceProvider, ViewModel.User.User user, CancellationToken token)
    {
        if (user.LToken is not null)
        {
            return true;
        }

        IPassportClient passportClient = serviceProvider
            .GetRequiredService<IOverseaSupportFactory<IPassportClient>>()
            .Create(user.IsOversea);

        Response<LTokenWrapper> lTokenResponse = await passportClient
            .GetLTokenBySTokenAsync(user.Entity, token)
            .ConfigureAwait(false);

        if (ResponseValidator.TryValidate(lTokenResponse, serviceProvider, out LTokenWrapper? wrapper))
        {
            user.LToken = new()
            {
                [Cookie.LTUID] = user.Entity.Aid ?? string.Empty,
                [Cookie.LTOKEN] = wrapper.LToken,
            };
            return true;
        }

        return false;
    }

    private static async ValueTask<bool> TrySetUserCookieTokenAsync(IServiceProvider serviceProvider, ViewModel.User.User user, CancellationToken token)
    {
        if (user.Entity.CookieTokenLastUpdateTime > DateTimeOffset.UtcNow - TimeSpan.FromDays(1))
        {
            if (user.CookieToken is not null)
            {
                return true;
            }
        }

        IPassportClient passportClient = serviceProvider
            .GetRequiredService<IOverseaSupportFactory<IPassportClient>>()
            .Create(user.IsOversea);

        Response<UidCookieToken> cookieTokenResponse = await passportClient
            .GetCookieAccountInfoBySTokenAsync(user.Entity, token)
            .ConfigureAwait(false);

        if (ResponseValidator.TryValidate(cookieTokenResponse, serviceProvider, out UidCookieToken? uidCookieToken))
        {
            user.CookieToken = new()
            {
                [Cookie.ACCOUNT_ID] = user.Entity.Aid ?? string.Empty,
                [Cookie.COOKIE_TOKEN] = uidCookieToken.CookieToken,
            };

            user.Entity.CookieTokenLastUpdateTime = DateTimeOffset.UtcNow;
            user.NeedDbUpdateAfterResume = true;
            return true;
        }

        return false;
    }

    private static async ValueTask<bool> TrySetUserUserInfoAsync(IServiceProvider serviceProvider, ViewModel.User.User user, CancellationToken token)
    {
        IUserClient userClient = serviceProvider
            .GetRequiredService<IOverseaSupportFactory<IUserClient>>()
            .Create(user.IsOversea);

        Response<UserFullInfoWrapper> response = await userClient
            .GetUserFullInfoAsync(user.Entity, token)
            .ConfigureAwait(false);

        if (ResponseValidator.TryValidate(response, serviceProvider, out UserFullInfoWrapper? wrapper))
        {
            user.UserInfo = wrapper.UserInfo;
            return true;
        }

        return false;
    }

    private static async ValueTask<bool> TrySetUserUserGameRolesAsync(IServiceProvider serviceProvider, ViewModel.User.User user, CancellationToken token)
    {
        BindingClient bindingClient = serviceProvider.GetRequiredService<BindingClient>();

        Response<ListWrapper<UserGameRole>> userGameRolesResponse = await bindingClient
            .GetUserGameRolesOverseaAwareAsync(user.Entity, token)
            .ConfigureAwait(false);

        if (ResponseValidator.TryValidate(userGameRolesResponse, serviceProvider, out ListWrapper<UserGameRole>? wrapper))
        {
            user.UserGameRoles = wrapper.List.AsAdvancedCollectionView();
            return true;
        }

        return false;
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

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IServiceProvider serviceProvider = scope.ServiceProvider;

            if (!await TrySetUserLTokenAsync(serviceProvider, user, token).ConfigureAwait(false))
            {
                return false;
            }

            if (!await TrySetUserCookieTokenAsync(serviceProvider, user, token).ConfigureAwait(false))
            {
                return false;
            }

            if (!await TrySetUserUserInfoAsync(serviceProvider, user, token).ConfigureAwait(false))
            {
                return false;
            }

            if (!await TrySetUserUserGameRolesAsync(serviceProvider, user, token).ConfigureAwait(false))
            {
                return false;
            }
        }

        await userFingerprintService.TryInitializeAsync(user, token).ConfigureAwait(false);
        await profilePictureService.TryInitializeAsync(user, token).ConfigureAwait(false);

        return user.IsInitialized = true;
    }
}