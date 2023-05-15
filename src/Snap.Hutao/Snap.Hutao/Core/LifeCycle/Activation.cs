// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Notifications;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service.DailyNote;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Navigation;
using System.Diagnostics;

namespace Snap.Hutao.Core.LifeCycle;

/// <summary>
/// 激活
/// </summary>
[HighQuality]
[Injection(InjectAs.Singleton, typeof(IActivation))]
[SuppressMessage("", "CA1001")]
internal sealed class Activation : IActivation
{
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

    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;
    private readonly WeakReference<MainWindow> mainWindowReference = new(default!);
    private readonly SemaphoreSlim activateSemaphore = new(1);

    /// <summary>
    /// 构造一个新的激活
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public Activation(IServiceProvider serviceProvider)
    {
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        this.serviceProvider = serviceProvider;
    }

    /// <summary>
    /// 响应激活事件
    /// 激活事件一般不会在UI线程上触发
    /// </summary>
    /// <param name="sender">发送方</param>
    /// <param name="args">激活参数</param>
    public void Activate(object? sender, AppActivationArguments args)
    {
        _ = sender;
        if (!ToastNotificationManagerCompat.WasCurrentProcessToastActivated())
        {
            HandleActivationAsync(args, false).SafeForget();
        }
    }

    /// <summary>
    /// 触发激活事件
    /// </summary>
    /// <param name="sender">发送方</param>
    /// <param name="args">激活参数</param>
    public void NonRedirectToActivate(object? sender, AppActivationArguments args)
    {
        _ = sender;
        if (!ToastNotificationManagerCompat.WasCurrentProcessToastActivated())
        {
            HandleActivationAsync(args, false).SafeForget();
        }
    }

    /// <inheritdoc/>
    public bool IsIncludedInSelfStart(bool set = false)
    {
        if (set)
        {
            StaticResource.IsIncludeSelfStartOrSetState(true, !StaticResource.IsIncludeSelfStartOrSetState());
        }

        return StaticResource.IsIncludeSelfStartOrSetState();
    }

    private void NotificationActivate(ToastNotificationActivatedEventArgsCompat args)
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

    private async Task HandleActivationAsync(AppActivationArguments args, bool isRedirected)
    {
        if (activateSemaphore.CurrentCount > 0)
        {
            using (await activateSemaphore.EnterAsync().ConfigureAwait(false))
            {
                await HandleActivationCoreAsync(args, isRedirected).ConfigureAwait(false);
            }
        }
    }

    private async Task HandleActivationCoreAsync(AppActivationArguments args, bool isRedirected)
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

    private async Task HandleNormalLaunchActionAsync()
    {
        // Increase launch times
        LocalSetting.Set(SettingKeys.LaunchTimes, LocalSetting.Get(SettingKeys.LaunchTimes, 0) + 1);

        await WaitMainWindowAsync().ConfigureAwait(false);
    }

    private async Task WaitMainWindowAsync()
    {
        if (!mainWindowReference.TryGetTarget(out _))
        {
            await taskContext.SwitchToMainThreadAsync();

            mainWindowReference.SetTarget(serviceProvider.GetRequiredService<MainWindow>());

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
    }

    private async Task HandleUrlActivationAsync(Uri uri, bool isRedirected)
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

    private async Task HandleAchievementActionAsync(string action, string parameter, bool isRedirected)
    {
        _ = parameter;
        _ = isRedirected;
        switch (action)
        {
            case UrlActionImport:
                {
                    await taskContext.SwitchToMainThreadAsync();

                    INavigationAwaiter navigationAwaiter = new NavigationExtra(ImportUIAFFromClipboard);
                    await serviceProvider
                        .GetRequiredService<INavigationService>()
                        .NavigateAsync<View.Page.AchievementPage>(navigationAwaiter, true)
                        .ConfigureAwait(false);
                    break;
                }
        }
    }

    private async Task HandleDailyNoteActionAsync(string action, string parameter, bool isRedirected)
    {
        _ = parameter;
        switch (action)
        {
            case UrlActionRefresh:
                {
                    await serviceProvider
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

    private async Task HandleLaunchGameActionAsync(string? uid = null)
    {
        serviceProvider
            .GetRequiredService<IMemoryCache>()
            .Set(ViewModel.Game.LaunchGameViewModel.DesiredUid, uid);

        await taskContext.SwitchToMainThreadAsync();

        if (!mainWindowReference.TryGetTarget(out _))
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