// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppNotifications;
using Snap.Hutao.Core.LifeCycle.InterProcess;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Core.Shell;
using Snap.Hutao.Service;
using Snap.Hutao.Service.Discord;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Job;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.UI.Input.HotKey;
using Snap.Hutao.UI.Shell;
using Snap.Hutao.UI.Xaml;
using Snap.Hutao.UI.Xaml.View.Page;
using Snap.Hutao.UI.Xaml.View.Window;
using Snap.Hutao.ViewModel.Guide;
using System.Diagnostics;

namespace Snap.Hutao.Core.LifeCycle;

[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IAppActivation))]
[SuppressMessage("", "CA1001")]
internal sealed partial class AppActivation : IAppActivation, IAppActivationActionHandlersAccess, IDisposable
{
    public const string Action = nameof(Action);
    public const string Uid = nameof(Uid);
    public const string LaunchGame = nameof(LaunchGame);
    public const string ImportUIAFFromClipboard = nameof(ImportUIAFFromClipboard);

    private const string CategoryAchievement = "ACHIEVEMENT";
    private const string UrlActionImport = "/IMPORT";

    private readonly ICurrentXamlWindowReference currentWindowReference;
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<AppActivation> logger;
    private readonly ITaskContext taskContext;

    private readonly SemaphoreSlim activateSemaphore = new(1);

    public void Activate(HutaoActivationArguments args)
    {
        HandleActivationExclusiveAsync(args).SafeForget(logger);

        async ValueTask HandleActivationExclusiveAsync(HutaoActivationArguments args)
        {
            await taskContext.SwitchToBackgroundAsync();

            if (activateSemaphore.CurrentCount > 0)
            {
                using (await activateSemaphore.EnterAsync().ConfigureAwait(false))
                {
                    switch (args.Kind)
                    {
                        case HutaoActivationKind.Protocol:
                            {
                                ArgumentNullException.ThrowIfNull(args.ProtocolActivatedUri);
                                await HandleProtocolActivationAsync(args.ProtocolActivatedUri, args.IsRedirectTo).ConfigureAwait(false);
                                break;
                            }

                        case HutaoActivationKind.Launch:
                            {
                                ArgumentNullException.ThrowIfNull(args.LaunchActivatedArguments);
                                await HandleLaunchActivationAsync(args.IsRedirectTo).ConfigureAwait(false);
                                break;
                            }

                        case HutaoActivationKind.AppNotification:
                            {
                                ArgumentNullException.ThrowIfNull(args.AppNotificationActivatedArguments);
                                await HandleAppNotificationActivationAsync(args.AppNotificationActivatedArguments, args.IsRedirectTo).ConfigureAwait(false);
                                break;
                            }
                    }
                }
            }
        }
    }

    public void NotificationInvoked(AppNotificationManager manager, AppNotificationActivatedEventArgs args)
    {
        HandleAppNotificationActivationAsync(args.Arguments, false).SafeForget(logger);
    }

    public void PostInitialization()
    {
        RunPostInitializationAsync().SafeForget(logger);

        async ValueTask RunPostInitializationAsync()
        {
            await taskContext.SwitchToBackgroundAsync();

            using (await activateSemaphore.EnterAsync().ConfigureAwait(false))
            {
                // TODO: Introduced in 1.10.2, remove in later version
                {
                    serviceProvider.GetRequiredService<IJumpListInterop>().ClearAsync().SafeForget(logger);
                    serviceProvider.GetRequiredService<IScheduleTaskInterop>().UnregisterAllTasks();
                }

                if (UnsafeLocalSetting.Get(SettingKeys.Major1Minor10Revision0GuideState, GuideState.Language) < GuideState.Completed)
                {
                    return;
                }

                serviceProvider.GetRequiredService<PrivateNamedPipeServer>().RunAsync().SafeForget(logger);

                // RegisterHotKey should be called from main thread
                await taskContext.SwitchToMainThreadAsync();
                serviceProvider.GetRequiredService<HotKeyOptions>().RegisterAll();

                if (serviceProvider.GetRequiredService<AppOptions>().IsNotifyIconEnabled)
                {
                    XamlApplicationLifetime.LaunchedWithNotifyIcon = true;

                    await taskContext.SwitchToMainThreadAsync();
                    serviceProvider.GetRequiredService<App>().DispatcherShutdownMode = DispatcherShutdownMode.OnExplicitShutdown;
                    _ = serviceProvider.GetRequiredService<NotifyIconController>();
                }

                serviceProvider.GetRequiredService<IDiscordService>().SetNormalActivityAsync().SafeForget(logger);
                serviceProvider.GetRequiredService<IQuartzService>().StartAsync().SafeForget(logger);

                if (serviceProvider.GetRequiredService<IMetadataService>() is IMetadataServiceInitialization metadataServiceInitialization)
                {
                    metadataServiceInitialization.InitializeInternalAsync().SafeForget(logger);
                }

                if (serviceProvider.GetRequiredService<IHutaoUserService>() is IHutaoUserServiceInitialization hutaoUserServiceInitialization)
                {
                    hutaoUserServiceInitialization.InitializeInternalAsync().SafeForget(logger);
                }
            }
        }
    }

