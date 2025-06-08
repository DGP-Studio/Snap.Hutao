// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Hutao.Response;
using Snap.Hutao.Web.Response;
using System.Diagnostics;
using System.IO;

namespace Snap.Hutao.Service.Update;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IUpdateService))]
internal sealed partial class UpdateService : IUpdateService
{
    private const string UpdaterFilename = "Snap.Hutao.Deployment.exe";

    // Avoid injecting services directly
    private readonly IServiceProvider serviceProvider;

    public async ValueTask<CheckUpdateResult> CheckUpdateAsync(CancellationToken token = default)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            ITaskContext taskContext = scope.ServiceProvider.GetRequiredService<ITaskContext>();
            await taskContext.SwitchToBackgroundAsync();

            HutaoInfrastructureClient infrastructureClient = scope.ServiceProvider.GetRequiredService<HutaoInfrastructureClient>();
            HutaoResponse<HutaoPackageInformation> response = await infrastructureClient.GetHutaoVersionInformationAsync(token).ConfigureAwait(false);

            CheckUpdateResult checkUpdateResult = new();

            if (!ResponseValidator.TryValidate(response, scope.ServiceProvider, out HutaoPackageInformation? packageInformation))
            {
                checkUpdateResult.Kind = CheckUpdateResultKind.VersionApiInvalidResponse;
                return checkUpdateResult;
            }

            checkUpdateResult.Kind = CheckUpdateResultKind.UpdateAvailable;
            checkUpdateResult.PackageInformation = packageInformation;

            if (!LocalSetting.Get(SettingKeys.OverrideUpdateVersionComparison, false))
            {
                // Launched in an updated version
                if (HutaoRuntime.Version >= checkUpdateResult.PackageInformation.Version)
                {
                    checkUpdateResult.Kind = CheckUpdateResultKind.AlreadyUpdated;
                    return checkUpdateResult;
                }
            }

            if (checkUpdateResult.PackageInformation.Validation is not { Length: > 0 })
            {
                checkUpdateResult.Kind = CheckUpdateResultKind.VersionApiInvalidSha256;
            }

            return checkUpdateResult;
        }
    }

    public async ValueTask<ValueResult<bool, Exception>> LaunchUpdaterAsync()
    {
        string updaterTargetPath;
        try
        {
            updaterTargetPath = HutaoRuntime.GetDataFolderUpdateCacheFolderFile(UpdaterFilename);
        }
        catch (UnauthorizedAccessException ex)
        {
            // Access to the path '?' is denied.
            return new(false, ex);
        }

        try
        {
            InstalledLocation.CopyFileFromApplicationUri($"ms-appx:///{UpdaterFilename}", updaterTargetPath);
        }
        catch (IOException ex)
        {
            // 0x80070002 无法启动服务，原因可能是已被禁用或与其相关联的设备没有启动
            // The process cannot access the file '?' because it is being used by another process.
            return new(false, ex);
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HutaoUserOptions hutaoUserOptions = scope.ServiceProvider.GetRequiredService<HutaoUserOptions>();

            string commandLine = new CommandLineBuilder()
                .Append("update", await hutaoUserOptions.GetAuthTokenAsync().ConfigureAwait(false))
                .ToString();

            try
            {
                // The updater will request UAC permissions itself
                Process.Start(new ProcessStartInfo
                {
                    Arguments = commandLine,
                    FileName = updaterTargetPath,
                    UseShellExecute = true,
                });

                return new(true, default!);
            }
            catch (Exception ex)
            {
                serviceProvider.GetRequiredService<IInfoBarService>().Error(ex);
                return new(false, ex);
            }
        }
    }
}