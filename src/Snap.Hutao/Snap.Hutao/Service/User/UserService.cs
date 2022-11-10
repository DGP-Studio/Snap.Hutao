// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Extension;
using Snap.Hutao.Message;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Bbs.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using System.Collections.ObjectModel;
using BindingUser = Snap.Hutao.Model.Binding.User.User;

namespace Snap.Hutao.Service.User;

/// <summary>
/// 用户服务
/// 主要负责将用户数据与数据库同步
/// </summary>
[Injection(InjectAs.Singleton, typeof(IUserService))]
internal class UserService : IUserService
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IMessenger messenger;

    private BindingUser? currentUser;
    private ObservableCollection<BindingUser>? userCollection;
    private ObservableCollection<Model.Binding.User.UserAndRole>? roleCollection;

    /// <summary>
    /// 构造一个新的用户服务
    /// </summary>
    /// <param name="scopeFactory">范围工厂</param>
    /// <param name="messenger">消息器</param>
    public UserService(IServiceScopeFactory scopeFactory, IMessenger messenger)
    {
        this.scopeFactory = scopeFactory;
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

            using (IServiceScope scope = scopeFactory.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

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
    }

    /// <inheritdoc/>
    public async Task RemoveUserAsync(BindingUser user)
    {
        await Task.Yield();

        // Sync cache
        userCollection!.Remove(user);
        roleCollection!.RemoveWhere(r => r.User.InnerId == user.Entity.InnerId);

        // Sync database
        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Note: cascade deleted dailynotes
            appDbContext.Users.RemoveAndSave(user.Entity);
        }

        messenger.Send(new UserRemovedMessage(user.Entity));
    }

    /// <inheritdoc/>
    public async Task<ObservableCollection<BindingUser>> GetUserCollectionAsync()
    {
        if (userCollection == null)
        {
            List<BindingUser> users = new();

            using (IServiceScope scope = scopeFactory.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                UserClient userClient = scope.ServiceProvider.GetRequiredService<UserClient>();
                BindingClient bindingClient = scope.ServiceProvider.GetRequiredService<BindingClient>();

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
                        appDbContext.Users.RemoveAndSave(entity);
                    }
                }
            }

            userCollection = new(users);
            Current = users.SingleOrDefault(user => user.IsSelected);
        }

        return userCollection;
    }

    /// <inheritdoc/>
    public async Task<ObservableCollection<Model.Binding.User.UserAndRole>> GetRoleCollectionAsync()
    {
        if (roleCollection == null)
        {
            List<Model.Binding.User.UserAndRole> userAndRoles = new();
            ObservableCollection<BindingUser> observableUsers = await GetUserCollectionAsync().ConfigureAwait(false);
            foreach (BindingUser user in observableUsers.ToList())
            {
                foreach (UserGameRole role in user.UserGameRoles)
                {
                    userAndRoles.Add(new(user.Entity, role));
                }
            }

            roleCollection = new(userAndRoles);
        }

        return roleCollection;
    }

    /// <inheritdoc/>
    public UserGameRole? GetUserGameRoleByUid(string uid)
    {
        if (roleCollection != null)
        {
            return roleCollection.Single(r => r.Role.GameUid == uid).Role;
        }

        return null;
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
            await cookie.TryAddMultiTokenAsync(uid).ConfigureAwait(false);

            // 检查 uid 对应用户是否存在
            if (UserHelper.TryGetUserByUid(userCollection, uid, out BindingUser? userWithSameUid))
            {
                using (IServiceScope scope = scopeFactory.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    // 检查 stoken 是否存在
                    if (cookie.ContainsSToken())
                    {
                        // insert stoken
                        await ThreadHelper.SwitchToMainThreadAsync();
                        userWithSameUid.UpdateSToken(uid, cookie);
                        appDbContext.Users.UpdateAndSave(userWithSameUid.Entity);

                        return new(UserOptionResult.Upgraded, uid);
                    }

                    if (cookie.ContainsLTokenAndCookieToken())
                    {
                        await ThreadHelper.SwitchToMainThreadAsync();
                        userWithSameUid.Cookie = cookie;
                        appDbContext.Users.UpdateAndSave(userWithSameUid.Entity);

                        return new(UserOptionResult.Updated, uid);
                    }
                }
            }
            else if (cookie.ContainsLTokenAndCookieToken())
            {
                return await TryCreateUserAndAddAsync(cookie).ConfigureAwait(false);
            }
        }

        return new(UserOptionResult.Incomplete, null!);
    }

    private async Task<ValueResult<UserOptionResult, string>> TryCreateUserAndAddAsync(Cookie cookie)
    {
        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            UserClient userClient = scope.ServiceProvider.GetRequiredService<UserClient>();
            BindingClient bindingClient = scope.ServiceProvider.GetRequiredService<BindingClient>();

            BindingUser? newUser = await BindingUser.CreateAsync(cookie, userClient, bindingClient).ConfigureAwait(false);
            if (newUser != null)
            {
                // Sync cache
                if (userCollection != null)
                {
                    await ThreadHelper.SwitchToMainThreadAsync();
                    userCollection!.Add(newUser);

                    if (roleCollection != null)
                    {
                        foreach (UserGameRole role in newUser.UserGameRoles)
                        {
                            roleCollection.Add(new(newUser.Entity, role));
                        }
                    }
                }

                // Sync database
                appDbContext.Users.AddAndSave(newUser.Entity);
                return new(UserOptionResult.Added, newUser.UserInfo!.Uid);
            }
            else
            {
                return new(UserOptionResult.Invalid, null!);
            }
        }
    }
}