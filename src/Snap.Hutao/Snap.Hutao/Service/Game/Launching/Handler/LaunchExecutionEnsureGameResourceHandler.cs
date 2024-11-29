// Copyright (c) DGP Studio. All rights reserved.
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

internal sealed partial class LaunchExecutionEnsureGameResourceHandler : ILaunchExecutionDelegateHandler
{
    public async ValueTask OnExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (!context.TryGetGameFileSystem(out IGameFileSystem? gameFileSystem))
        {
            return;
        }

        UnsafeRelaxedGameFileSystemReference reference = new(gameFileSystem);

        if (ShouldConvert(context, reference))
        {
            IServiceProvider serviceProvider = context.ServiceProvider;
            IContentDialogFactory contentDialogFactory = serviceProvider.GetRequiredService<IContentDialogFactory>();

            LaunchGamePackageConvertDialog dialog = await contentDialogFactory.CreateInstanceAsync<LaunchGamePackageConvertDialog>().ConfigureAwait(false);
            using (await contentDialogFactory.BlockAsync(dialog).ConfigureAwait(false))
            {
                IProgress<PackageConvertStatus> convertProgress = serviceProvider
                    .GetRequiredService<IProgressFactory>()
                    .CreateForMainThread<PackageConvertStatus>(state => dialog.State = state);

                if (!await EnsureGameResourceAsync(context, reference, convertProgress).ConfigureAwait(false))
                {
                    // context.Result is set in EnsureGameResourceAsync
                    return;
                }

                // If EnsureGameResourceAsync succeeded, The GameFileSystem is no longer valid.
                if (!context.TryGetGameFileSystem(out gameFileSystem))
                {
                    return;
                }

                // Backup config file, recover when an incompatible launcher deleted it.
                context.ServiceProvider.GetRequiredService<IGameConfigurationFileService>()
                    .Backup(gameFileSystem.GetGameConfigurationFilePath());

                await context.TaskContext.SwitchToMainThreadAsync();
                context.PerformGamePathEntrySynchronization();
            }
        }

