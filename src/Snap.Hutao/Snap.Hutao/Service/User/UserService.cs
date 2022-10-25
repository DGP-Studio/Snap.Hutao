// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Core.Threading;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Bbs.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Auth;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using System.Collections.ObjectModel;
using BindingUser = Snap.Hutao.Model.Binding.User;

namespace Snap.Hutao.Service.User;

/// <summary>
/// 用户服务
/// 主要负责将用户数据与数据库同步
/// </summary>
[Injection(InjectAs.Singleton, typeof(IUserService))]
internal class UserService : IUserService
{
    private readonly AppDbContext appDbContext;
    private readonly UserClient userClient;
    private readonly BindingClient bindingClient;
    private readonly AuthClient authClient;
    private readonly IMessenger messenger;

    private BindingUser? currentUser;
    private ObservableCollection<BindingUser>? userCollection;

    /// <summary>
    /// 构造一个新的用户服务
    /// </summary>
    /// <param name="appDbContext">应用程序数据库上下文</param>
    /// <param name="userClient">用户客户端</param>
    /// <param name="bindingClient">角色客户端</param>
    /// <param name="authClient">验证客户端</param>
    /// <param name="messenger">消息器</param>
    public UserService(
        AppDbContext appDbContext,
        UserClient userClient,
        BindingClient bindingClient,
        AuthClient authClient,
        IMessenger messenger)
    {
        this.appDbContext = appDbContext;
        this.userClient = userClient;
        this.bindingClient = bindingClient;
        this.authClient = authClient;
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

            Message.UserChangedMessage message = new() { OldValue = currentUser, NewValue = value };

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
                    .ResumeAsync(entity, userClient, bindingClient)
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
            Current = users.SingleOrDefault(user => user.IsSelected);
        }

        return userCollection;
    }

    /// <inheritdoc/>
    public async Task<ValueResult<UserOptionResult, string>> ProcessInputCookieAsync(Cookie cookie)
    {
        cookie.Trim();
        Must.NotNull(userCollection!);

        // 检查 uid 是否存在
        if (cookie.TryGetUid(out string? uid))
        {
            // 检查 login ticket 是否存在
            // 若存在则尝试升级至 stoken
            await TryAddMultiTokenAsync(cookie, uid).ConfigureAwait(false);

            // 检查 uid 对应用户是否存在
            if (UserHelper.TryGetUserByUid(userCollection, uid, out BindingUser? userWithSameUid))
            {
                // 检查 stoken 是否存在
                if (cookie.ContainsSToken())
                {
                    // insert stoken
                    userWithSameUid.UpdateSToken(uid, cookie);
                    appDbContext.Users.Update(userWithSameUid.Entity);
                    appDbContext.SaveChanges();

                    return new(UserOptionResult.Upgraded, uid);
                }

                if (cookie.ContainsLTokenAndCookieToken())
                {
                    userWithSameUid.Cookie = cookie;
                    appDbContext.Users.Update(userWithSameUid.Entity);
                    appDbContext.SaveChanges();

                    return new(UserOptionResult.Updated, uid);
                }
            }
            else if (cookie.ContainsLTokenAndCookieToken())
            {
                return await TryCreateUserAndAddAsync(userCollection, cookie).ConfigureAwait(false);
            }
        }

        return new(UserOptionResult.Incomplete, null!);
    }

    private async Task TryAddMultiTokenAsync(Cookie cookie, string uid)
    {
        if (cookie.TryGetLoginTicket(out string? loginTicket))
        {
            // get multitoken
            Dictionary<string, string> multiToken = await authClient
                .GetMultiTokenByLoginTicketAsync(loginTicket, uid, default)
                .ConfigureAwait(false);

            if (multiToken.Count >= 2)
            {
                cookie.InsertMultiToken(uid, multiToken);
                cookie.RemoveLoginTicket();
            }
        }
    }

    private async Task<ValueResult<UserOptionResult, string>> TryCreateUserAndAddAsync(ObservableCollection<BindingUser> users, Cookie cookie)
    {
        BindingUser? newUser = await BindingUser.CreateAsync(cookie, userClient, bindingClient).ConfigureAwait(false);
        if (newUser != null)
        {
            // Sync cache
            await ThreadHelper.SwitchToMainThreadAsync();
            users.Add(newUser);

            // Sync database
            appDbContext.Users.Add(newUser.Entity);
            await appDbContext.SaveChangesAsync().ConfigureAwait(false);

            return new(UserOptionResult.Added, newUser.UserInfo!.Uid);
        }
        else
        {
            return new(UserOptionResult.Invalid, null!);
        }
    }
}