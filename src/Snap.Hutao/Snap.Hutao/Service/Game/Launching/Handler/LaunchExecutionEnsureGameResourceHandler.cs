// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using JetBrains.Annotations;
using Microsoft.Win32.SafeHandles;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Progress;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Service.Game.Package;
using Snap.Hutao.Service.Game.Package.Advanced;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.ChannelSDK;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.DeprecatedFile;
using Snap.Hutao.Web.Response;
using System.IO;
using System.Net.Http;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionEnsureGameResourceHandler : ILaunchExecutionDelegateHandler
{
    public ValueTask<bool> BeforeExecutionAsync(LaunchExecutionContext context, BeforeExecutionDelegate next)
    {
        return next();
    }

    public async ValueTask ExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (!context.TryGetGameFileSystem(out IGameFileSystemView? gameFileSystemView))
        {
            return;
        }

        if (ShouldConvert(context, gameFileSystemView))
        {
            using (IServiceScope scope = context.ServiceProvider.CreateScope())
            {
                IContentDialogFactory contentDialogFactory = scope.ServiceProvider.GetRequiredService<IContentDialogFactory>();
                LaunchGamePackageConvertDialog dialog = await contentDialogFactory.CreateInstanceAsync<LaunchGamePackageConvertDialog>(scope.ServiceProvider).ConfigureAwait(false);
                using (await contentDialogFactory.BlockAsync(dialog).ConfigureAwait(false))
                {
                    IProgress<PackageConvertStatus> convertProgress = scope.ServiceProvider
                        .GetRequiredService<IProgressFactory>()
                        .CreateForMainThread<PackageConvertStatus, LaunchGamePackageConvertDialog>(static (state, dialog) => dialog.State = state, dialog);

                    if (!await EnsureGameResourceAsync(context, gameFileSystemView, convertProgress).ConfigureAwait(false))
                    {
                        // context.Result is set in EnsureGameResourceAsync
                        return;
                    }
                }
            }
        }

        await next().ConfigureAwait(false);
    }

    private static bool ShouldConvert(LaunchExecutionContext context, IGameFileSystemView gameFileSystemView)
    {
        // Configuration file changed
        if (context.ChannelOptionsChanged)
        {
            return true;
        }

        if (context.TargetScheme.IsOversea ^ gameFileSystemView.IsExecutableOversea())
        {
            return true;
        }

        if (!context.TargetScheme.IsOversea)
        {
            // [It's Bilibili channel xor PCGameSDK.dll exists] means we need to convert
            if (context.TargetScheme.Channel is ChannelType.Bili ^ File.Exists(gameFileSystemView.GetPcGameSdkFilePath()))
            {
                return true;
            }
        }

        return false;
    }

    private static async ValueTask<bool> EnsureGameResourceAsync(LaunchExecutionContext context, IGameFileSystemView gameFileSystemView, IProgress<PackageConvertStatus> progress)
    {
        string gameFolder = gameFileSystemView.GetGameDirectory();

        if (!CheckDirectoryPermissions(gameFolder))
        {
            context.Result.Kind = LaunchExecutionResultKind.GameDirectoryInsufficientPermissions;
            context.Result.ErrorMessage = SH.ServiceGameEnsureGameResourceInsufficientDirectoryPermissions;
            return false;
        }

        progress.Report(new(SH.ServiceGameEnsureGameResourceQueryResourceInformation));
        HoyoPlayClient hoyoPlayClient = context.ServiceProvider.GetRequiredService<HoyoPlayClient>();

        if (await TryGetCurrentBranchesAsync(hoyoPlayClient, context).ConfigureAwait(false) is not (true, { } currentBranches))
        {
            return false;
        }

        if (await TryGetTargetBranchesAsync(hoyoPlayClient, context).ConfigureAwait(false) is not (true, { } targetBranches))
        {
            return false;
        }

        if (await TryGetChannelSdkAsync(hoyoPlayClient, context).ConfigureAwait(false) is not (true, { } channelSdks))
        {
            return false;
        }

        if (await TryGetDeprecatedFileConfigurationsAsync(hoyoPlayClient, context).ConfigureAwait(false) is not (true, { } deprecatedFileConfigs))
        {
            return false;
        }

        IHttpClientFactory httpClientFactory = context.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        using (HttpClient httpClient = httpClientFactory.CreateClient(GamePackageService.HttpClientName))
        {
            string currentGameId = context.CurrentScheme.GameId;
            string targetGameId = context.TargetScheme.GameId;
            PackageConverterContext converterContext = new(
                new(httpClient, context.CurrentScheme, context.TargetScheme, gameFileSystemView, progress),
                currentBranches.GameBranches.Single(b => b.Game.Id == currentGameId).Main,
                targetBranches.GameBranches.Single(b => b.Game.Id == targetGameId).Main);

            IPackageConverter packageConverter = context.ServiceProvider.GetRequiredService<IPackageConverter>();

            // Executable does not match the target scheme
            if (context.TargetScheme.IsOversea ^ gameFileSystemView.IsExecutableOversea())
            {
                if (!await packageConverter.EnsureGameResourceAsync(converterContext).ConfigureAwait(false))
                {
                    context.Result.Kind = LaunchExecutionResultKind.GameResourcePackageConvertInternalError;
                    context.Result.ErrorMessage = SH.ViewModelLaunchGameEnsureGameResourceFail;
                    return false;
                }

                // We need to change the gamePath.
                await context.TaskContext.SwitchToMainThreadAsync();
                string executableName = context.TargetScheme.IsOversea ? GameConstants.GenshinImpactFileName : GameConstants.YuanShenFileName;
                context.UpdateGamePath(Path.Combine(gameFolder, executableName));

                // After UpdateGamePath the GameFileSystem is no longer valid.
                if (!context.TryGetGameFileSystem(out gameFileSystemView!))
                {
                    return false;
                }
            }

            PackageConverterDeprecationContext deprecationContext = new(httpClient, gameFileSystemView, channelSdks.GameChannelSDKs.SingleOrDefault(), deprecatedFileConfigs.DeprecatedFileConfigurations.SingleOrDefault());
            await packageConverter.EnsureDeprecatedFilesAndSdkAsync(deprecationContext).ConfigureAwait(false);
            return true;
        }
    }

    private static async ValueTask<ValueResult<bool, T>> TryGetAsync<T>(HoyoPlayClient hoyoPlayClient, LaunchExecutionResult result, LaunchScheme scheme, [RequireStaticDelegate] Func<HoyoPlayClient, LaunchScheme, ValueTask<Response<T>>> asyncMethod)
    {
        Response<T> response = await asyncMethod(hoyoPlayClient, scheme).ConfigureAwait(false);
        if (!ResponseValidator.TryValidateWithoutUINotification(response, out T? data))
        {
            result.Kind = LaunchExecutionResultKind.GameResourceIndexQueryInvalidResponse;
            result.ErrorMessage = SH.FormatServiceGameLaunchExecutionGameResourceQueryIndexFailed(response);
            return new(false, default!);
        }

        return new(true, data);
    }

    private static ValueTask<ValueResult<bool, GameBranchesWrapper>> TryGetCurrentBranchesAsync(HoyoPlayClient hoyoPlayClient, LaunchExecutionContext context)
    {
        return TryGetAsync(hoyoPlayClient, context.Result, context.CurrentScheme, static (client, scheme) => client.GetBranchesAsync(scheme));
    }

    private static ValueTask<ValueResult<bool, GameBranchesWrapper>> TryGetTargetBranchesAsync(HoyoPlayClient hoyoPlayClient, LaunchExecutionContext context)
    {
        return TryGetAsync(hoyoPlayClient, context.Result, context.TargetScheme, static (client, scheme) => client.GetBranchesAsync(scheme));
    }

    private static ValueTask<ValueResult<bool, GameChannelSDKsWrapper>> TryGetChannelSdkAsync(HoyoPlayClient hoyoPlayClient, LaunchExecutionContext context)
    {
        return TryGetAsync(hoyoPlayClient, context.Result, context.TargetScheme, static (client, scheme) => client.GetChannelSDKAsync(scheme));
    }

    private static ValueTask<ValueResult<bool, DeprecatedFileConfigurationsWrapper>> TryGetDeprecatedFileConfigurationsAsync(HoyoPlayClient hoyoPlayClient, LaunchExecutionContext context)
    {
        return TryGetAsync(hoyoPlayClient, context.Result, context.TargetScheme, static (client, scheme) => client.GetDeprecatedFileConfigurationsAsync(scheme));
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