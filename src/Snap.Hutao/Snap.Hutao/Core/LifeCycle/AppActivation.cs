// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Windows.AppNotifications;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.LifeCycle.InterProcess;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Factory.Process;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Job;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Input.HotKey;
using Snap.Hutao.UI.Shell;
using Snap.Hutao.UI.Windowing;
using Snap.Hutao.UI.Xaml.View.Page;
using Snap.Hutao.UI.Xaml.View.Window;
using Snap.Hutao.ViewModel.Achievement;
using Snap.Hutao.ViewModel.Game;
using Snap.Hutao.ViewModel.Guide;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Core.LifeCycle;

[Service(ServiceLifetime.Singleton, typeof(IAppActivation))]
[SuppressMessage("", "CA1001")]
internal sealed partial class AppActivation : IAppActivation, IAppActivationActionHandlersAccess
{
    public const string Action = nameof(Action);
    public const string Uid = nameof(Uid);
    public const string LaunchGame = nameof(LaunchGame);

    private const string CategoryAchievement = "ACHIEVEMENT";
    private const string UrlActionImport = "/IMPORT";

    private readonly ICurrentXamlWindowReference currentXamlWindowReference;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;

    private readonly AsyncLock activateLock = new();
    private int isActivating;

    [GeneratedConstructor]
    public partial AppActivation(IServiceProvider serviceProvider);

    public void RedirectedActivate(HutaoActivationArguments args)
    {
        HandleActivationExclusivelyAsync(args).SafeForget();

        async ValueTask HandleActivationExclusivelyAsync(HutaoActivationArguments args)
        {
            if (Interlocked.CompareExchange(ref isActivating, 1, 0) is not 0)
            {
                return;
            }

            using (await activateLock.LockAsync().ConfigureAwait(false))
            {
                await UnsynchronizedHandleActivationAsync(args).ConfigureAwait(false);
            }

            Interlocked.Exchange(ref isActivating, 0);
        }
    }

    public void NotificationInvoked(AppNotificationManager manager, AppNotificationActivatedEventArgs args)
    {
        HandleAppNotificationActivationAsync(args.Arguments.AsReadOnly(), false).SafeForget();
    }

