// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.BbsSignReward;
using Snap.Hutao.Web.Response;
using System.Collections.Generic;

namespace Snap.Hutao.Service.Sign;

/// <summary>
/// 签到服务
/// </summary>
[Injection(InjectAs.Transient, typeof(ISignService))]
internal class SignService : ISignService
{
    private readonly IUserService userService;
    private readonly IInfoBarService infoBarService;
    private readonly SignClient signClient;

    /// <summary>
    /// 构造一个新的签到服务
    /// </summary>
    /// <param name="userService">用户服务</param>
    /// <param name="infoBarService">信息条服务</param>
    /// <param name="signClient">签到客户端</param>
    public SignService(IUserService userService, IInfoBarService infoBarService, SignClient signClient)
    {
        this.userService = userService;
        this.infoBarService = infoBarService;
        this.signClient = signClient;
    }

    /// <inheritdoc/>
    public async Task<SignResult> SignForAllAsync(CancellationToken token)
    {
        IEnumerable<Model.Binding.User>? users = await userService
            .GetUserCollectionAsync()
            .ConfigureAwait(false);
        Queue<UserRole> userRolesQueue = GetSignQueue(users);

        int totalCount = 0;
        int retryCount = 0;
        ValueStopwatch stopwatch = ValueStopwatch.StartNew();

        while (userRolesQueue.TryDequeue(out UserRole current))
        {
            totalCount++;
            Response<SignInResult>? resp = await signClient
                .SignAsync(current, token)
                .ConfigureAwait(false);

            Must.NotNull(resp!);

            if (resp.Data != null)
            {
                Must.Argument(resp.ReturnCode == 0, "返回代码应为 0");

                // Geetest applied
                if (resp.Data.Success != 0)
                {
                    userRolesQueue.Enqueue(current);
                    retryCount++;
                }
                else
                {
                    infoBarService.Information($"[{current.Role}] 签到成功");
                }
            }
            else
            {
                switch ((KnownReturnCode)resp.ReturnCode)
                {
                    case KnownReturnCode.OK:
                        infoBarService.Information($"[{current.Role}] 签到成功");
                        break;
                    case KnownReturnCode.NotLoggedIn:
                    case KnownReturnCode.AlreadySignedIn:
                        infoBarService.Information($"[{current.Role}] {resp.Message}");
                        break;
                    case KnownReturnCode.InvalidRequest:
                        infoBarService.Information("米游社SALT过期，请更新胡桃");
                        break;
                    default:
                        throw Must.NeverHappen();
                }
            }

            if (userRolesQueue.Count > 0)
            {
                int seconds = Random.Shared.Next(5, 15);
                await Task
                    .Delay(TimeSpan.FromSeconds(seconds), token)
                    .ConfigureAwait(false);
            }
        }

        return new(totalCount, retryCount, stopwatch.GetElapsedTime());
    }

    private static Queue<UserRole> GetSignQueue(IEnumerable<Model.Binding.User> users)
    {
        Queue<UserRole> queue = new();

        foreach (Model.Binding.User user in users)
        {
            foreach (UserGameRole role in user.UserGameRoles)
            {
                queue.Enqueue(new(user, role));
            }
        }

        return queue;
    }
}