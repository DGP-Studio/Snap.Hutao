// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32.SafeHandles;
using Snap.Hutao.Control.Extension;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Progress;
using Snap.Hutao.Service.Game.Package;
using Snap.Hutao.Service.Game.PathAbstraction;
using Snap.Hutao.View.Dialog;
using Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;
using System.IO;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionEnsureGameResourceHandler : ILaunchExecutionDelegateHandler
{
    public async ValueTask OnExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        IServiceProvider serviceProvider = context.ServiceProvider;
        IContentDialogFactory contentDialogFactory = serviceProvider.GetRequiredService<IContentDialogFactory>();
        IProgressFactory progressFactory = serviceProvider.GetRequiredService<IProgressFactory>();

        LaunchGamePackageConvertDialog dialog = await contentDialogFactory.CreateInstanceAsync<LaunchGamePackageConvertDialog>().ConfigureAwait(false);
        IProgress<PackageConvertStatus> convertProgress = progressFactory.CreateForMainThread<PackageConvertStatus>(state => dialog.State = state);

        using (await dialog.BlockAsync(context.TaskContext).ConfigureAwait(false))
        {
            if (!await EnsureGameResourceAsync(context, convertProgress).ConfigureAwait(false))
            {
                // context.Result is set in EnsureGameResourceAsync
                return;
            }

            await context.TaskContext.SwitchToMainThreadAsync();
            ImmutableList<GamePathEntry> gamePathEntries = context.Options.GetGamePathEntries(out GamePathEntry? selected);
            context.ViewModel.SetGamePathEntriesAndSelectedGamePathEntry(gamePathEntries, selected);
        }

        await next().ConfigureAwait(false);
    }

    private static async ValueTask<bool> EnsureGameResourceAsync(LaunchExecutionContext context, IProgress<PackageConvertStatus> progress)
    {
        if (!context.Options.TryGetGameDirectoryAndGameFileName(out string? gameFolder, out string? gameFileName))
        {
            context.Result.Kind = LaunchExecutionResultKind.NoActiveGamePath;
            context.Result.ErrorMessage = SH.ServiceGameLaunchExecutionGamePathNotValid;
            return false;
        }

        context.Logger.LogInformation("Game folder: {GameFolder}", gameFolder);

        if (!CheckDirectoryPermissions(gameFolder))
        {
            context.Result.Kind = LaunchExecutionResultKind.GameDirectoryInsufficientPermissions;
            context.Result.ErrorMessage = SH.ServiceGameEnsureGameResourceInsufficientDirectoryPermissions;
            return false;
        }

        progress.Report(new(SH.ServiceGameEnsureGameResourceQueryResourceInformation));

        ResourceClient resourceClient = context.ServiceProvider.GetRequiredService<ResourceClient>();
        Response<GameResource> response = await resourceClient.GetResourceAsync(context.Scheme).ConfigureAwait(false);

        if (!response.TryGetDataWithoutUINotification(out GameResource? resource))
        {
            context.Result.Kind = LaunchExecutionResultKind.GameResourceIndexQueryInvalidResponse;
            context.Result.ErrorMessage = SH.FormatServiceGameLaunchExecutionGameResourceQueryIndexFailed(response);
            return false;
        }

        PackageConverter packageConverter = context.ServiceProvider.GetRequiredService<PackageConverter>();

        if (!context.Scheme.ExecutableMatches(gameFileName))
        {
            if (!await packageConverter.EnsureGameResourceAsync(context.Scheme, resource, gameFolder, progress).ConfigureAwait(false))
            {
                context.Result.Kind = LaunchExecutionResultKind.GameResourcePackageConvertInternalError;
                context.Result.ErrorMessage = SH.ViewModelLaunchGameEnsureGameResourceFail;
                return false;
            }

            // We need to change the gamePath if we switched.
            string executableName = context.Scheme.IsOversea ? GameConstants.GenshinImpactFileName : GameConstants.YuanShenFileName;

            await context.TaskContext.SwitchToMainThreadAsync();
            context.Options.UpdateGamePathAndRefreshEntries(Path.Combine(gameFolder, executableName));
        }

        await packageConverter.EnsureDeprecatedFilesAndSdkAsync(resource, gameFolder).ConfigureAwait(false);
        return true;
    }

    private static bool CheckDirectoryPermissions(string folder)
    {
        // Program Files has special permissions limitation.
        string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        if (folder.StartsWith(programFiles, StringComparison.OrdinalIgnoreCase))
        {
            return false;
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