    public void ActivateAndInitialize(HutaoActivationArguments args)
    {
        if (Volatile.Read(ref isActivating) is 1)
        {
            return;
        }

        ActivateAndInitializeAsync().SafeForget();

        async ValueTask ActivateAndInitializeAsync()
        {
            try
            {
                using (await activateLock.LockAsync().ConfigureAwait(false))
                {
                    if (Interlocked.CompareExchange(ref isActivating, 1, 0) is not 0)
                    {
                        return;
                    }

                    await UnsynchronizedHandleActivationAsync(args).ConfigureAwait(false);
                    await UnsynchronizedHandleInitializationAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                XamlApplicationLifetime.ActivationAndInitializationCompleted = true;
                Interlocked.Exchange(ref isActivating, 0);
            }
        }
    }

    public async ValueTask HandleLaunchGameActionAsync(string? uid = null)
    {
        await taskContext.SwitchToMainThreadAsync();

        switch (currentXamlWindowReference.Window)
        {
            case null:
            case MainWindow:
                if (await WaitWindowAsync<MainWindow>().ConfigureAwait(true) is not null)
                {
                    INavigationService navigationService = serviceProvider.GetRequiredService<INavigationService>();
                    await navigationService.NavigateAsync<LaunchGamePage>(LaunchGameExtraData.CreateForUid(uid), true).ConfigureAwait(false);
                }

                return;

            default:
                Debugger.Break(); // Should never happen
                ProcessFactory.KillCurrent();
                return;
        }
    }

    private async ValueTask UnsynchronizedHandleActivationAsync(HutaoActivationArguments args)
    {
        await taskContext.SwitchToBackgroundAsync();
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

    private async ValueTask UnsynchronizedHandleInitializationAsync()
    {
        // Sentry IpAddress Traits, should always be configured
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            // Transient, we need the scope to manage its lifetime
            await scope.ServiceProvider.GetRequiredService<SentryIpAddressEnricher>().ConfigureAsync().ConfigureAwait(false);
        }

        // In guide
        if (UnsafeLocalSetting.Get(SettingKeys.GuideState, GuideState.Language) < GuideState.Completed)
        {
            return;
        }

        // Start named pipe server
        serviceProvider.GetRequiredService<PrivateNamedPipeServer>().Start();
        Bootstrap.UseNamedPipeRedirection();

        // Notify icon
        App app = serviceProvider.GetRequiredService<App>();
        await taskContext.SwitchToMainThreadAsync();
        try
        {
            app.DispatcherShutdownMode = DispatcherShutdownMode.OnExplicitShutdown;
        }
        catch (COMException ex) when (ex.HResult == unchecked((int)0x8001010E))
        {
            // The given object has already been closed / disposed and may no longer be used.
            ProcessFactory.KillCurrent();
        }

        lock (NotifyIconController.InitializationSyncRoot)
        {
            try
            {
                serviceProvider.GetRequiredService<NotifyIconController>().Create();
                XamlApplicationLifetime.NotifyIconCreated = true;
            }
            catch (Exception ex)
            {
                serviceProvider.GetRequiredService<IMessenger>().Send(InfoBarMessage.Error(new HutaoException(SH.CoreLifeCycleAppActivationNotifyIconCreateFailed, ex)));
            }
        }

        await taskContext.SwitchToBackgroundAsync();

        // Services Initialization
        await Task.WhenAll(
        [
            serviceProvider.GetRequiredService<HotKeyOptions>().InitializeAsync().AsTask(),
            serviceProvider.GetRequiredService<HutaoUserOptions>().InitializeAsync().AsTask(),
            serviceProvider.GetRequiredService<IMetadataService>().InitializeInternalAsync().AsTask(),
            serviceProvider.GetRequiredService<IQuartzService>().StartAsync()
        ]).ConfigureAwait(false);

        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateInfo("Initialization completed", "Application"));
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
                    await WaitWindowAsync<MainWindow>().ConfigureAwait(false);
                    switch (action)
                    {
                        case UrlActionImport:
                            {
                                await taskContext.SwitchToMainThreadAsync();

                                INavigationCompletionSource navigationAwaiter = new NavigationExtraData(AchievementViewModel.ImportUIAFFromClipboard);
#pragma warning disable CA1849
                                // We can't await there to navigate to Achievement Page, the Achievement
                                // ViewModel requires the Metadata Service to be initialized.
                                // Which is initialized in there (AppActivation - Initialization) which is after Activation.
                                // Thus await would cause a deadlock.
                                // ReSharper disable once MethodHasAsyncOverload
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
        if (isRedirectTo)
        {
            await WaitWindowAsync<MainWindow>().ConfigureAwait(false);
            return;
        }

        // Increase launch times
        LocalSetting.Update(SettingKeys.LaunchTimes, 0, static x => unchecked(x + 1));

        // If the guide is completed, we check if there's any unfulfilled resource category present.
        if (UnsafeLocalSetting.Get(SettingKeys.GuideState, GuideState.Language) >= GuideState.StaticResourceBegin)
        {
            if (StaticResource.IsAnyUnfulfilledCategoryPresent())
            {
                UnsafeLocalSetting.Set(SettingKeys.GuideState, GuideState.StaticResourceBegin);
            }
        }

        if (UnsafeLocalSetting.Get(SettingKeys.GuideState, GuideState.Language) < GuideState.Completed)
        {
            await WaitWindowAsync<GuideWindow>().ConfigureAwait(false);
            return;
        }

        if (Version.Parse(LocalSetting.Update(SettingKeys.LastVersion, "0.0.0.0", $"{HutaoRuntime.Version}")) < HutaoRuntime.Version)
        {
            // Note: If the user close MainWindow too quickly, and then exit app, he will never see the update log again.
            XamlApplicationLifetime.IsFirstRunAfterUpdate = true;
        }

        await WaitWindowAsync<MainWindow>().ConfigureAwait(false);
    }

    private async ValueTask HandleAppNotificationActivationAsync(IReadOnlyDictionary<string, string> arguments, bool isRedirectTo)
    {
        if (arguments.TryGetValue(Action, out string? action))
        {
            if (action is LaunchGame)
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

    private async ValueTask<Window?> WaitWindowAsync<TWindow>()
        where TWindow : Window
    {
        await taskContext.SwitchToMainThreadAsync();

        if (currentXamlWindowReference.Window is not { } window)
        {
            try
            {
                window = serviceProvider.GetRequiredService<TWindow>();
            }
            catch (COMException)
            {
                if (XamlApplicationLifetime.Exiting)
                {
                    return default;
                }

                throw;
            }

            currentXamlWindowReference.Window = window;
        }

        window.SwitchTo();
        window.AppWindow?.MoveInZOrderAtTop();
        return window;
    }
}