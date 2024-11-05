﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32.SafeHandles;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Progress;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Service.Game.Configuration;
using Snap.Hutao.Service.Game.Package;
using Snap.Hutao.Service.Game.Package.Advanced;
using Snap.Hutao.UI.Xaml.Control;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.ChannelSDK;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.DeprecatedFile;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Package;
using Snap.Hutao.Web.Response;
using System.IO;
using System.Net.Http;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionEnsureGameResourceHandler : ILaunchExecutionDelegateHandler
{
    public async ValueTask OnExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (!context.TryGetGameFileSystem(out GameFileSystem? gameFileSystem))
        {
            return;
        }

        if (ShouldConvert(context, gameFileSystem))
        {
            IServiceProvider serviceProvider = context.ServiceProvider;
            IContentDialogFactory contentDialogFactory = serviceProvider.GetRequiredService<IContentDialogFactory>();

            LaunchGamePackageConvertDialog dialog = await contentDialogFactory.CreateInstanceAsync<LaunchGamePackageConvertDialog>().ConfigureAwait(false);
            using (await contentDialogFactory.BlockAsync(dialog).ConfigureAwait(false))
            {
                IProgress<PackageConvertStatus> convertProgress = serviceProvider
                    .GetRequiredService<IProgressFactory>()
                    .CreateForMainThread<PackageConvertStatus>(state => dialog.State = state);

                if (!await EnsureGameResourceAsync(context, gameFileSystem, convertProgress).ConfigureAwait(false))
                {
                    // context.Result is set in EnsureGameResourceAsync
                    return;
                }

                // Backup config file, recover when a incompatible launcher deleted it.
                context.ServiceProvider.GetRequiredService<IGameConfigurationFileService>().Backup(gameFileSystem.GameConfigFilePath);

                await context.TaskContext.SwitchToMainThreadAsync();
                context.UpdateGamePathEntry();
            }
        }

        await next().ConfigureAwait(false);
    }

    private static bool ShouldConvert(LaunchExecutionContext context, GameFileSystem gameFileSystem)
    {
        // Configuration file changed
        if (context.ChannelOptionsChanged)
        {
            return true;
        }

        // Executable name not match
        if (!context.TargetScheme.ExecutableMatches(gameFileSystem.GameFileName))
        {
            return true;
        }

        if (!context.TargetScheme.IsOversea)
        {
            // [It's Bilibili channel xor PCGameSDK.dll exists] means we need to convert
            if (context.TargetScheme.Channel is ChannelType.Bili ^ File.Exists(gameFileSystem.PCGameSDKFilePath))
            {
                return true;
            }
        }

        return false;
    }

    private static async ValueTask<bool> EnsureGameResourceAsync(LaunchExecutionContext context, GameFileSystem gameFileSystem, IProgress<PackageConvertStatus> progress)
    {
        string gameFolder = gameFileSystem.GameDirectory;
        context.Logger.LogInformation("Game folder: {GameFolder}", gameFolder);

        if (!CheckDirectoryPermissions(gameFolder))
        {
            context.Result.Kind = LaunchExecutionResultKind.GameDirectoryInsufficientPermissions;
            context.Result.ErrorMessage = SH.ServiceGameEnsureGameResourceInsufficientDirectoryPermissions;
            return false;
        }

        progress.Report(new(SH.ServiceGameEnsureGameResourceQueryResourceInformation));

        HoyoPlayClient hoyoPlayClient = context.ServiceProvider.GetRequiredService<HoyoPlayClient>();

        Response<GameChannelSDKsWrapper> sdkResponse = await hoyoPlayClient.GetChannelSDKAsync(context.TargetScheme).ConfigureAwait(false);
        if (!ResponseValidator.TryValidateWithoutUINotification(sdkResponse, out GameChannelSDKsWrapper? channelSDKs))
        {
            context.Result.Kind = LaunchExecutionResultKind.GameResourceIndexQueryInvalidResponse;
            context.Result.ErrorMessage = SH.FormatServiceGameLaunchExecutionGameResourceQueryIndexFailed(sdkResponse);
            return false;
        }

        Response<DeprecatedFileConfigurationsWrapper> deprecatedFileResponse = await hoyoPlayClient.GetDeprecatedFileConfigurationsAsync(context.TargetScheme).ConfigureAwait(false);
        if (!ResponseValidator.TryValidateWithoutUINotification(deprecatedFileResponse, out DeprecatedFileConfigurationsWrapper? deprecatedFileConfigs))
        {
            context.Result.Kind = LaunchExecutionResultKind.GameResourceIndexQueryInvalidResponse;
            context.Result.ErrorMessage = SH.FormatServiceGameLaunchExecutionGameResourceQueryIndexFailed(deprecatedFileResponse);
            return false;
        }

        IHttpClientFactory httpClientFactory = context.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        using (HttpClient httpClient = httpClientFactory.CreateClient(GamePackageService.HttpClientName))
        {
            PackageConverterType type = context.ServiceProvider.GetRequiredService<AppOptions>().PackageConverterType;

            PackageConverterContext.CommonReferences common = new(
                httpClient,
                context.CurrentScheme,
                context.TargetScheme,
                gameFileSystem,
                channelSDKs.GameChannelSDKs.SingleOrDefault(),
                deprecatedFileConfigs.DeprecatedFileConfigurations.SingleOrDefault(),
                progress);

            PackageConverterContext packageConverterContext;
            switch (type)
            {
                case PackageConverterType.ScatteredFiles:
                    Response<GamePackagesWrapper> packagesResponse = await hoyoPlayClient.GetPackagesAsync(context.TargetScheme).ConfigureAwait(false);
                    if (!ResponseValidator.TryValidateWithoutUINotification(packagesResponse, out GamePackagesWrapper? gamePackages))
                    {
                        context.Result.Kind = LaunchExecutionResultKind.GameResourceIndexQueryInvalidResponse;
                        context.Result.ErrorMessage = SH.FormatServiceGameLaunchExecutionGameResourceQueryIndexFailed(packagesResponse);
                        return false;
                    }

                    packageConverterContext = new(common, gamePackages.GamePackages.Single());
                    break;
                case PackageConverterType.SophonChunks:
                    Response<GameBranchesWrapper> currentBranchesResponse = await hoyoPlayClient.GetBranchesAsync(context.CurrentScheme).ConfigureAwait(false);
                    if (!ResponseValidator.TryValidateWithoutUINotification(currentBranchesResponse, out GameBranchesWrapper? currentBranches))
                    {
                        context.Result.Kind = LaunchExecutionResultKind.GameResourceIndexQueryInvalidResponse;
                        context.Result.ErrorMessage = SH.FormatServiceGameLaunchExecutionGameResourceQueryIndexFailed(currentBranchesResponse);
                        return false;
                    }

                    Response<GameBranchesWrapper> targetBranchesResponse = await hoyoPlayClient.GetBranchesAsync(context.TargetScheme).ConfigureAwait(false);
                    if (!ResponseValidator.TryValidateWithoutUINotification(targetBranchesResponse, out GameBranchesWrapper? targetBranches))
                    {
                        context.Result.Kind = LaunchExecutionResultKind.GameResourceIndexQueryInvalidResponse;
                        context.Result.ErrorMessage = SH.FormatServiceGameLaunchExecutionGameResourceQueryIndexFailed(targetBranchesResponse);
                        return false;
                    }

                    packageConverterContext = new(
                        common,
                        currentBranches.GameBranches.Single(b => b.Game.Id == context.CurrentScheme.GameId).Main,
                        targetBranches.GameBranches.Single(b => b.Game.Id == context.TargetScheme.GameId).Main);
                    break;
                default:
                    throw HutaoException.NotSupported();
            }

            IPackageConverter packageConverter = context.ServiceProvider.GetRequiredKeyedService<IPackageConverter>(type);

            if (!context.TargetScheme.ExecutableMatches(gameFileSystem.GameFileName))
            {
                if (!await packageConverter.EnsureGameResourceAsync(packageConverterContext).ConfigureAwait(false))
                {
                    context.Result.Kind = LaunchExecutionResultKind.GameResourcePackageConvertInternalError;
                    context.Result.ErrorMessage = SH.ViewModelLaunchGameEnsureGameResourceFail;
                    return false;
                }

                // We need to change the gamePath if we switched.
                string executableName = context.TargetScheme.IsOversea ? GameConstants.GenshinImpactFileName : GameConstants.YuanShenFileName;

                await context.TaskContext.SwitchToMainThreadAsync();
                context.Options.UpdateGamePath(Path.Combine(gameFolder, executableName));
            }

            await packageConverter.EnsureDeprecatedFilesAndSdkAsync(packageConverterContext).ConfigureAwait(false);
            return true;
        }
    }

    private static bool CheckDirectoryPermissions(string folder)
    {
        if (LocalSetting.Get(SettingKeys.OverridePackageConvertDirectoryPermissionsRequirement, false))
        {
            return true;
        }

        try
        {
            string tempFilePath = Path.Combine(folder, $"{Guid.NewGuid():N}.tmp");
            string tempFilePathMove = Path.Combine(folder, $"{Guid.NewGuid():N}.tmp");

            // Test create file
            using (SafeFileHandle handle = File.OpenHandle(tempFilePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None, preallocationSize: 32 * 1024))
            {
                // Test write file
                RandomAccess.Write(handle, "SNAP HUTAO DIRECTORY PERMISSION CHECK"u8, 0);
                RandomAccess.FlushToDisk(handle);
            }

            // Test move file
            File.Move(tempFilePath, tempFilePathMove);

            // Test delete file
            File.Delete(tempFilePathMove);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
