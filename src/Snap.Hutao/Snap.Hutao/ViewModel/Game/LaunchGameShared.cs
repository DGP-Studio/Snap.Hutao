// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Configuration;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.UI.Xaml.View.Page;

namespace Snap.Hutao.ViewModel.Game;

[Injection(InjectAs.Transient)]
[ConstructorGenerated]
internal sealed partial class LaunchGameShared
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly INavigationService navigationService;
    private readonly IGameServiceFacade gameService;
    private readonly IInfoBarService infoBarService;
    private readonly LaunchOptions launchOptions;
    private readonly ITaskContext taskContext;

    public LaunchScheme? GetCurrentLaunchSchemeFromConfigFile()
    {
        ChannelOptions options = gameService.GetChannelOptions();

        switch (options.ErrorKind)
        {
            case ChannelOptionsErrorKind.None:
                try
                {
                    return KnownLaunchSchemes.Get().Single(scheme => scheme.Equals(options));
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
                    SH.FormatViewModelLaunchGameMultiChannelReadFail(options.FilePath),
                    SH.ViewModelLaunchGameFixConfigurationFileButtonText,
                    HandleConfigurationFileNotFoundCommand);
                break;
            case ChannelOptionsErrorKind.GamePathNullOrEmpty:
                infoBarService.Warning(
                    $"{options.ErrorKind}",
                    SH.FormatViewModelLaunchGameMultiChannelReadFail(options.FilePath),
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

    [Command("HandleConfigurationFileNotFoundCommand")]
    private async Task HandleConfigurationFileNotFoundAsync()
    {
        if (!launchOptions.TryGetGameFileSystem(out IGameFileSystem? gameFileSystem))
        {
            return;
        }

        using (gameFileSystem)
        {
            bool isOversea = LaunchScheme.ExecutableIsOversea(gameFileSystem.GameFileName);

            LaunchGameConfigurationFixDialog dialog = await contentDialogFactory
                .CreateInstanceAsync<LaunchGameConfigurationFixDialog>()
                .ConfigureAwait(false);

            await taskContext.SwitchToMainThreadAsync();
            dialog.KnownSchemes = KnownLaunchSchemes.Get().Where(scheme => scheme.IsOversea == isOversea);
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