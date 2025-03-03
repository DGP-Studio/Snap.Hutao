// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Core;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Configuration;
using Snap.Hutao.Service.Game.Launching;
using Snap.Hutao.Service.Game.Launching.Handler;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Service.Game.Unlocker;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.UI.Xaml.View.Page;
using System.Diagnostics;

namespace Snap.Hutao.ViewModel.Game;

[Injection(InjectAs.Singleton)]
[ConstructorGenerated]
internal sealed partial class LaunchGameShared
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly INavigationService navigationService;
    private readonly IServiceProvider serviceProvider;
    private readonly IInfoBarService infoBarService;
    private readonly LaunchOptions launchOptions;
    private readonly ITaskContext taskContext;
    private readonly IGameService gameService;

    private TaskCompletionSource? tcs;

    public bool IsGameLaunched { get; set; }

    public LaunchScheme? GetCurrentLaunchSchemeFromConfigFile()
    {
        ChannelOptions options = gameService.GetChannelOptions();

        switch (options.ErrorKind)
        {
            case ChannelOptionsErrorKind.None:
                try
                {
                    return KnownLaunchSchemes.Values.Single(scheme => scheme.Equals(options));
                }
                catch (InvalidOperationException)
                {
                    if (!IgnoredInvalidChannelOptions.Contains(options))
                    {
                        // 后台收集
                        HutaoException.Throw($"不支持的 ChannelOptions: {options}");
                    }
                }

                break;
            case ChannelOptionsErrorKind.ConfigurationFileNotFound:
                infoBarService.Warning(
                    $"{options.ErrorKind}",
                    SH.FormatViewModelLaunchGameConfigurationFileNotFound(options.FilePath),
                    SH.ViewModelLaunchGameFixConfigurationFileButtonText,
                    HandleConfigurationFileNotFoundCommand);
                break;
            case ChannelOptionsErrorKind.GamePathNullOrEmpty:
                infoBarService.Warning(
                    $"{options.ErrorKind}",
                    SH.ViewModelLaunchGameGamePathNullOrEmpty,
                    SH.ViewModelLaunchGameSetGamePathButtonText,
                    HandleGamePathNullOrEmptyCommand);
                break;
            case ChannelOptionsErrorKind.GameContentCorrupted:
                infoBarService.Warning(
                    $"{options.ErrorKind}",
                    SH.FormatViewModelLaunchGameContentCorrupted(options.FilePath));
                break;
        }

        return default;
    }

    public async ValueTask ResumeLaunchExecutionAsync()
    {
        if (IsGameLaunched || tcs is not null)
        {
            return;
        }

        await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);
        if (!LaunchExecutionEnsureGameNotRunningHandler.IsGameRunning(out Process? gameProcess))
        {
            return;
        }

        tcs = new();
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<IMessenger>().Send(new LaunchExecutionGameFileSystemExclusiveAccessChangedMessage(false));

            if (HutaoRuntime.IsProcessElevated && launchOptions.IsIslandEnabled)
            {
                if (!launchOptions.TryGetGameFileSystem(out IGameFileSystem? gameFileSystem))
                {
                    return;
                }

                using (gameFileSystem)
                {
                    if (!gameFileSystem.TryGetGameVersion(out string? gameVersion))
                    {
                        return;
                    }

                    GameFpsUnlocker unlocker = new(serviceProvider, gameProcess, gameVersion);
                    if (await unlocker.UnlockAsync(true).ConfigureAwait(false))
                    {
                        await unlocker.PostUnlockAsync(true).ConfigureAwait(false);
                    }
                }
            }

            unsafe
            {
                SpinWaitPolyfill.SpinWhile(&LaunchExecutionEnsureGameNotRunningHandler.IsGameRunning);
            }

            scope.ServiceProvider.GetRequiredService<IMessenger>().Send<LaunchExecutionProcessStatusChangedMessage>();
            scope.ServiceProvider.GetRequiredService<IMessenger>().Send(new LaunchExecutionGameFileSystemExclusiveAccessChangedMessage(true));
        }

        tcs.TrySetResult();
        tcs = default;
    }

    [Command("HandleConfigurationFileNotFoundCommand")]
    private async Task HandleConfigurationFileNotFoundAsync()
    {
        if (!launchOptions.TryGetGameFileSystem(out IGameFileSystem? gameFileSystem))
        {
            return;
        }

        using (gameFileSystem)
        {
            LaunchGameConfigurationFixDialog dialog = await contentDialogFactory
                .CreateInstanceAsync<LaunchGameConfigurationFixDialog>()
                .ConfigureAwait(false);

            bool isOversea = gameFileSystem.IsOversea();

            await taskContext.SwitchToMainThreadAsync();

            dialog.KnownSchemes = KnownLaunchSchemes.Values.Where(scheme => scheme.IsOversea == isOversea);
            dialog.SelectedScheme = dialog.KnownSchemes.First(scheme => scheme.IsNotCompatOnly);
            (bool isOk, LaunchScheme launchScheme) = await dialog.GetLaunchSchemeAsync().ConfigureAwait(false);

            if (!isOk)
            {
                return;
            }

            gameFileSystem.TryFixConfigurationFile(launchScheme);
            infoBarService.Success(SH.ViewModelLaunchGameFixConfigurationFileSuccess);
        }
    }

    [Command("HandleGamePathNullOrEmptyCommand")]
    private void HandleGamePathNullOrEmpty()
    {
        navigationService.Navigate<LaunchGamePage>(INavigationCompletionSource.Default, true);
    }
}