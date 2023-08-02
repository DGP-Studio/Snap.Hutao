// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Message;
using Snap.Hutao.Model.Entity.Database;
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
internal sealed partial class UserService : IUserService
{
    private readonly ITaskContext taskContext;
    private readonly IUserDbService userDbService;
    private readonly IServiceProvider serviceProvider;
    private readonly IMessenger messenger;
    private readonly ScopedDbCurrent<BindingUser, Model.Entity.User, UserChangedMessage> dbCurrent;

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
        userCollection!.Remove(user);
        userAndUidCollection?.RemoveWhere(r => r.User.Mid == user.Entity.Mid);

        // Sync database
        await taskContext.SwitchToBackgroundAsync();
        await userDbService.DeleteUserByIdAsync(user.Entity.InnerId).ConfigureAwait(false);

        messenger.Send(new UserRemovedMessage(user.Entity));
    }

    /// <inheritdoc/>
    public async ValueTask<ObservableCollection<BindingUser>> GetUserCollectionAsync()
    {
        await taskContext.SwitchToBackgroundAsync();
        if (userCollection == null)
        {
            List<BindingUser> users = new();

            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                foreach (Model.Entity.User entity in appDbContext.Users)
                {
                    BindingUser initialized = await BindingUser.ResumeAsync(entity).ConfigureAwait(false);
                    users.Add(initialized);
                }
            }

            userCollection = users.ToObservableCollection();
            try
            {
                Current = users.SingleOrDefault(user => user.IsSelected);
            }
            catch (InvalidOperationException ex)
            {
                throw new UserdataCorruptedException(SH.ServiceUserCurrentMultiMatched, ex);
            }
        }

        return userCollection;
    }

    /// <inheritdoc/>
    public async Task<ObservableCollection<UserAndUid>> GetRoleCollectionAsync()
    {
        await taskContext.SwitchToBackgroundAsync();
        if (userAndUidCollection == null)
        {
            List<UserAndUid> userAndUids = new();
            ObservableCollection<BindingUser> observableUsers = await GetUserCollectionAsync().ConfigureAwait(false);
            foreach (BindingUser user in observableUsers)
            {
                foreach (UserGameRole role in user.UserGameRoles)
                {
                    userAndUids.Add(new(user.Entity, role));
                }
            }

            userAndUidCollection = userAndUids.ToObservableCollection();
        }

        return userAndUidCollection;
    }

    /// <inheritdoc/>
    public UserGameRole? GetUserGameRoleByUid(string uid)
    {
        if (userCollection != null)
        {
            try
            {
                // TODO: optimize match speed.
                return userCollection.SelectMany(u => u.UserGameRoles).SingleOrDefault(r => r.GameUid == uid);
            }
            catch (InvalidOperationException)
            {
                // Sequence contains more than one matching element
                // TODO: return a specialize UserGameRole to indicate error
            }
        }

        return null;
    }

    /// <inheritdoc/>
    public async Task<ValueResult<UserOptionResult, string>> ProcessInputCookieAsync(Cookie cookie, bool isOversea)
    {
        await taskContext.SwitchToBackgroundAsync();
        string? mid = cookie.GetValueOrDefault(isOversea ? Cookie.STUID : Cookie.MID);

        if (mid == null)
        {
            return new(UserOptionResult.Invalid, SH.ServiceUserProcessCookieNoMid);
        }

        // 检查 mid 对应用户是否存在
        if (TryGetUser(userCollection!, mid, out BindingUser? user))
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                if (cookie.TryGetSToken(isOversea, out Cookie? stoken))
                {
                    user.SToken = stoken;
                    user.LToken = cookie.TryGetLToken(out Cookie? ltoken) ? ltoken : user.LToken;
                    user.CookieToken = cookie.TryGetCookieToken(out Cookie? cookieToken) ? cookieToken : user.CookieToken;

                    await appDbContext.Users.UpdateAndSaveAsync(user.Entity).ConfigureAwait(false);
                    return new(UserOptionResult.Updated, mid);
                }
                else
                {
                    return new(UserOptionResult.Invalid, SH.ServiceUserProcessCookieNoSToken);
                }
            }
        }
        else
        {
            return await TryCreateUserAndAddAsync(cookie, isOversea).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> RefreshCookieTokenAsync(BindingUser user)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            Response<UidCookieToken> cookieTokenResponse = await scope.ServiceProvider
                .GetRequiredService<IOverseaSupportFactory<IPassportClient>>()
                .Create(user.Entity.IsOversea)
                .GetCookieAccountInfoBySTokenAsync(user.Entity)
                .ConfigureAwait(false);

            if (cookieTokenResponse.IsOk())
            {
                string cookieToken = cookieTokenResponse.Data.CookieToken;

                // Check null and create a new one to avoid System.NullReferenceException
                user.CookieToken ??= new();

                // sync ui and database
                user.CookieToken[Cookie.COOKIE_TOKEN] = cookieToken!;
                scope.ServiceProvider.GetRequiredService<AppDbContext>().Users.UpdateAndSave(user.Entity);

                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private static bool TryGetUser(ObservableCollection<BindingUser> users, string mid, [NotNullWhen(true)] out BindingUser? user)
    {
        user = users.SingleOrDefault(u => u.Entity.Mid == mid);
        return user != null;
    }

    private async Task<ValueResult<UserOptionResult, string>> TryCreateUserAndAddAsync(Cookie cookie, bool isOversea)
    {
        await taskContext.SwitchToBackgroundAsync();
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            BindingUser? newUser = await BindingUser.CreateAsync(cookie, isOversea).ConfigureAwait(false);

            if (newUser != null)
            {
                // Sync cache
                if (userCollection != null)
                {
                    await taskContext.SwitchToMainThreadAsync();
                    {
                        userCollection!.Add(newUser);

                        if (userAndUidCollection != null)
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
                await appDbContext.Users.AddAndSaveAsync(newUser.Entity).ConfigureAwait(false);
                return new(UserOptionResult.Added, newUser.UserInfo!.Uid);
            }
            else
            {
                return new(UserOptionResult.Invalid, SH.ServiceUserProcessCookieRequestUserInfoFailed);
            }
        }
    }
}

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IUserDbService))]
internal sealed partial class UserDbService : IUserDbService
{
    private readonly IServiceProvider serviceProvider;

    public async ValueTask DeleteUserByIdAsync(Guid id)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.Users.ExecuteDeleteWhereAsync(u => u.InnerId == id).ConfigureAwait(false);
        }
    }
}

internal interface IUserDbService
{
    ValueTask DeleteUserByIdAsync(Guid id);
}