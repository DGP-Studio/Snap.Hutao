// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
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
/// 主要负责将用户数据与数据库同步
/// </summary>
[Injection(InjectAs.Singleton, typeof(IUserService))]
internal class UserService : IUserService
{
    private readonly ITaskContext taskContext;
    private readonly IServiceProvider serviceProvider;
    private readonly IMessenger messenger;
    private readonly object currentUserLocker = new();

    private BindingUser? currentUser;
    private ObservableCollection<BindingUser>? userCollection;
    private ObservableCollection<UserAndUid>? roleCollection;

    /// <summary>
    /// 构造一个新的用户服务
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="messenger">消息器</param>
    public UserService(IServiceProvider serviceProvider, IMessenger messenger)
    {
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();

        this.serviceProvider = serviceProvider;
        this.messenger = messenger;
    }

    /// <inheritdoc/>
    public BindingUser? Current
    {
        get => currentUser;
        set
        {
            if (currentUser == value)
            {
                return;
            }

            lock (currentUserLocker)
            {
                using (IServiceScope scope = serviceProvider.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    // only update when not processing a deletion
                    if (value != null)
                    {
                        if (currentUser != null)
                        {
                            currentUser.IsSelected = false;
                            appDbContext.Users.UpdateAndSave(currentUser.Entity);
                        }
                    }

                    UserChangedMessage message = new() { OldValue = currentUser, NewValue = value };

                    // 当删除到无用户时也能正常反应状态
                    currentUser = value;

                    if (currentUser != null)
                    {
                        currentUser.IsSelected = true;
                        try
                        {
                            appDbContext.Users.UpdateAndSave(currentUser.Entity);
                        }
                        catch (InvalidOperationException ex)
                        {
                            ThrowHelper.UserdataCorrupted(string.Format(SH.ServiceUserCurrentUpdateAndSaveFailed, currentUser.UserInfo?.Uid), ex);
                        }
                    }

                    messenger.Send(message);
                }
            }
        }
    }

    /// <inheritdoc/>
    public async Task RemoveUserAsync(BindingUser user)
    {
        // Sync cache
        await taskContext.SwitchToMainThreadAsync();
        userCollection!.Remove(user);
        roleCollection?.RemoveWhere(r => r.User.Mid == user.Entity.Mid);

        // Sync database
        await taskContext.SwitchToBackgroundAsync();
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.Users
                .ExecuteDeleteWhereAsync(u => u.InnerId == user.Entity.InnerId)
                .ConfigureAwait(false);
        }

        messenger.Send(new UserRemovedMessage(user.Entity));
    }

    /// <inheritdoc/>
    public async Task<ObservableCollection<BindingUser>> GetUserCollectionAsync()
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
        if (roleCollection == null)
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

            roleCollection = userAndUids.ToObservableCollection();
        }

        return roleCollection;
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
                .PickRequiredService<IPassportClient>(user.Entity.IsOversea)
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

                        if (roleCollection != null)
                        {
                            foreach (UserGameRole role in newUser.UserGameRoles)
                            {
                                roleCollection.Add(new(newUser.Entity, role));
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