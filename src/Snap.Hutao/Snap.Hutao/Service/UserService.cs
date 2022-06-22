// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Web.Hoyolab.Bbs.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Snap.Hutao.Service;

/// <summary>
/// 用户服务
/// </summary>
[Injection(InjectAs.Transient, typeof(IUserService))]
internal class UserService : IUserService
{
    private readonly AppDbContext appDbContext;
    private readonly UserClient userClient;
    private readonly UserGameRoleClient userGameRoleClient;

    private User? currentUser;
    private ObservableCollection<User>? cachedUser = null;

    /// <summary>
    /// 构造一个新的用户服务
    /// </summary>
    /// <param name="appDbContext">应用程序数据库上下文</param>
    /// <param name="userClient">用户客户端</param>
    /// <param name="userGameRoleClient">角色客户端</param>
    public UserService(AppDbContext appDbContext, UserClient userClient, UserGameRoleClient userGameRoleClient)
    {
        this.appDbContext = appDbContext;
        this.userClient = userClient;
        this.userGameRoleClient = userGameRoleClient;
    }

    /// <inheritdoc/>
    public User? CurrentUser
    {
        get => currentUser;
        set
        {
            // only update when not processing a deletion
            if (!User.IsNone(value))
            {
                if (!User.IsNone(currentUser))
                {
                    User.SetSelectionState(currentUser, false);
                    appDbContext.Users.Update(currentUser);
                }
            }

            // 当删除到无用户时也能正常反应状态
            currentUser = value;

            if (!User.IsNone(currentUser))
            {
                User.SetSelectionState(currentUser, true);
                appDbContext.Users.Update(currentUser);
            }
        }
    }

    /// <inheritdoc/>
    public async Task<bool> TryAddUserAsync(User user)
    {
        if (await user.InitializeAsync(userClient, userGameRoleClient))
        {
            appDbContext.Users.Add(user);
            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public void RemoveUser(User user)
    {
        appDbContext.Users.Remove(user);
    }

    /// <inheritdoc/>
    public async Task<ObservableCollection<User>> GetInitializedUsersAsync(ICommand removeCommand)
    {
        if (cachedUser == null)
        {
            appDbContext.Users.Load();
            cachedUser = appDbContext.Users.Local.ToObservableCollection();

            foreach (User user in cachedUser)
            {
                user.RemoveCommand = removeCommand;
                await user.InitializeAsync(userClient, userGameRoleClient);
            }

            CurrentUser = await appDbContext.Users.SingleOrDefaultAsync(user => user.IsSelected);
        }

        return cachedUser;
    }

    /// <inheritdoc/>
    public IDictionary<string, string> ParseCookie(string cookie)
    {
        Dictionary<string, string> cookieDictionary = new();

        string[] values = cookie.TrimEnd(';').Split(';');
        foreach (string[] parts in values.Select(c => c.Split(new[] { '=' }, 2)))
        {
            string cookieName = parts[0].Trim();
            string cookieValue = parts.Length == 1 ? string.Empty : parts[1].Trim();

            cookieDictionary[cookieName] = cookieValue;
        }

        return cookieDictionary;
    }
}