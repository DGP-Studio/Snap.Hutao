// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Notifications;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.DailyNote;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Navigation;
using System.Diagnostics;
using System.Security.Principal;
using Windows.ApplicationModel;

namespace Snap.Hutao.Core.LifeCycle;

/// <summary>
/// 激活处理器
/// </summary>
[HighQuality]
internal static class Activation
{
    // TODO: make this class a dependency

    /// <summary>
    /// 操作
    /// </summary>
    public const string Action = nameof(Action);

    /// <summary>
    /// Uid
    /// </summary>
    public const string Uid = nameof(Uid);

    /// <summary>
    /// 启动游戏启动参数
    /// </summary>
    public const string LaunchGame = nameof(LaunchGame);

    /// <summary>
    /// 从剪贴板导入成就
    /// </summary>
    public const string ImportUIAFFromClipboard = nameof(ImportUIAFFromClipboard);

    private const string CategoryAchievement = "achievement";
    private const string CategoryDailyNote = "dailynote";
    private const string UrlActionImport = "/import";
    private const string UrlActionRefresh = "/refresh";

    private static readonly WeakReference<MainWindow> MainWindowReference = new(default!);
    private static readonly SemaphoreSlim ActivateSemaphore = new(1);

    /// <summary>
    /// 获取是否提升了权限
    /// </summary>
    /// <returns>是否提升了权限</returns>
    public static bool GetElevated()
    {
        if (Debugger.IsAttached)
        {
            return true;
        }

        using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
        {
            WindowsPrincipal principal = new(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }

    /// <summary>
    /// 以管理员模式重启
    /// </summary>
    /// <returns>任务</returns>
    public static async ValueTask RestartAsElevatedAsync()
    {
        if (!GetElevated())
        {
            await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            Process.GetCurrentProcess().Kill();
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
        if (!ToastNotificationManagerCompat.WasCurrentProcessToastActivated())
        {
            HandleActivationAsync(args, true).SafeForget();
        }
    }

    /// <summary>
    /// 触发激活事件
    /// </summary>
    /// <param name="sender">发送方</param>
    /// <param name="args">激活参数</param>
    public static void NonRedirectToActivate(object? sender, AppActivationArguments args)
    {
        _ = sender;
        if (!ToastNotificationManagerCompat.WasCurrentProcessToastActivated())
        {
            HandleActivationAsync(args, false).SafeForget();
        }
    }

    /// <summary>
    /// 响应通知激活事件
    /// </summary>
    /// <param name="args">参数</param>
    public static void NotificationActivate(ToastNotificationActivatedEventArgsCompat args)
    {
        ToastArguments toastArgs = ToastArguments.Parse(args.Argument);

        if (toastArgs.TryGetValue(Action, out string? action))
        {
            if (action == LaunchGame)
            {
                _ = toastArgs.TryGetValue(Uid, out string? uid);
                HandleLaunchGameActionAsync(uid).SafeForget();
            }
        }
    }

    /// <summary>
    /// 异步响应激活事件
    /// </summary>
    /// <returns>任务</returns>
    private static async Task HandleActivationAsync(AppActivationArguments args, bool isRedirected)
    {
        if (ActivateSemaphore.CurrentCount > 0)
        {
            using (await ActivateSemaphore.EnterAsync().ConfigureAwait(false))
            {
                await HandleActivationCoreAsync(args, isRedirected).ConfigureAwait(false);
            }
        }
    }

    private static async Task HandleActivationCoreAsync(AppActivationArguments args, bool isRedirected)
    {
        if (args.Kind == ExtendedActivationKind.Protocol)
        {
            if (args.TryGetProtocolActivatedUri(out Uri? uri))
            {
                await HandleUrlActivationAsync(uri, isRedirected).ConfigureAwait(false);
            }
        }
        else if (args.Kind == ExtendedActivationKind.Launch)
        {
            if (args.TryGetLaunchActivatedArgument(out string? arguments))
            {
                switch (arguments)
                {
                    case LaunchGame:
                        {
                            await HandleLaunchGameActionAsync().ConfigureAwait(false);
                            break;
                        }

                    default:
                        {
                            await HandleNormalLaunchActionAsync().ConfigureAwait(false);
                            break;
                        }
                }
            }
        }
    }

    private static async Task HandleNormalLaunchActionAsync()
    {
        // Increase launch times
        LocalSetting.Set(SettingKeys.LaunchTimes, LocalSetting.Get(SettingKeys.LaunchTimes, 0) + 1);

        await WaitMainWindowAsync().ConfigureAwait(false);
    }

    private static async Task WaitMainWindowAsync()
    {
        IServiceProvider serviceProvider = Ioc.Default;
        ITaskContext taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        await taskContext.SwitchToMainThreadAsync();

        MainWindowReference.SetTarget(serviceProvider.GetRequiredService<MainWindow>());

        await serviceProvider
            .GetRequiredService<IInfoBarService>()
            .WaitInitializationAsync()
            .ConfigureAwait(false);

        serviceProvider
            .GetRequiredService<IMetadataService>()
            .As<IMetadataServiceInitialization>()?
            .InitializeInternalAsync()
            .SafeForget();

        serviceProvider
            .GetRequiredService<IHutaoUserService>()
            .As<IHutaoUserServiceInitialization>()?
            .InitializeInternalAsync()
            .SafeForget();
    }

    private static async Task HandleUrlActivationAsync(Uri uri, bool isRedirected)
    {
        UriBuilder builder = new(uri);

        string category = builder.Host.ToLowerInvariant();
        string action = builder.Path.ToLowerInvariant();
        string parameter = builder.Query.ToLowerInvariant();

        switch (category)
        {
            case CategoryAchievement:
                {
                    await WaitMainWindowAsync().ConfigureAwait(false);
                    await HandleAchievementActionAsync(action, parameter, isRedirected).ConfigureAwait(false);
                    break;
                }

            case CategoryDailyNote:
                {
                    await HandleDailyNoteActionAsync(action, parameter, isRedirected).ConfigureAwait(false);
                    break;
                }

            default:
                {
                    await HandleNormalLaunchActionAsync().ConfigureAwait(false);
                    break;
                }
        }
    }

    private static async Task HandleAchievementActionAsync(string action, string parameter, bool isRedirected)
    {
        _ = parameter;
        _ = isRedirected;
        switch (action)
        {
            case UrlActionImport:
                {
                    ITaskContext taskContext = Ioc.Default.GetRequiredService<ITaskContext>();
                    await taskContext.SwitchToMainThreadAsync();

                    INavigationAwaiter navigationAwaiter = new NavigationExtra(ImportUIAFFromClipboard);
                    await Ioc.Default
                        .GetRequiredService<INavigationService>()
                        .NavigateAsync<View.Page.AchievementPage>(navigationAwaiter, true)
                        .ConfigureAwait(false);
                    break;
                }
        }
    }

    private static async Task HandleDailyNoteActionAsync(string action, string parameter, bool isRedirected)
    {
        _ = parameter;
        switch (action)
        {
            case UrlActionRefresh:
                {
                    await Ioc.Default
                        .GetRequiredService<IDailyNoteService>()
                        .RefreshDailyNotesAsync()
                        .ConfigureAwait(false);

                    // Check if it's redirected.
                    if (!isRedirected)
                    {
                        // It's a direct open process, should exit immediately.
                        Process.GetCurrentProcess().Kill();
                    }

                    break;
                }
        }
    }

    private static async Task HandleLaunchGameActionAsync(string? uid = null)
    {
        IServiceProvider serviceProvider = Ioc.Default;
        IMemoryCache memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
        memoryCache.Set(ViewModel.Game.LaunchGameViewModel.DesiredUid, uid);
        ITaskContext taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        await taskContext.SwitchToMainThreadAsync();

        if (!MainWindowReference.TryGetTarget(out _))
        {
            _ = serviceProvider.GetRequiredService<LaunchGameWindow>();
        }
        else
        {
            await serviceProvider
                .GetRequiredService<INavigationService>()
                .NavigateAsync<View.Page.LaunchGamePage>(INavigationAwaiter.Default, true)
                .ConfigureAwait(false);
        }
    }
}