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
using Snap.Hutao.ViewModel.Guide;
using System.Diagnostics;

namespace Snap.Hutao.Core.LifeCycle;

/// <summary>
/// 激活
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IActivation))]
[SuppressMessage("", "CA1001")]
internal sealed partial class Activation : IActivation
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

    private const string CategoryAchievement = "ACHIEVEMENT";
    private const string CategoryDailyNote = "DAILYNOTE";
    private const string UrlActionImport = "/IMPORT";
    private const string UrlActionRefresh = "/REFRESH";

    private readonly IServiceProvider serviceProvider;
    private readonly ICurrentWindowReference currentWindowReference;
    private readonly ITaskContext taskContext;
    private readonly SemaphoreSlim activateSemaphore = new(1);

    /// <inheritdoc/>
    public void Activate(object? sender, AppActivationArguments args)
    {
        _ = sender;
        if (!ToastNotificationManagerCompat.WasCurrentProcessToastActivated())
        {
            HandleActivationAsync(args, true).SafeForget();
        }
    }

    /// <inheritdoc/>
    public void NonRedirectToActivate(object? sender, AppActivationArguments args)
    {
        _ = sender;
        if (!ToastNotificationManagerCompat.WasCurrentProcessToastActivated())
        {
            HandleActivationAsync(args, false).SafeForget();
        }
    }

    /// <inheritdoc/>
    public void InitializeWith(AppInstance appInstance)
    {
        appInstance.Activated += Activate;
        ToastNotificationManagerCompat.OnActivated += NotificationActivate;
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

    private async ValueTask HandleActivationAsync(AppActivationArguments args, bool isRedirected)
    {
        if (activateSemaphore.CurrentCount > 0)
        {
            using (await activateSemaphore.EnterAsync().ConfigureAwait(false))
            {
                await HandleActivationCoreAsync(args, isRedirected).ConfigureAwait(false);
            }
        }
    }

    private async ValueTask HandleActivationCoreAsync(AppActivationArguments args, bool isRedirected)
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

    private async ValueTask HandleNormalLaunchActionAsync()
    {
        // Increase launch times
        LocalSetting.Set(SettingKeys.LaunchTimes, LocalSetting.Get(SettingKeys.LaunchTimes, 0) + 1);

        if (StaticResource.IsAnyUnfulfilledCategoryPresent())
        {
            LocalSetting.Set(SettingKeys.Major1Minor7Revision0GuideState, (uint)GuideState.StaticResourceBegin);
        }

        if (LocalSetting.Get(SettingKeys.Major1Minor7Revision0GuideState, (uint)GuideState.Language) < (uint)GuideState.Completed)
        {
            await taskContext.SwitchToMainThreadAsync();
            serviceProvider.GetRequiredService<GuideWindow>();
        }
        else
        {
            await WaitMainWindowAsync().ConfigureAwait(false);
        }
    }

    private async ValueTask WaitMainWindowAsync()
    {
        if (currentWindowReference.Window is null)
        {
            await taskContext.SwitchToMainThreadAsync();

            currentWindowReference.Window = serviceProvider.GetRequiredService<MainWindow>();

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

    private async ValueTask HandleUrlActivationAsync(Uri uri, bool isRedirected)
    {
        UriBuilder builder = new(uri);

        string category = builder.Host.ToUpperInvariant();
        string action = builder.Path.ToUpperInvariant();
        string parameter = builder.Query.ToUpperInvariant();

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

    private async ValueTask HandleAchievementActionAsync(string action, string parameter, bool isRedirected)
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

    private async ValueTask HandleDailyNoteActionAsync(string action, string parameter, bool isRedirected)
    {
        _ = parameter;
        switch (action)
        {
            case UrlActionRefresh:
                {
                    try
                    {
                        await serviceProvider
                            .GetRequiredService<IDailyNoteService>()
                            .RefreshDailyNotesAsync()
                            .ConfigureAwait(false);
                    }
                    catch
                    {
                    }

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

    private async ValueTask HandleLaunchGameActionAsync(string? uid = null)
    {
        serviceProvider
            .GetRequiredService<IMemoryCache>()
            .Set(ViewModel.Game.LaunchGameViewModel.DesiredUid, uid);

        await taskContext.SwitchToMainThreadAsync();

        if (currentWindowReference.Window is null)
        {
            currentWindowReference.Window = serviceProvider.GetRequiredService<LaunchGameWindow>();
        }
        else
        {
            if (currentWindowReference.Window is MainWindow)
            {
                await serviceProvider
                    .GetRequiredService<INavigationService>()
                    .NavigateAsync<View.Page.LaunchGamePage>(INavigationAwaiter.Default, true)
                    .ConfigureAwait(false);
            }
            else
            {
                // We have a non-Main Window, just exit current process anyway
                Process.GetCurrentProcess().Kill();
            }
        }
    }
}