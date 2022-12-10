// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Extension;
using Snap.Hutao.Message;
using Snap.Hutao.Web.Hoyolab;
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
    private readonly object currentUserLocker = new();

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

            lock (currentUserLocker)
            {
                using (IServiceScope scope = scopeFactory.CreateScope())
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
                        appDbContext.Users.UpdateAndSave(currentUser.Entity);
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
        await ThreadHelper.SwitchToMainThreadAsync();
        userCollection!.Remove(user);
        roleCollection?.RemoveWhere(r => r.User.InnerId == user.Entity.InnerId);

        // Sync database
        await ThreadHelper.SwitchToBackgroundAsync();
        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            // Note: cascade deleted dailynotes
            await scope.ServiceProvider.GetRequiredService<AppDbContext>().Users.RemoveAndSaveAsync(user.Entity).ConfigureAwait(false);
        }

        messenger.Send(new UserRemovedMessage(user.Entity));
    }

    /// <inheritdoc/>
    public async Task<ObservableCollection<BindingUser>> GetUserCollectionAsync()
    {
        await ThreadHelper.SwitchToBackgroundAsync();
        if (userCollection == null)
        {
            List<BindingUser> users = new();

            using (IServiceScope scope = scopeFactory.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                foreach (Model.Entity.User entity in appDbContext.Users)
                {
                    BindingUser? initialized = await BindingUser.ResumeAsync(entity).ConfigureAwait(false);

                    if (initialized != null)
                    {
                        users.Add(initialized);
                    }
                    else
                    {
                        // User is unable to be initialized, remove it.
                        await appDbContext.Users.RemoveAndSaveAsync(entity).ConfigureAwait(false);
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
        await ThreadHelper.SwitchToBackgroundAsync();
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
            // System.InvalidOperationException: Sequence contains no matching element
            // Not quiet sure why this happen when its Single()
            return roleCollection.SingleOrDefault(r => r.Role.GameUid == uid)?.Role;
        }

        return null;
    }

    /// <inheritdoc/>
    public async Task<ValueResult<UserOptionResult, string>> ProcessInputCookieAsync(Cookie cookie)
    {
        await ThreadHelper.SwitchToBackgroundAsync();
        string? mid = cookie.GetValueOrDefault(Cookie.MID);

        if (mid == null)
        {
            return new(UserOptionResult.Invalid, "输入的Cookie无法获取用户信息");
        }

        // 检查 mid 对应用户是否存在
        if (UserHelper.TryGetUser(userCollection!, mid, out BindingUser? user))
        {
            using (IServiceScope scope = scopeFactory.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                if (cookie.TryGetAsStoken(out Cookie? stoken))
                {
                    user.Stoken = stoken;
                    user.Ltoken = cookie.TryGetAsLtoken(out Cookie? ltoken) ? ltoken : user.Ltoken;
                    user.CookieToken = cookie.TryGetAsCookieToken(out Cookie? cookieToken) ? cookieToken : user.CookieToken;

                    await appDbContext.Users.UpdateAndSaveAsync(user.Entity).ConfigureAwait(false);
                    return new(UserOptionResult.Updated, mid);
                }
                else
                {
                    return new(UserOptionResult.Invalid, "必须包含 Stoken");
                }
            }
        }
        else
        {
            return await TryCreateUserAndAddAsync(cookie).ConfigureAwait(false);
        }
    }

    private async Task<ValueResult<UserOptionResult, string>> TryCreateUserAndAddAsync(Cookie cookie)
    {
        await ThreadHelper.SwitchToBackgroundAsync();
        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            BindingUser? newUser = await BindingUser.CreateAsync(cookie).ConfigureAwait(false);
            if (newUser != null)
            {
                // Sync cache
                if (userCollection != null)
                {
                    await ThreadHelper.SwitchToMainThreadAsync();
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
                await ThreadHelper.SwitchToBackgroundAsync();
                await appDbContext.Users.AddAndSaveAsync(newUser.Entity).ConfigureAwait(false);
                return new(UserOptionResult.Added, newUser.UserInfo!.Uid);
            }
            else
            {
                return new(UserOptionResult.Invalid, "输入的 Cookie 无法获取用户信息");
            }
        }
    }
}