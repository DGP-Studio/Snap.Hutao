// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32.SafeHandles;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Service.Game.FileSystem;
using Snap.Hutao.Service.Game.Launching.Context;
using Snap.Hutao.Service.Game.Package;
using Snap.Hutao.Service.Game.Package.Advanced;
using System.IO;
using System.Net.Http;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionEnsureGameResourceHandler : AbstractLaunchExecutionHandler
{
    public override async ValueTask BeforeAsync(BeforeLaunchExecutionContext context)
    {
        if (!ShouldConvert(context))
        {
            return;
        }

        using (BlockDeferralWithProgress<PackageConvertStatus> deferral = await context.ViewModel.CreateConvertBlockDeferralAsync().ConfigureAwait(false))
        {
            await EnsureGameResourceAsync(context, deferral.Progress).ConfigureAwait(false);
        }
    }

    private static bool ShouldConvert(BeforeLaunchExecutionContext context)
    {
        // Configuration file changed
        if (context.TryGetOption(LaunchExecutionOptionsKey.ChannelOptionsChanged, out bool changed) && changed)
        {
            return true;
        }

        if (context.TargetScheme.IsOversea ^ context.FileSystem.IsExecutableOversea())
        {
            return true;
        }

        if (!context.TargetScheme.IsOversea)
        {
            // [It's Bilibili channel xor PCGameSDK.dll exists] means we need to convert
            if (context.TargetScheme.Channel is ChannelType.Bili ^ File.Exists(context.FileSystem.GetPCGameSDKFilePath()))
            {
                return true;
            }
        }

        return false;
    }

    private static async ValueTask EnsureGameResourceAsync(BeforeLaunchExecutionContext context, IProgress<PackageConvertStatus> progress)
    {
        string gameFolder = context.FileSystem.GetGameDirectory();

        if (!CheckDirectoryPermissions(gameFolder, out Exception? inner))
        {
            throw HutaoException.UnauthorizedAccess(SH.ServiceGameEnsureGameResourceInsufficientDirectoryPermissions, inner);
        }

        progress.Report(new(SH.ServiceGameEnsureGameResourceQueryResourceInformation));

        if (await context.HoyoPlay.TryGetBranchesAsync(context.CurrentScheme).ConfigureAwait(false) is not (true, { } currentBranches))
        {
            throw HutaoException.InvalidOperation(SH.FormatServiceGameLaunchExecutionGameResourceQueryIndexFailed("Current Branches"));
        }

        if (await context.HoyoPlay.TryGetBranchesAsync(context.TargetScheme).ConfigureAwait(false) is not (true, { } targetBranches))
        {
            throw HutaoException.InvalidOperation(SH.FormatServiceGameLaunchExecutionGameResourceQueryIndexFailed("Target Branches"));
        }

        if (await context.HoyoPlay.TryGetChannelSDKsAsync(context.TargetScheme).ConfigureAwait(false) is not (true, { } channelSdks))
        {
            throw HutaoException.InvalidOperation(SH.FormatServiceGameLaunchExecutionGameResourceQueryIndexFailed("Target Channel SDKs"));
        }

        if (await context.HoyoPlay.TryGetDeprecatedFileConfigurationsAsync(context.TargetScheme).ConfigureAwait(false) is not (true, { } deprecatedFileConfigs))
        {
            throw HutaoException.InvalidOperation(SH.FormatServiceGameLaunchExecutionGameResourceQueryIndexFailed("Target Deprecated File Configs"));
        }

        IHttpClientFactory httpClientFactory = context.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        using (HttpClient httpClient = httpClientFactory.CreateClient(GamePackageService.HttpClientName))
        {
            PackageConverterContext converterContext = new(context.CurrentScheme, context.TargetScheme, context.FileSystem)
            {
                HttpClient = httpClient,
                Progress = progress,
                CurrentBranch = currentBranches.GetMainBranch(context.CurrentScheme.GameId),
                TargetBranch = targetBranches.GetMainBranch(context.TargetScheme.GameId),
            };

            IPackageConverter packageConverter = context.ServiceProvider.GetRequiredService<IPackageConverter>();

            // Executable does not match the target scheme
            if (context.TargetScheme.IsOversea ^ context.FileSystem.IsExecutableOversea())
            {
                if (!await packageConverter.EnsureGameResourceAsync(converterContext).ConfigureAwait(false))
                {
                    throw HutaoException.InvalidOperation(SH.ViewModelLaunchGameEnsureGameResourceFail);
                }

                // We need to change the gamePath.
                await context.TaskContext.SwitchToMainThreadAsync();
                string executableName = context.TargetScheme.IsOversea ? GameConstants.GenshinImpactFileName : GameConstants.YuanShenFileName;
                context.FileSystem = context.LaunchOptions.UnsafeForceUpdateGamePath(Path.Combine(gameFolder, executableName), context.FileSystem);
            }

            PackageConverterDeprecationContext deprecationContext = new(httpClient, context.FileSystem, channelSdks.GameChannelSDKs.SingleOrDefault(), deprecatedFileConfigs.DeprecatedFileConfigurations.SingleOrDefault());
            await packageConverter.EnsureDeprecatedFilesAndSDKAsync(deprecationContext).ConfigureAwait(false);
        }
    }

    private static bool CheckDirectoryPermissions(string folder, [NotNullWhen(false)] out Exception? exception)
    {
        if (!LocalSetting.Get(SettingKeys.OverridePackageConvertDirectoryPermissionsRequirement, false))
        {
            try
            {
                Directory.CreateDirectory(folder);

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
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        exception = null;
        return true;
    }
}