        await next().ConfigureAwait(false);
    }

    private static bool ShouldConvert(LaunchExecutionContext context, UnsafeRelaxedGameFileSystemReference reference)
    {
        // Configuration file changed
        if (context.ChannelOptionsChanged)
        {
            return true;
        }

        if (context.TargetScheme.IsOversea ^ reference.IsOversea())
        {
            return true;
        }

        if (!context.TargetScheme.IsOversea)
        {
            // [It's Bilibili channel xor PCGameSDK.dll exists] means we need to convert
            if (context.TargetScheme.Channel is ChannelType.Bili ^ File.Exists(reference.GetPcGameSdkFilePath()))
            {
                return true;
            }
        }

        return false;
    }

    private static async ValueTask<bool> EnsureGameResourceAsync(LaunchExecutionContext context, UnsafeRelaxedGameFileSystemReference reference, IProgress<PackageConvertStatus> progress)
    {
        string gameFolder = reference.GetGameDirectory();
        context.Logger.LogInformation("Game folder: {GameFolder}", gameFolder);

        if (!CheckDirectoryPermissions(gameFolder))
        {
            context.Result.Kind = LaunchExecutionResultKind.GameDirectoryInsufficientPermissions;
            context.Result.ErrorMessage = SH.ServiceGameEnsureGameResourceInsufficientDirectoryPermissions;
            return false;
        }

        progress.Report(new(SH.ServiceGameEnsureGameResourceQueryResourceInformation));

        HoyoPlayClient hoyoPlayClient = context.ServiceProvider.GetRequiredService<HoyoPlayClient>();

        IHttpClientFactory httpClientFactory = context.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        using (HttpClient httpClient = httpClientFactory.CreateClient(GamePackageService.HttpClientName))
        {
            PackageConverterContext.CommonReferences common = new(httpClient, context.CurrentScheme, context.TargetScheme, reference, progress);
            PackageConverterContext packageConverterContext;
            PackageConverterType type = context.ServiceProvider.GetRequiredService<AppOptions>().PackageConverterType;
            switch (type)
            {
                case PackageConverterType.ScatteredFiles:
                    if (await TryGetPackagesAsync(hoyoPlayClient, context).ConfigureAwait(false) is not (true, { } gamePackages))
                    {
                        return false;
                    }

                    packageConverterContext = new(common, gamePackages.GamePackages.Single());
                    break;
                case PackageConverterType.SophonChunks:
                    if (await TryGetCurrentBranchesAsync(hoyoPlayClient, context).ConfigureAwait(false) is not (true, { } currentBranches))
                    {
                        return false;
                    }

                    if (await TryGetTargetBranchesAsync(hoyoPlayClient, context).ConfigureAwait(false) is not (true, { } targetBranches))
                    {
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

            if (context.TargetScheme.IsOversea ^ reference.IsOversea())
            {
                if (!await packageConverter.EnsureGameResourceAsync(packageConverterContext).ConfigureAwait(false))
                {
                    context.Result.Kind = LaunchExecutionResultKind.GameResourcePackageConvertInternalError;
                    context.Result.ErrorMessage = SH.ViewModelLaunchGameEnsureGameResourceFail;
                    return false;
                }

                // The GameFileSystem no longer valid, and meanwhile we need to change the gamePath.
                string executableName = context.TargetScheme.IsOversea ? GameConstants.GenshinImpactFileName : GameConstants.YuanShenFileName;

                await context.TaskContext.SwitchToMainThreadAsync();
                context.UpdateGamePath(Path.Combine(gameFolder, executableName));

                if (!context.TryGetGameFileSystem(out IGameFileSystem? newValue))
                {
                    return false;
                }

                reference.Value = newValue;
            }

            if (await TryGetChannelSdkAsync(hoyoPlayClient, context).ConfigureAwait(false) is not (true, { } channelSdks))
            {
                return false;
            }

            if (await TryGetDeprecatedFileConfigurationsAsync(hoyoPlayClient, context).ConfigureAwait(false) is not (true, { } deprecatedFileConfigs))
            {
                return false;
            }

            PackageConverterDeprecationContext deprecationContext = new(httpClient, reference, channelSdks.GameChannelSDKs.SingleOrDefault(), deprecatedFileConfigs.DeprecatedFileConfigurations.SingleOrDefault());
            await packageConverter.EnsureDeprecatedFilesAndSdkAsync(deprecationContext).ConfigureAwait(false);
            return true;
        }
    }

    private static async ValueTask<ValueResult<bool, GameChannelSDKsWrapper>> TryGetChannelSdkAsync(HoyoPlayClient hoyoPlayClient, LaunchExecutionContext context)
    {
        Response<GameChannelSDKsWrapper> response = await hoyoPlayClient.GetChannelSDKAsync(context.TargetScheme).ConfigureAwait(false);
        if (!ResponseValidator.TryValidateWithoutUINotification(response, out GameChannelSDKsWrapper? data))
        {
            context.Result.Kind = LaunchExecutionResultKind.GameResourceIndexQueryInvalidResponse;
            context.Result.ErrorMessage = SH.FormatServiceGameLaunchExecutionGameResourceQueryIndexFailed(response);
            return new(false, default!);
        }

        return new(true, data);
    }

    private static async ValueTask<ValueResult<bool, DeprecatedFileConfigurationsWrapper>> TryGetDeprecatedFileConfigurationsAsync(HoyoPlayClient hoyoPlayClient, LaunchExecutionContext context)
    {
        Response<DeprecatedFileConfigurationsWrapper> response = await hoyoPlayClient.GetDeprecatedFileConfigurationsAsync(context.TargetScheme).ConfigureAwait(false);
        if (!ResponseValidator.TryValidateWithoutUINotification(response, out DeprecatedFileConfigurationsWrapper? data))
        {
            context.Result.Kind = LaunchExecutionResultKind.GameResourceIndexQueryInvalidResponse;
            context.Result.ErrorMessage = SH.FormatServiceGameLaunchExecutionGameResourceQueryIndexFailed(response);
            return new(false, default!);
        }

        return new(true, data);
    }

    private static async ValueTask<ValueResult<bool, GamePackagesWrapper>> TryGetPackagesAsync(HoyoPlayClient hoyoPlayClient, LaunchExecutionContext context)
    {
        Response<GamePackagesWrapper> response = await hoyoPlayClient.GetPackagesAsync(context.TargetScheme).ConfigureAwait(false);
        if (!ResponseValidator.TryValidateWithoutUINotification(response, out GamePackagesWrapper? data))
        {
            context.Result.Kind = LaunchExecutionResultKind.GameResourceIndexQueryInvalidResponse;
            context.Result.ErrorMessage = SH.FormatServiceGameLaunchExecutionGameResourceQueryIndexFailed(response);
            return new(false, default!);
        }

        return new(true, data);
    }

    private static async ValueTask<ValueResult<bool, GameBranchesWrapper>> TryGetCurrentBranchesAsync(HoyoPlayClient hoyoPlayClient, LaunchExecutionContext context)
    {
        Response<GameBranchesWrapper> response = await hoyoPlayClient.GetBranchesAsync(context.CurrentScheme).ConfigureAwait(false);
        if (!ResponseValidator.TryValidateWithoutUINotification(response, out GameBranchesWrapper? data))
        {
            context.Result.Kind = LaunchExecutionResultKind.GameResourceIndexQueryInvalidResponse;
            context.Result.ErrorMessage = SH.FormatServiceGameLaunchExecutionGameResourceQueryIndexFailed(response);
            return new(false, default!);
        }

        return new(true, data);
    }

    private static async ValueTask<ValueResult<bool, GameBranchesWrapper>> TryGetTargetBranchesAsync(HoyoPlayClient hoyoPlayClient, LaunchExecutionContext context)
    {
        Response<GameBranchesWrapper> response = await hoyoPlayClient.GetBranchesAsync(context.TargetScheme).ConfigureAwait(false);
        if (!ResponseValidator.TryValidateWithoutUINotification(response, out GameBranchesWrapper? data))
        {
            context.Result.Kind = LaunchExecutionResultKind.GameResourceIndexQueryInvalidResponse;
            context.Result.ErrorMessage = SH.FormatServiceGameLaunchExecutionGameResourceQueryIndexFailed(response);
            return new(false, default!);
        }

        return new(true, data);
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

    private sealed partial class UnsafeRelaxedGameFileSystemReference : IGameFileSystem
    {
        public UnsafeRelaxedGameFileSystemReference(IGameFileSystem value)
        {
            Value = value;
        }

        public IGameFileSystem Value
        {
            private get;
            set
            {
                field?.Dispose();
                field = value;
            }
        }

        public string GameFilePath { get => Value.GameFilePath; }

        public GameAudioSystem Audio { get => Value.Audio; }

        public bool IsDisposed { get => Value.IsDisposed; }

        public void Dispose()
        {
            Value?.Dispose();
        }
    }
}