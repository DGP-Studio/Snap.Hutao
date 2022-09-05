// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Model.Binding;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Web.Hoyolab.Bbs.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using System.Collections.ObjectModel;

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
    private readonly UserGameRoleClient userGameRoleClient;
    private readonly IMessenger messenger;

    private User? currentUser;
    private ObservableCollection<User>? userCollection = null;

    /// <summary>
    /// 构造一个新的用户服务
    /// </summary>
    /// <param name="appDbContext">应用程序数据库上下文</param>
    /// <param name="userClient">用户客户端</param>
    /// <param name="userGameRoleClient">角色客户端</param>
    /// <param name="messenger">消息器</param>
    public UserService(AppDbContext appDbContext, UserClient userClient, UserGameRoleClient userGameRoleClient, IMessenger messenger)
    {
        this.appDbContext = appDbContext;
        this.userClient = userClient;
        this.userGameRoleClient = userGameRoleClient;
        this.messenger = messenger;
    }

    /// <inheritdoc/>
    public User? CurrentUser
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
    public async Task<UserAddResult> TryAddUserAsync(User newUser, string uid)
    {
        Must.NotNull(userCollection!);

        // 查找是否有相同的uid
        if (userCollection.SingleOrDefault(u => u.UserInfo!.Uid == uid) is User userWithSameUid)
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
            userCollection.Add(newUser);

            // Sync database
            appDbContext.Users.Add(newUser.Entity);
            await appDbContext.SaveChangesAsync().ConfigureAwait(false);

            return UserAddResult.Added;
        }
    }

    /// <inheritdoc/>
    public Task RemoveUserAsync(User user)
    {
        Must.NotNull(userCollection!);

        // Sync cache
        userCollection.Remove(user);

        // Sync database
        appDbContext.Users.Remove(user.Entity);
        return appDbContext.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task<ObservableCollection<User>> GetUserCollectionAsync()
    {
        if (userCollection == null)
        {
            List<User> users = new();

            foreach (Model.Entity.User entity in appDbContext.Users)
            {
                User? initialized = await User
                    .CreateAsync(entity, userClient, userGameRoleClient)
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
    public Task<User?> CreateUserAsync(string cookie)
    {
        return User.CreateAsync(Model.Entity.User.Create(cookie), userClient, userGameRoleClient);
    }

    /// <inheritdoc/>
    public IDictionary<string, string> ParseCookie(string cookie)
    {
        SortedDictionary<string, string> cookieDictionary = new();

        string[] values = cookie.TrimEnd(';').Split(';');
        foreach (string[] parts in values.Select(c => c.Split('=', 2)))
        {
            string cookieName = parts[0].Trim();
            string cookieValue = parts.Length == 1 ? string.Empty : parts[1].Trim();

            cookieDictionary.Add(cookieName, cookieValue);
        }

        return cookieDictionary;
    }
}