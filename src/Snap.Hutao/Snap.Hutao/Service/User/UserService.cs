// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Response;
using BindingUser = Snap.Hutao.ViewModel.User.User;
using EntityUser = Snap.Hutao.Model.Entity.User;

namespace Snap.Hutao.Service.User;

[Service(ServiceLifetime.Singleton, typeof(IUserService))]
internal sealed partial class UserService : IUserService
{
    private readonly IUserInitializationService userInitializationService;
    private readonly IProfilePictureService profilePictureService;
    private readonly IUserCollectionService userCollectionService;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly IUserRepository userRepository;

    [GeneratedConstructor]
    public partial UserService(IServiceProvider serviceProvider);

    public partial ITaskContext TaskContext { get; }

    public ValueTask RemoveUserAsync(BindingUser user)
    {
        return userCollectionService.RemoveUserAsync(user);
    }

    public ValueTask<AdvancedDbCollectionView<BindingUser, EntityUser>> GetUsersAsync()
    {
        return userCollectionService.GetUsersAsync();
    }

    public async ValueTask<ValueResult<UserOptionResultKind, string?>> ProcessInputCookieAsync(InputCookie inputCookie)
    {
        ContentDialog dialog = await contentDialogFactory
            .CreateForIndeterminateProgressAsync(SH.ServiceUserProcessInputCookieDialogTitle)
            .ConfigureAwait(false);

        using (await contentDialogFactory.BlockAsync(dialog).ConfigureAwait(false))
        {
            await TaskContext.SwitchToBackgroundAsync();
            (Cookie cookie, bool _, string? deviceFp) = inputCookie;

            string? mid = cookie.GetValueOrDefault(Cookie.MID);

            if (string.IsNullOrEmpty(mid))
            {
                return new(UserOptionResultKind.CookieInvalid, SH.ServiceUserProcessCookieNoMid);
            }

            if (await this.GetUserByMidAsync(mid).ConfigureAwait(false) is not { } user)
            {
                return await userCollectionService.TryCreateAndAddUserFromInputCookieAsync(inputCookie).ConfigureAwait(false);
            }

            user.IsInitialized = false;

            if (!cookie.TryGetSToken(out Cookie? sToken))
            {
                return new(UserOptionResultKind.CookieInvalid, SH.ServiceUserProcessCookieNoSToken);
            }

            user.SToken = sToken;
            user.LToken = cookie.TryGetLToken(out Cookie? lToken) ? lToken : user.LToken;
            user.CookieToken = cookie.TryGetCookieToken(out Cookie? cookieToken) ? cookieToken : user.CookieToken;
            user.TryUpdateFingerprint(deviceFp);

            await userInitializationService.ResumeUserAsync(user).ConfigureAwait(false);
            userRepository.UpdateUser(user.Entity);
            return new(UserOptionResultKind.CookieUpdated, mid);
        }
    }

    public async ValueTask<bool> RefreshCookieTokenAsync(EntityUser user)
    {
        UidCookieToken? uidCookieToken;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IPassportClient passportClient = scope.ServiceProvider
                .GetRequiredService<IOverseaSupportFactory<IPassportClient>>()
                .Create(user.IsOversea);

            Response<UidCookieToken> cookieTokenResponse = await passportClient
                .GetCookieAccountInfoBySTokenAsync(user)
                .ConfigureAwait(false);

            if (!ResponseValidator.TryValidate(cookieTokenResponse, scope.ServiceProvider, out uidCookieToken))
            {
                return false;
            }
        }

        string cookieToken = uidCookieToken.CookieToken;

        // Check null and create a new one to avoid System.NullReferenceException
        user.CookieToken ??= new();
        user.CookieToken[Cookie.COOKIE_TOKEN] = cookieToken;

        string? mid = user.Mid;
        if (!userRepository.Execute(query => query.Any(u => u.Mid == mid)))
        {
            return false;
        }

        userRepository.UpdateUser(user);
        return true;
    }

    public async ValueTask RefreshProfilePictureAsync(UserGameRole userGameRole)
    {
        await profilePictureService.RefreshUserGameRoleAsync(userGameRole).ConfigureAwait(false);
    }
}