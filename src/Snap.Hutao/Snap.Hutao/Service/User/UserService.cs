// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Message;
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
    private readonly Throttler throttler = new();

    private readonly ScopedDbCurrent<BindingUser, Model.Entity.User, UserChangedMessage> dbCurrent;
    private readonly IUserInitializationService userInitializationService;
    private readonly IServiceProvider serviceProvider;
    private readonly IUserDbService userDbService;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    private ObservableCollection<BindingUser>? userCollection;
    private ObservableCollection<UserAndUid>? userAndUidCollection;

    /// <inheritdoc/>
    public BindingUser? Current
    {
        get => dbCurrent.Current;
        set => dbCurrent.Current = value;
    }

    /// <inheritdoc/>
    public async ValueTask RemoveUserAsync(BindingUser user)
    {
        // Sync cache
        await taskContext.SwitchToMainThreadAsync();
        ArgumentNullException.ThrowIfNull(userCollection);
        userCollection.Remove(user);
        userAndUidCollection?.RemoveWhere(r => r.User.Mid == user.Entity.Mid);

        // Sync database
        await taskContext.SwitchToBackgroundAsync();
        await userDbService.DeleteUserByIdAsync(user.Entity.InnerId).ConfigureAwait(false);

        messenger.Send(new UserRemovedMessage(user.Entity));
    }

    public async ValueTask UnsafeRemoveUsersAsync()
    {
        await taskContext.SwitchToBackgroundAsync();
        await userDbService.DeleteUsersAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask<ObservableCollection<BindingUser>> GetUserCollectionAsync()
    {
        using (await throttler.ThrottleAsync().ConfigureAwait(false))
        {
            if (userCollection is null)
            {
                List<Model.Entity.User> entities = await userDbService.GetUserListAsync().ConfigureAwait(false);
                List<BindingUser> users = await entities.SelectListAsync(userInitializationService.ResumeUserAsync, default).ConfigureAwait(false);
                userCollection = users.ToObservableCollection();

                try
                {
                    Current = users.SelectedOrDefault();
                }
                catch (InvalidOperationException ex)
                {
                    ThrowHelper.UserdataCorrupted(SH.ServiceUserCurrentMultiMatched, ex);
                }
            }
        }

        return userCollection;
    }

    /// <inheritdoc/>
    public async ValueTask<ObservableCollection<UserAndUid>> GetRoleCollectionAsync()
    {
        await taskContext.SwitchToBackgroundAsync();
        if (userAndUidCollection is null)
        {
            ObservableCollection<BindingUser> users = await GetUserCollectionAsync().ConfigureAwait(false);
            userAndUidCollection = users
                .SelectMany(user => user.UserGameRoles.Select(role => UserAndUid.From(user.Entity, role)))
                .ToObservableCollection();
        }

        return userAndUidCollection;
    }

    /// <inheritdoc/>
    public UserGameRole? GetUserGameRoleByUid(string uid)
    {
        if (userCollection is not null)
        {
            try
            {
                return userCollection.SelectMany(u => u.UserGameRoles).SingleOrDefault(r => r.GameUid == uid);
            }
            catch (InvalidOperationException)
            {
                // Sequence contains more than one matching element
                // TODO: return a specialize UserGameRole to indicate error
            }
        }

        return default;
    }

    /// <inheritdoc/>
    public async ValueTask<ValueResult<UserOptionResult, string>> ProcessInputCookieAsync(Cookie cookie, bool isOversea)
    {
        await taskContext.SwitchToBackgroundAsync();
        string? mid = cookie.GetValueOrDefault(isOversea ? Cookie.STUID : Cookie.MID);

        if (string.IsNullOrEmpty(mid))
        {
            return new(UserOptionResult.Invalid, SH.ServiceUserProcessCookieNoMid);
        }

        // 检查 mid 对应用户是否存在
        ArgumentNullException.ThrowIfNull(userCollection);
        if (TryGetUser(userCollection, mid, out BindingUser? user))
        {
            if (cookie.TryGetSToken(isOversea, out Cookie? stoken))
            {
                user.SToken = stoken;
                user.LToken = cookie.TryGetLToken(out Cookie? ltoken) ? ltoken : user.LToken;
                user.CookieToken = cookie.TryGetCookieToken(out Cookie? cookieToken) ? cookieToken : user.CookieToken;

                await userDbService.UpdateUserAsync(user.Entity).ConfigureAwait(false);
                return new(UserOptionResult.Updated, mid);
            }
            else
            {
                return new(UserOptionResult.Invalid, SH.ServiceUserProcessCookieNoSToken);
            }
        }
        else
        {
            return await TryCreateUserAndAddAsync(cookie, isOversea).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public ValueTask<bool> RefreshCookieTokenAsync(BindingUser user)
    {
        return RefreshCookieTokenAsync(user.Entity);
    }

    public async ValueTask<bool> RefreshCookieTokenAsync(Model.Entity.User user)
    {
        // TODO: 提醒其他组件此用户的Cookie已更改
        Response<UidCookieToken> cookieTokenResponse = await serviceProvider
            .GetRequiredService<IOverseaSupportFactory<IPassportClient>>()
            .Create(user.IsOversea)
            .GetCookieAccountInfoBySTokenAsync(user)
            .ConfigureAwait(false);

        if (cookieTokenResponse.IsOk())
        {
            string cookieToken = cookieTokenResponse.Data.CookieToken;

            // Check null and create a new one to avoid System.NullReferenceException
            user.CookieToken ??= new();

            // Sync ui and database
            user.CookieToken[Cookie.COOKIE_TOKEN] = cookieToken;
            await userDbService.UpdateUserAsync(user).ConfigureAwait(false);

            return true;
        }
        else
        {
            return false;
        }
    }

    private static bool TryGetUser(ObservableCollection<BindingUser> users, string mid, [NotNullWhen(true)] out BindingUser? user)
    {
        user = users.SingleOrDefault(u => u.Entity.Mid == mid);
        return user is not null;
    }

    private async ValueTask<ValueResult<UserOptionResult, string>> TryCreateUserAndAddAsync(Cookie cookie, bool isOversea)
    {
        await taskContext.SwitchToBackgroundAsync();
        BindingUser? newUser = await userInitializationService.CreateOrDefaultUserFromCookieAsync(cookie, isOversea).ConfigureAwait(false);

        if (newUser is not null)
        {
            // Sync cache
            if (userCollection is not null)
            {
                await taskContext.SwitchToMainThreadAsync();
                {
                    userCollection.Add(newUser);

                    if (userAndUidCollection is not null)
                    {
                        foreach (UserGameRole role in newUser.UserGameRoles)
                        {
                            userAndUidCollection.Add(new(newUser.Entity, role));
                        }
                    }
                }
            }

            // Sync database
            await taskContext.SwitchToBackgroundAsync();
            await userDbService.AddUserAsync(newUser.Entity).ConfigureAwait(false);
            ArgumentNullException.ThrowIfNull(newUser.UserInfo);
            return new(UserOptionResult.Added, newUser.UserInfo.Uid);
        }
        else
        {
            return new(UserOptionResult.Invalid, SH.ServiceUserProcessCookieRequestUserInfoFailed);
        }
    }
}