    public void Dispose()
    {
        activateSemaphore.Dispose();
    }

    public async ValueTask HandleLaunchGameActionAsync(string? uid = null)
    {
        serviceProvider
            .GetRequiredService<IMemoryCache>()
            .Set(ViewModel.Game.LaunchGameViewModel.DesiredUid, uid);

        await taskContext.SwitchToMainThreadAsync();

        switch (currentWindowReference.Window)
        {
            case null:
                LaunchGameWindow launchGameWindow = serviceProvider.GetRequiredService<LaunchGameWindow>();
                currentWindowReference.Window = launchGameWindow;

                launchGameWindow.SwitchTo();
                launchGameWindow.BringToForeground();
                return;

            case MainWindow:
                await serviceProvider
                    .GetRequiredService<INavigationService>()
                    .NavigateAsync<LaunchGamePage>(INavigationAwaiter.Default, true)
                    .ConfigureAwait(false);
                return;

            case LaunchGameWindow currentLaunchGameWindow:
                currentLaunchGameWindow.SwitchTo();
                currentLaunchGameWindow.BringToForeground();
                return;

            default:
                Process.GetCurrentProcess().Kill();
                return;
        }
    }

    private async ValueTask HandleProtocolActivationAsync(Uri uri, bool isRedirectTo)
    {
        UriBuilder builder = new(uri);

        string category = builder.Host.ToUpperInvariant();
        string action = builder.Path.ToUpperInvariant();

        // string parameter = builder.Query.ToUpperInvariant();
        switch (category)
        {
            case CategoryAchievement:
                {
                    await WaitMainWindowOrCurrentAsync().ConfigureAwait(false);
                    if (currentWindowReference.Window is not MainWindow)
                    {
                        // TODO: Send notification to hint?
                        return;
                    }

                    switch (action)
                    {
                        case UrlActionImport:
                            {
                                await taskContext.SwitchToMainThreadAsync();

                                INavigationAwaiter navigationAwaiter = new NavigationExtra(ImportUIAFFromClipboard);
#pragma warning disable CA1849
                                // We can't await here to navigate to Achievment Page, the Achievement
                                // ViewModel requires the Metadata Service to be initialized.
                                serviceProvider
                                    .GetRequiredService<INavigationService>()
                                    .Navigate<AchievementPage>(navigationAwaiter, true);
#pragma warning restore CA1849
                                break;
                            }
                    }

                    break;
                }

            default:
                {
                    await HandleLaunchActivationAsync(isRedirectTo).ConfigureAwait(false);
                    break;
                }
        }
    }

    private async ValueTask HandleLaunchActivationAsync(bool isRedirectTo)
    {
        if (!isRedirectTo)
        {
            // Increase launch times
            LocalSetting.Update(SettingKeys.LaunchTimes, 0, x => unchecked(x + 1));

            // If the guide is completed, we check if there's any unfulfilled resource category present.
            if (UnsafeLocalSetting.Get(SettingKeys.Major1Minor10Revision0GuideState, GuideState.Language) >= GuideState.StaticResourceBegin)
            {
                if (StaticResource.IsAnyUnfulfilledCategoryPresent())
                {
                    UnsafeLocalSetting.Set(SettingKeys.Major1Minor10Revision0GuideState, GuideState.StaticResourceBegin);
                }
            }

            if (UnsafeLocalSetting.Get(SettingKeys.Major1Minor10Revision0GuideState, GuideState.Language) < GuideState.Completed)
            {
                await taskContext.SwitchToMainThreadAsync();

                GuideWindow guideWindow = serviceProvider.GetRequiredService<GuideWindow>();
                currentWindowReference.Window = guideWindow;

                guideWindow.SwitchTo();
                guideWindow.BringToForeground();
                return;
            }
        }

        await WaitMainWindowOrCurrentAsync().ConfigureAwait(false);
    }

    private async ValueTask HandleAppNotificationActivationAsync(IDictionary<string, string> arguments, bool isRedirectTo)
    {
        if (arguments.TryGetValue(Action, out string? action))
        {
            if (action == LaunchGame)
            {
                _ = arguments.TryGetValue(Uid, out string? uid);
                await HandleLaunchGameActionAsync(uid).ConfigureAwait(false);
            }
        }
        else
        {
            await HandleLaunchActivationAsync(isRedirectTo).ConfigureAwait(false);
        }
    }

    private async ValueTask WaitMainWindowOrCurrentAsync()
    {
        if (currentWindowReference.Window is { } window)
        {
            window.SwitchTo();
            window.BringToForeground();
            return;
        }

        await taskContext.SwitchToMainThreadAsync();

        MainWindow mainWindow = serviceProvider.GetRequiredService<MainWindow>();
        currentWindowReference.Window = mainWindow;

        mainWindow.SwitchTo();
        mainWindow.BringToForeground();
    }
}