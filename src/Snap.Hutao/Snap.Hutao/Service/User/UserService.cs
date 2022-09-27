// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Core.Threading;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Bbs.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Auth;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using System.Collections.ObjectModel;
using BindingUser = Snap.Hutao.Model.Binding.User;

namespace Snap.Hutao.Service;

/// <summary>
/// 用户服务
/// 主要负责将用户数据与数据库同步
/// </summary>
[Injection(InjectAs.Singleton, typeof(IUserService))]
internal class UserService : IUserService
{
    private readonly AppDbContext appDbContext;
    private readonly UserClient userClient;
    private readonly BindingClient userGameRoleClient;
    private readonly AuthClient authClient;
    private readonly IMessenger messenger;

    private BindingUser? currentUser;
    private ObservableCollection<BindingUser>? userCollection;

    /// <summary>
    /// 构造一个新的用户服务
    /// </summary>
    /// <param name="appDbContext">应用程序数据库上下文</param>
    /// <param name="userClient">用户客户端</param>
    /// <param name="userGameRoleClient">角色客户端</param>
    /// <param name="authClient">验证客户端</param>
    /// <param name="messenger">消息器</param>
    public UserService(
        AppDbContext appDbContext,
        UserClient userClient,
        BindingClient userGameRoleClient,
        AuthClient authClient,
        IMessenger messenger)
    {
        this.appDbContext = appDbContext;
        this.userClient = userClient;
        this.userGameRoleClient = userGameRoleClient;
        this.authClient = authClient;
        this.messenger = messenger;
    }

    /// <inheritdoc/>
    public BindingUser? CurrentUser
    {
        get => currentUser;
        set
        {
            if (currentUser == value)
            {
                return;
            }

            // only update when not processing a deletion
            if (value != null)
            {
                if (currentUser != null)
                {
                    currentUser.IsSelected = false;
                    appDbContext.Users.Update(currentUser.Entity);
                    appDbContext.SaveChanges();
                }
            }

            Message.UserChangedMessage message = new(currentUser, value);

            // 当删除到无用户时也能正常反应状态
            currentUser = value;

            if (currentUser != null)
            {
                currentUser.IsSelected = true;
                appDbContext.Users.Update(currentUser.Entity);
                appDbContext.SaveChanges();
            }

            messenger.Send(message);
        }
    }

    /// <inheritdoc/>
    public async Task<UserAddResult> TryAddUserAsync(BindingUser newUser, string uid)
    {
        Must.NotNull(userCollection!);

        // 查找是否有相同的uid
        if (userCollection.SingleOrDefault(u => u.UserInfo!.Uid == uid) is BindingUser userWithSameUid)
        {
            // Prevent users from adding a completely same cookie.
            if (userWithSameUid.Cookie == newUser.Cookie)
            {
                return UserAddResult.AlreadyExists;
            }
            else
            {
                // Update user cookie here.
                userWithSameUid.Cookie = newUser.Cookie;
                appDbContext.Users.Update(userWithSameUid.Entity);
                await appDbContext.SaveChangesAsync().ConfigureAwait(false);

                return UserAddResult.Updated;
            }
        }
        else
        {
            Verify.Operation(newUser.IsInitialized, "该用户尚未初始化");

            // Sync cache
            await ThreadHelper.SwitchToMainThreadAsync();
            userCollection.Add(newUser);

            // Sync database
            appDbContext.Users.Add(newUser.Entity);
            await appDbContext.SaveChangesAsync().ConfigureAwait(false);

            return UserAddResult.Added;
        }
    }

    /// <inheritdoc/>
    public Task RemoveUserAsync(BindingUser user)
    {
        Must.NotNull(userCollection!);

        // Sync cache
        userCollection.Remove(user);

        // Sync database
        appDbContext.Users.Remove(user.Entity);
        return appDbContext.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task<ObservableCollection<BindingUser>> GetUserCollectionAsync()
    {
        if (userCollection == null)
        {
            List<BindingUser> users = new();

            foreach (Model.Entity.User entity in appDbContext.Users)
            {
                BindingUser? initialized = await BindingUser
                    .ResumeAsync(entity, userClient, userGameRoleClient)
                    .ConfigureAwait(false);

                if (initialized != null)
                {
                    users.Add(initialized);
                }
                else
                {
                    // User is unable to be initialized, remove it.
                    appDbContext.Users.Remove(entity);
                    await appDbContext.SaveChangesAsync().ConfigureAwait(false);
                }
            }

            userCollection = new(users);
            CurrentUser = users.SingleOrDefault(user => user.IsSelected);
        }

        return userCollection;
    }

    /// <inheritdoc/>
    public Task<BindingUser?> CreateUserAsync(IDictionary<string, string> cookie)
    {
        return BindingUser.CreateAsync(cookie, userClient, userGameRoleClient, authClient);
    }

    /// <inheritdoc/>
    public async Task<ValueResult<bool, string>> TryUpgradeUserByLoginTicketAsync(IDictionary<string, string> addition, CancellationToken token = default)
    {
        Must.NotNull(userCollection!);
        if (addition.TryGetValue(CookieKeys.LOGIN_UID, out string? uid))
        {
            // 查找是否有相同的uid
            if (userCollection.SingleOrDefault(u => u.UserInfo!.Uid == uid) is BindingUser userWithSameUid)
            {
                // Update user cookie here.
                if (await userWithSameUid.TryUpgradeByLoginTicketAsync(addition, authClient, token))
                {
                    appDbContext.Users.Update(userWithSameUid.Entity);
                    await appDbContext.SaveChangesAsync().ConfigureAwait(false);
                    return new(true, userWithSameUid.UserInfo?.Nickname ?? string.Empty);
                }
            }
        }

        return new(false, string.Empty);
    }

    /// <inheritdoc/>
    public async Task<ValueResult<bool, string>> TryUpgradeUserByStokenAsync(IDictionary<string, string> stoken)
    {
        Must.NotNull(userCollection!);
        if (stoken.TryGetValue(CookieKeys.STUID, out string? uid))
        {
            // 查找是否有相同的uid
            if (userCollection.SingleOrDefault(u => u.UserInfo!.Uid == uid) is BindingUser userWithSameUid)
            {
                // Update user cookie here.
                userWithSameUid.AddStoken(stoken);

                appDbContext.Users.Update(userWithSameUid.Entity);
                await appDbContext.SaveChangesAsync().ConfigureAwait(false);
                return new(true, userWithSameUid.UserInfo?.Nickname ?? string.Empty);
            }
        }

        return new(false, string.Empty);
    }
}