// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Configuration;
using Snap.Hutao.Service.Game.FileSystem;
using Snap.Hutao.Service.Game.Launching.Context;
using Snap.Hutao.Service.Game.Launching.Invoker;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.UI.Xaml.View.Page;

namespace Snap.Hutao.ViewModel.Game;

[Service(ServiceLifetime.Singleton)]
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
    private readonly IMessenger messenger;

    private bool resuming;

    public LaunchScheme? GetCurrentLaunchSchemeFromConfigFile(bool showInfo = true)
    {
        ChannelOptions options = gameService.GetChannelOptions();

        if (options.ErrorKind is ChannelOptionsErrorKind.None)
        {
            try
            {
                return KnownLaunchSchemes.Values.Single(scheme => scheme.Equals(options));
            }
            catch (InvalidOperationException)
            {
                // 后台收集
                HutaoException.Throw($"不支持的 ChannelOptions: {options}");
            }
        }

        if (!showInfo)
        {
            return default;
        }

        InfoBarMessage? message = options.ErrorKind switch
        {
            ChannelOptionsErrorKind.ConfigurationFileNotFound => InfoBarMessage.Warning(
                SH.FormatViewModelLaunchGameConfigurationFailed(options.ErrorKind),
                SH.FormatViewModelLaunchGameConfigurationFileNotFound(options.FilePath),
                SH.ViewModelLaunchGameFixConfigurationFileButtonText,
                HandleConfigurationFileNotFoundCommand),
            ChannelOptionsErrorKind.GamePathNullOrEmpty => InfoBarMessage.Warning(
                SH.FormatViewModelLaunchGameConfigurationFailed(options.ErrorKind),
                SH.ViewModelLaunchGameGamePathNullOrEmpty,
                SH.ViewModelLaunchGameSetGamePathButtonText,
                HandleGamePathNullOrEmptyCommand),
            ChannelOptionsErrorKind.DeviceNotFound => InfoBarMessage.Warning(
                SH.FormatViewModelLaunchGameConfigurationFailed(options.ErrorKind),
                SH.ViewModelLaunchGameDeviceNotFound),
            ChannelOptionsErrorKind.GameContentCorrupted => InfoBarMessage.Warning(
                SH.FormatViewModelLaunchGameConfigurationFailed(options.ErrorKind),
                SH.FormatViewModelLaunchGameContentCorrupted(options.FilePath)),
            _ => default,
        };

        if (message is not null)
        {
            messenger.Send(message);
        }

        return default;
    }

    public async ValueTask ResumeLaunchExecutionAsync(IViewModelSupportLaunchExecution viewModel)
    {
        if (Interlocked.Exchange(ref resuming, true))
        {
            return;
        }

        if (GetCurrentLaunchSchemeFromConfigFile(false) is null)
        {
            return;
        }

        if (AbstractLaunchExecutionInvoker.HasAnyInvoking())
        {
            return;
        }

        try
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                try
                {
                    using (LaunchExecutionContext context = new(scope.ServiceProvider, viewModel, default))
                    {
                        LaunchExecutionResult result = await new ResumeLaunchExecutionInvoker().InvokeAsync(context).ConfigureAwait(false);

                        if (result.Kind is not LaunchExecutionResultKind.Ok)
                        {
                            infoBarService.Warning(result.ErrorMessage);
                        }
                    }
                }
                catch (Exception ex)
                {
                    infoBarService.Error(ex);
                }
            }
        }
        finally
        {
            Volatile.Write(ref resuming, false);
        }
    }

    [Command("HandleConfigurationFileNotFoundCommand")]
    private async Task HandleConfigurationFileNotFoundAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Generate config file", "LaunchGameShared.Command"));

        if (!launchOptions.TryGetGameFileSystem(out IGameFileSystem? gameFileSystem))
        {
            return;
        }

        using (gameFileSystem)
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                LaunchGameConfigurationFixDialog dialog = await contentDialogFactory
                    .CreateInstanceAsync<LaunchGameConfigurationFixDialog>(scope.ServiceProvider)
                    .ConfigureAwait(false);

                bool isOversea = gameFileSystem.IsExecutableOversea();

                await taskContext.SwitchToMainThreadAsync();

                dialog.KnownSchemes = KnownLaunchSchemes.Values.Where(scheme => scheme.IsOversea == isOversea);
                dialog.SelectedScheme = dialog.KnownSchemes.First(scheme => scheme.IsNotCompatOnly);

                if (await dialog.GetLaunchSchemeAsync().ConfigureAwait(false) is not (true, { } launchScheme))
                {
                    return;
                }

                GameConfiguration.Patch(launchScheme, gameFileSystem.GetScriptVersionFilePath(), gameFileSystem.GetGameConfigurationFilePath());
                infoBarService.Success(SH.ViewModelLaunchGameFixConfigurationFileSuccess);
            }
        }
    }

    [Command("HandleGamePathNullOrEmptyCommand")]
    private void HandleGamePathNullOrEmpty()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Navigate to LaunchGamePage", "LaunchGameShared.Command"));
        navigationService.Navigate<LaunchGamePage>(NavigationExtraData.Default, true);
    }
}