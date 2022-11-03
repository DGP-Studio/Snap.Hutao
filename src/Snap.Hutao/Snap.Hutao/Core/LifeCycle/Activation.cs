// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core.Threading;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Navigation;
using System.Security.Principal;

namespace Snap.Hutao.Core.LifeCycle;

/// <summary>
/// 激活处理器
/// </summary>
internal static class Activation
{
    /// <summary>
    /// 启动游戏启动参数
    /// </summary>
    public const string LaunchGame = "LaunchGame";

    private static readonly SemaphoreSlim ActivateSemaphore = new(1);

    /// <summary>
    /// 获取是否提升了权限
    /// </summary>
    /// <returns>是否提升了权限</returns>
    public static bool GetElevated()
    {
        using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
        {
            WindowsPrincipal principal = new(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }

    /// <summary>
    /// 响应激活事件
    /// 激活事件一般不会在UI线程上触发
    /// </summary>
    /// <param name="sender">发送方</param>
    /// <param name="args">激活参数</param>
    public static void Activate(object? sender, AppActivationArguments args)
    {
        _ = sender;
        HandleActivationAsync(args).SafeForget();
    }

    /// <summary>
    /// 异步响应激活事件
    /// </summary>
    /// <returns>任务</returns>
    private static async Task HandleActivationAsync(AppActivationArguments args)
    {
        if (ActivateSemaphore.CurrentCount > 0)
        {
            using (await ActivateSemaphore.EnterAsync().ConfigureAwait(false))
            {
                await HandleActivationCoreAsync(args).ConfigureAwait(false);
            }
        }
    }

    private static async Task HandleActivationCoreAsync(AppActivationArguments args)
    {
        string argument = string.Empty;

        if (args.Kind == ExtendedActivationKind.Launch)
        {
            if (args.TryGetLaunchActivatedArgument(out string? arguments))
            {
                argument = arguments;
            }
        }

        switch (argument)
        {
            case "":
                {
                    _ = Ioc.Default.GetRequiredService<MainWindow>();
                    await Ioc.Default.GetRequiredService<IInfoBarService>().WaitInitializationAsync().ConfigureAwait(false);
                    break;
                }

            case LaunchGame:
                {
                    await ThreadHelper.SwitchToMainThreadAsync();
                    if (!MainWindow.IsPresent)
                    {
                        _ = Ioc.Default.GetRequiredService<LaunchGameWindow>();
                    }
                    else
                    {
                        await Ioc.Default
                            .GetRequiredService<INavigationService>()
                            .NavigateAsync<View.Page.LaunchGamePage>(INavigationAwaiter.Default, true).ConfigureAwait(false);
                    }

                    break;
                }
        }

        if (args.Kind == ExtendedActivationKind.Protocol)
        {
            if (args.TryGetProtocolActivatedUri(out Uri? uri))
            {
                Ioc.Default.GetRequiredService<IInfoBarService>().Information(uri.ToString());
                await HandleUrlActivationAsync(uri).ConfigureAwait(false);
            }
        }
    }

    private static async Task HandleUrlActivationAsync(Uri uri)
    {
        UriBuilder builder = new(uri);
        Must.Argument(builder.Scheme == "hutao", "uri 的协议不正确");

        string category = builder.Host.ToLowerInvariant();
        string action = builder.Path.ToLowerInvariant();
        string rawParameter = builder.Query.ToLowerInvariant();

        switch (category)
        {
            case "achievement":
                {
                    await HandleAchievementActionAsync(action, rawParameter).ConfigureAwait(false);
                    break;
                }
        }
    }

    private static async Task HandleAchievementActionAsync(string action, string parameter)
    {
        switch (action)
        {
            case "/import":
                {
                    await ThreadHelper.SwitchToMainThreadAsync();

                    INavigationAwaiter navigationAwaiter = new NavigationExtra("InvokeByUri");
                    await Ioc.Default
                        .GetRequiredService<INavigationService>()
                        .NavigateAsync<View.Page.AchievementPage>(navigationAwaiter, true)
                        .ConfigureAwait(false);
                    break;
                }
        }
    }
}