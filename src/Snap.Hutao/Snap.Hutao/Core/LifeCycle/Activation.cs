// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Notifications;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.UI.Xaml;
using Snap.Hutao.Core.LifeCycle.InterProcess;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Core.Windowing.HotKey;
using Snap.Hutao.Core.Windowing.NotifyIcon;
using Snap.Hutao.Service.DailyNote;
using Snap.Hutao.Service.Discord;
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
internal sealed partial class Activation : IActivation, IDisposable
{
    public const string Action = nameof(Action);
    public const string Uid = nameof(Uid);
    public const string LaunchGame = nameof(LaunchGame);
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
    public void Activate(HutaoActivationArguments args)
    {
        // Before activate, we try to redirect to the opened process in App,
        // And we check if it's a toast activation.
        if (ToastNotificationManagerCompat.WasCurrentProcessToastActivated())
        {
            return;
        }

        HandleActivationAsync(args).SafeForget();
    }

    /// <inheritdoc/>
    public void PostInitialization()
    {
        serviceProvider.GetRequiredService<PrivateNamedPipeServer>().RunAsync().SafeForget();
        ToastNotificationManagerCompat.OnActivated += NotificationActivate;

        serviceProvider.GetRequiredService<HotKeyOptions>().RegisterAll();
        if (LocalSetting.Get(SettingKeys.IsNotifyIconEnabled, true))
        {
            serviceProvider.GetRequiredService<App>().DispatcherShutdownMode = DispatcherShutdownMode.OnExplicitShutdown;
            _ = serviceProvider.GetRequiredService<NotifyIconController>();
        }
    }

    public void Dispose()
    {
        activateSemaphore.Dispose();
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

    private async ValueTask HandleActivationAsync(HutaoActivationArguments args)
    {
        if (activateSemaphore.CurrentCount > 0)
        {
            using (await activateSemaphore.EnterAsync().ConfigureAwait(false))
            {
                await HandleActivationCoreAsync(args).ConfigureAwait(false);
            }
        }
    }

    private async ValueTask HandleActivationCoreAsync(HutaoActivationArguments args)
    {
        if (args.Kind is HutaoActivationKind.Protocol)
        {
            ArgumentNullException.ThrowIfNull(args.ProtocolActivatedUri);
            await HandleUrlActivationAsync(args.ProtocolActivatedUri, args.IsRedirectTo).ConfigureAwait(false);
        }
        else if (args.Kind is HutaoActivationKind.Launch)
        {
            ArgumentNullException.ThrowIfNull(args.LaunchActivatedArguments);
            switch (args.LaunchActivatedArguments)
            {
                default:
                    {
                        await HandleNormalLaunchActionAsync().ConfigureAwait(false);
                        break;
                    }
            }
        }
    }

    private async ValueTask HandleNormalLaunchActionAsync()
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

        // If it's the first time launch, show the guide window anyway.
        if (UnsafeLocalSetting.Get(SettingKeys.Major1Minor10Revision0GuideState, GuideState.Language) < GuideState.Completed)
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
        if (currentWindowReference.Window is not null)
        {
            return;
        }

        await taskContext.SwitchToMainThreadAsync();

        serviceProvider.GetRequiredService<MainWindow>();

        await taskContext.SwitchToBackgroundAsync();

        if (serviceProvider.GetRequiredService<IMetadataService>() is IMetadataServiceInitialization metadataServiceInitialization)
        {
            metadataServiceInitialization.InitializeInternalAsync().SafeForget();
        }

        if (serviceProvider.GetRequiredService<IHutaoUserService>() is IHutaoUserServiceInitialization hutaoUserServiceInitialization)
        {
            hutaoUserServiceInitialization.InitializeInternalAsync().SafeForget();
        }

        serviceProvider.GetRequiredService<IDiscordService>().SetNormalActivityAsync().SafeForget();
    }

    private async ValueTask HandleUrlActivationAsync(Uri uri, bool isRedirectTo)
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
                    await HandleAchievementActionAsync(action, parameter, isRedirectTo).ConfigureAwait(false);
                    break;
                }

            case CategoryDailyNote:
                {
                    await HandleDailyNoteActionAsync(action, parameter, isRedirectTo).ConfigureAwait(false);
                    break;
                }

            default:
                {
                    await HandleNormalLaunchActionAsync().ConfigureAwait(false);
                    break;
                }
        }
    }

    private async ValueTask HandleAchievementActionAsync(string action, string parameter, bool isRedirectTo)
    {
        _ = parameter;
        _ = isRedirectTo;
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

    private async ValueTask HandleDailyNoteActionAsync(string action, string parameter, bool isRedirectTo)
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
                    if (!isRedirectTo)
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
            serviceProvider.GetRequiredService<LaunchGameWindow>();
            return;
        }

        if (currentWindowReference.Window is MainWindow)
        {
            await serviceProvider
                .GetRequiredService<INavigationService>()
                .NavigateAsync<View.Page.LaunchGamePage>(INavigationAwaiter.Default, true)
                .ConfigureAwait(false);

            return;
        }
        else
        {
            // We have a non-Main Window, just exit current process anyway
            Process.GetCurrentProcess().Kill();
        }
    }
}