// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.DataTransfer;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.Graphics;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Picker;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Package.Advanced;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml;
using Snap.Hutao.UI.Xaml.View.Window;
using Snap.Hutao.ViewModel.Game;
using Snap.Hutao.ViewModel.Guide;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;
using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;
using Snap.Hutao.Web.Hutao.HutaoAsAService;
using Snap.Hutao.Web.Hutao.Redeem;
using Snap.Hutao.Web.Hutao.Response;
using Snap.Hutao.Web.Response;
using System.IO;
using System.Text.RegularExpressions;

// ReSharper disable LocalizableElement
namespace Snap.Hutao.ViewModel;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class TestViewModel : Abstraction.ViewModel
{
    private readonly IFileSystemPickerInteraction fileSystemPickerInteraction;
    private readonly ICurrentXamlWindowReference currentXamlWindowReference;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IClipboardProvider clipboardProvider;
    private readonly IServiceProvider serviceProvider;
    private readonly IInfoBarService infoBarService;
    private readonly ILogger<TestViewModel> logger;
    private readonly ITaskContext taskContext;

    public UploadAnnouncement Announcement { get; set => SetProperty(ref field, value); } = new();

    public ExtractOptions ExtractExeOptions { get; } = new();

    public int GachaLogCompensationDays { get; set; } = 15;

    public DesignationOptions GachaLogDesignationOptions { get; } = new();

    public int CdnCompensationDays { get; set; } = 15;

    public DesignationOptions CdnDesignationOptions { get; } = new();

    public RedeemCodeGenerateOptions RedeemCodeGenerateOption { get; set => SetProperty(ref field, value); } = new();

    public bool SuppressMetadataInitialization
    {
        get => LocalSetting.Get(SettingKeys.SuppressMetadataInitialization, false);
        set => LocalSetting.SetIfNot(IsViewDisposed, SettingKeys.SuppressMetadataInitialization, value);
    }

    public bool OverrideElevationRequirement
    {
        get => LocalSetting.Get(SettingKeys.OverrideElevationRequirement, false);
        set => LocalSetting.SetIfNot(IsViewDisposed, SettingKeys.OverrideElevationRequirement, value);
    }

    public bool OverrideUpdateVersionComparison
    {
        get => LocalSetting.Get(SettingKeys.OverrideUpdateVersionComparison, false);
        set => LocalSetting.SetIfNot(IsViewDisposed, SettingKeys.OverrideUpdateVersionComparison, value);
    }

    public bool OverridePackageConvertDirectoryPermissionsRequirement
    {
        get => LocalSetting.Get(SettingKeys.OverridePackageConvertDirectoryPermissionsRequirement, false);
        set => LocalSetting.SetIfNot(IsViewDisposed, SettingKeys.OverridePackageConvertDirectoryPermissionsRequirement, value);
    }

    public bool OverrideHardDriveType
    {
        get => LocalSetting.Get(SettingKeys.OverridePhysicalDriverType, false);
        set => LocalSetting.SetIfNot(IsViewDisposed, SettingKeys.OverridePhysicalDriverType, value);
    }

    public bool OverrideHardDriveTypeIsSolidState
    {
        get => LocalSetting.Get(SettingKeys.PhysicalDriverIsAlwaysSolidState, false);
        set => LocalSetting.SetIfNot(IsViewDisposed, SettingKeys.PhysicalDriverIsAlwaysSolidState, value);
    }

    public bool AlwaysIsFirstRunAfterUpdate
    {
        get => LocalSetting.Get(SettingKeys.AlwaysIsFirstRunAfterUpdate, false);
        set => LocalSetting.SetIfNot(IsViewDisposed, SettingKeys.AlwaysIsFirstRunAfterUpdate, value);
    }

    public bool AlphaBuildUseCNPatchEndpoint
    {
        get => LocalSetting.Get(SettingKeys.AlphaBuildUseCnPatchEndpoint, false);
        set => LocalSetting.SetIfNot(IsViewDisposed, SettingKeys.AlphaBuildUseCnPatchEndpoint, value);
    }

    public bool AlphaBuildUseFJPatchEndpoint
    {
        get => LocalSetting.Get(SettingKeys.AlphaBuildUseFjPatchEndpoint, false);
        set => LocalSetting.SetIfNot(IsViewDisposed, SettingKeys.AlphaBuildUseFjPatchEndpoint, value);
    }

    [GeneratedRegex(@"AssetBundles.*\.blk$", RegexOptions.IgnoreCase)]
    private static partial Regex AssetBundlesBlockRegex { get; }

    [GeneratedRegex(@"^(Yuanshen|GenshinImpact)\.exe$", RegexOptions.IgnoreCase)]
    private static partial Regex GameExecutableFileRegex { get; }

    [Command("ResetGuideStateCommand")]
    private static void ResetGuideState()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Reset guide state", "TestViewModel.Command"));
        UnsafeLocalSetting.Set(SettingKeys.GuideState, GuideState.Language);
    }

    [Command("ExceptionCommand")]
    private static void ThrowTestException()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Throw test exception", "TestViewModel.Command"));

        InvalidOperationException inner = new()
        {
            HResult = 0x12345678,
        };
        HutaoException.Throw("Test Exception", inner);
    }

    [Command("FileOperationRenameCommand")]
    private static void FileOperationRename()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Test file operation rename", "TestViewModel.Command"));

        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string source = Path.Combine(desktop, "TestFolder");
        DirectoryOperation.UnsafeRename(source, "TestFolder1");
    }

    [Command("ResetMainWindowSizeCommand")]
    private void ResetMainWindowSize()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Resize MainWindow", "TestViewModel.Command"));

        if (currentXamlWindowReference.Window is MainWindow mainWindow)
        {
            double scale = mainWindow.GetRasterizationScale();
            mainWindow.AppWindow.Resize(new Windows.Graphics.SizeInt32(1372, 772).Scale(scale));
        }
    }

    [Command("UploadAnnouncementCommand")]
    private async Task UploadAnnouncementAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Upload Hutao announcement", "TestViewModel.Command"));

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HutaoAsAServiceClient hutaoAsAServiceClient = scope.ServiceProvider.GetRequiredService<HutaoAsAServiceClient>();
            Response response = await hutaoAsAServiceClient.UploadAnnouncementAsync(Announcement).ConfigureAwait(false);
            if (ResponseValidator.TryValidate(response, scope.ServiceProvider))
            {
                infoBarService.Success(response.Message);
                await taskContext.SwitchToMainThreadAsync();
                Announcement = new();
            }
        }
    }

    [Command("CompensationGachaLogServiceTimeCommand")]
    private async Task CompensationGachaLogServiceTimeAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Compensation gachalog service time", "TestViewModel.Command"));

        ContentDialogResult result = await contentDialogFactory.CreateForConfirmCancelAsync(
            "Hutao Cloud",
            $"Compensation Gacha Log Service Time For {GachaLogCompensationDays} Days?").ConfigureAwait(false);

        if (result is not ContentDialogResult.Primary)
        {
            return;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HutaoAsAServiceClient hutaoAsAServiceClient = scope.ServiceProvider.GetRequiredService<HutaoAsAServiceClient>();
            Response response = await hutaoAsAServiceClient.GachaLogCompensationAsync(GachaLogCompensationDays).ConfigureAwait(false);
            if (ResponseValidator.TryValidate(response, scope.ServiceProvider))
            {
                infoBarService.Success(response.Message);
            }
        }
    }

    [Command("DesignationGachaLogServiceTimeCommand")]
    private async Task DesignationGachaLogServiceTimeAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Designation gachalog service time", "TestViewModel.Command"));

        ContentDialogResult result = await contentDialogFactory.CreateForConfirmCancelAsync(
            "Hutao Cloud",
            $"Designation Gacha Log Service Time To {GachaLogDesignationOptions.UserName} For {GachaLogDesignationOptions.Days} Days?").ConfigureAwait(false);

        if (result is not ContentDialogResult.Primary)
        {
            return;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HutaoAsAServiceClient hutaoAsAServiceClient = scope.ServiceProvider.GetRequiredService<HutaoAsAServiceClient>();
            Response response = await hutaoAsAServiceClient.GachaLogDesignationAsync(GachaLogDesignationOptions.UserName, GachaLogDesignationOptions.Days).ConfigureAwait(false);
            if (ResponseValidator.TryValidate(response, scope.ServiceProvider))
            {
                infoBarService.Success(response.Message);
            }
        }
    }

    [Command("CompensationCdnServiceTimeCommand")]
    private async Task CompensationCdnServiceTimeAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Compensation CDN service time", "TestViewModel.Command"));

        ContentDialogResult result = await contentDialogFactory.CreateForConfirmCancelAsync(
            "Hutao Cloud",
            $"Compensation CDN Service Time For {CdnCompensationDays} Days?").ConfigureAwait(false);

        if (result is not ContentDialogResult.Primary)
        {
            return;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HutaoAsAServiceClient hutaoAsAServiceClient = scope.ServiceProvider.GetRequiredService<HutaoAsAServiceClient>();
            Response response = await hutaoAsAServiceClient.CdnCompensationAsync(CdnCompensationDays).ConfigureAwait(false);
            if (ResponseValidator.TryValidate(response, scope.ServiceProvider))
            {
                infoBarService.Success(response.Message);
            }
        }
    }

    [Command("DesignationCdnServiceTimeCommand")]
    private async Task DesignationCdnServiceTimeAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Designation CDN service time", "TestViewModel.Command"));

        ContentDialogResult result = await contentDialogFactory.CreateForConfirmCancelAsync(
            "Hutao Cloud",
            $"Designation CDN Service Time To {CdnDesignationOptions.UserName} For {CdnDesignationOptions.Days} Days?").ConfigureAwait(false);

        if (result is not ContentDialogResult.Primary)
        {
            return;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HutaoAsAServiceClient hutaoAsAServiceClient = scope.ServiceProvider.GetRequiredService<HutaoAsAServiceClient>();
            Response response = await hutaoAsAServiceClient.CdnDesignationAsync(CdnDesignationOptions.UserName, CdnDesignationOptions.Days).ConfigureAwait(false);
            if (ResponseValidator.TryValidate(response, scope.ServiceProvider))
            {
                infoBarService.Success(response.Message);
            }
        }
    }

    [Command("SendRandomInfoBarNotificationCommand")]
    private void SendRandomInfoBarNotification()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Send random infobar notification", "TestViewModel.Command"));

        infoBarService.PrepareInfoBarAndShow(builder => builder
            .SetSeverity((InfoBarSeverity)Random.Shared.Next((int)InfoBarSeverity.Error) + 1)
            .SetTitle("Lorem ipsum dolor sit amet")
            .SetMessage("Consectetur adipiscing elit. Nullam nec purus nec elit ultricies tincidunt. Donec nec sapien nec elit ultricies tincidunt. Donec nec sapien nec elit ultricies tincidunt."));
    }

    [Command("CheckPathBelongsToSSDCommand")]
    private void CheckPathBelongsToSSD()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Test SSD path", "TestViewModel.Command"));

        (bool isOk, ValueFile file) = fileSystemPickerInteraction.PickFile("Pick any file!", default, default);
        if (isOk)
        {
            bool isSolidState = PhysicalDrive.DangerousGetIsSolidState(file);
            infoBarService.Success($"The path '{file}' belongs to a {(isSolidState ? "solid state" : "hard disk")} drive.");
        }
    }

    [Command("RunCodeCommand")]
    private async Task RunCodeAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Test run code", "TestViewModel.Command"));

        try
        {
            string script = """
                            return 1 + 1;
                            """;
            object? result = await CSharpScript.EvaluateAsync(script, ScriptOptions.Default).ConfigureAwait(false);
            logger.LogInformation("Run Code Result: '{Result}'", result);
        }
        catch (CompilationErrorException ex)
        {
            logger.LogCritical(ex, "Compilation Error");
        }
    }

    [Command("ExtractGameBlocksCommand")]
    private async Task ExtractGameBlocksAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Test extract game blocks", "TestViewModel.Command"));

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IGamePackageService gamePackageService = scope.ServiceProvider.GetRequiredService<IGamePackageService>();
            LaunchGameShared launchGameShared = scope.ServiceProvider.GetRequiredService<LaunchGameShared>();
            HoyoPlayClient hoyoPlayClient = scope.ServiceProvider.GetRequiredService<HoyoPlayClient>();
            LaunchOptions launchOptions = scope.ServiceProvider.GetRequiredService<LaunchOptions>();

            if (launchGameShared.GetCurrentLaunchSchemeFromConfigFile() is not { } launchScheme)
            {
                return;
            }

            Response<GameBranchesWrapper> branchResp = await hoyoPlayClient.GetBranchesAsync(launchScheme).ConfigureAwait(false);
            if (!ResponseValidator.TryValidate(branchResp, serviceProvider, out GameBranchesWrapper? branchesWrapper))
            {
                return;
            }

            if (branchesWrapper.GameBranches.FirstOrDefault(b => b.Game.Id == launchScheme.GameId) is not { } gameBranch)
            {
                return;
            }

            if (gameBranch.PreDownload is null)
            {
                infoBarService.Warning("PreDownload not available");
                return;
            }

            if (!launchOptions.TryGetGameFileSystem(out IGameFileSystem? gameFileSystem))
            {
                return;
            }

            using (gameFileSystem)
            {
                if (!gameFileSystem.TryGetGameVersion(out string? localVersion))
                {
                    return;
                }

                (bool isOk, string? extractDirectory) = fileSystemPickerInteraction.PickFolder("Select directory to extract the game blks");
                if (!isOk)
                {
                    return;
                }

                string message = $"""
                    Local: {localVersion}
                    Remote: {gameBranch.PreDownload.Tag}
                    Extract Directory: {extractDirectory}

                    Please ensure local game is integrated.
                    We need some of old blocks to patch up.
                    """;

                ContentDialogResult result = await contentDialogFactory.CreateForConfirmCancelAsync(
                        "Extract Game Blocks",
                        message)
                    .ConfigureAwait(false);

                if (result is not ContentDialogResult.Primary)
                {
                    return;
                }

                BranchWrapper localBranch = gameBranch.Main.GetTaggedCopy(localVersion);
                BranchWrapper remoteBranch = gameBranch.PreDownload;

                ContentDialog dialog = await contentDialogFactory
                    .CreateForIndeterminateProgressAsync(SH.UIXamlViewSpecializedSophonProgressDefault)
                    .ConfigureAwait(false);

                SophonDecodedBuild? localBuild;
                SophonDecodedBuild? remoteBuild;
                using (await contentDialogFactory.BlockAsync(dialog).ConfigureAwait(false))
                {
                    localBuild = await gamePackageService.DecodeManifestsAsync(gameFileSystem, localBranch).ConfigureAwait(false);
                    remoteBuild = await gamePackageService.DecodeManifestsAsync(gameFileSystem, remoteBranch).ConfigureAwait(false);
                    if (localBuild is null || remoteBuild is null)
                    {
                        infoBarService.Error(SH.ServiceGamePackageAdvancedDecodeManifestFailed);
                        return;
                    }
                }

                GamePackageOperationContext context = new(
                    serviceProvider,
                    GamePackageOperationKind.ExtractBlocks,
                    gameFileSystem,
                    ExtractGameAssetBundles(localBuild),
                    ExtractGameAssetBundles(remoteBuild),
                    default,
                    extractDirectory);

                await gamePackageService.ExecuteOperationAsync(context).ConfigureAwait(false);
            }

            SophonDecodedBuild ExtractGameAssetBundles(SophonDecodedBuild decodedBuild)
            {
                SophonDecodedManifest manifest = decodedBuild.Manifests.First();
                SophonManifestProto proto = new();
                proto.Assets.AddRange(manifest.Data.Assets.Where(asset => AssetBundlesBlockRegex.IsMatch(asset.AssetName)));
                return new(decodedBuild.Tag, decodedBuild.DownloadTotalBytes, decodedBuild.UncompressedTotalBytes, [new(manifest.UrlPrefix, proto)]);
            }
        }
    }

    [Command("ExtractGameExeCommand")]
    private async Task ExtractGameExeAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Test extract game executable", "TestViewModel.Command"));

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IGamePackageService gamePackageService = serviceProvider.GetRequiredService<IGamePackageService>();
            HoyoPlayClient hoyoPlayClient = serviceProvider.GetRequiredService<HoyoPlayClient>();

            // TODO: Refactor
            LaunchScheme launchScheme = KnownLaunchSchemes.Values.First(s => s.IsOversea == ExtractExeOptions.IsOversea);

            Response<GameBranchesWrapper> branchResp = await hoyoPlayClient.GetBranchesAsync(launchScheme).ConfigureAwait(false);
            if (!ResponseValidator.TryValidate(branchResp, serviceProvider, out GameBranchesWrapper? branchesWrapper))
            {
                return;
            }

            if (branchesWrapper.GameBranches.FirstOrDefault(b => b.Game.Id == launchScheme.GameId) is not { } gameBranch)
            {
                return;
            }

            BranchWrapper? branch = ExtractExeOptions.IsPreDownload ? gameBranch.PreDownload : gameBranch.Main;

            if (branch is null)
            {
                infoBarService.Warning("PreDownload not available");
                return;
            }

            if (fileSystemPickerInteraction.PickFolder("Select directory to extract the game blks") is not (true, { } extractDirectory))
            {
                return;
            }

            string message = $"""
                Version: {branch.Tag}
                IsOversea: {ExtractExeOptions.IsOversea}
                Extract Directory: {extractDirectory}
                """;

            ContentDialogResult result = await contentDialogFactory.CreateForConfirmCancelAsync(
                    "Extract Game Blocks",
                    message)
                .ConfigureAwait(false);

            if (result is ContentDialogResult.Primary)
            {
                IGameFileSystem gameFileSystem = GameFileSystem.CreateForPackageOperation(Path.Combine(extractDirectory, ExtractExeOptions.IsOversea ? GameConstants.GenshinImpactFileName : GameConstants.YuanShenFileName));

                ContentDialog dialog = await contentDialogFactory
                    .CreateForIndeterminateProgressAsync(SH.UIXamlViewSpecializedSophonProgressDefault)
                    .ConfigureAwait(false);

                SophonDecodedBuild? build;
                using (await contentDialogFactory.BlockAsync(dialog).ConfigureAwait(false))
                {
                    build = await gamePackageService.DecodeManifestsAsync(gameFileSystem, branch).ConfigureAwait(false);
                    if (build is null)
                    {
                        infoBarService.Error(SH.ServiceGamePackageAdvancedDecodeManifestFailed);
                        return;
                    }
                }

                GamePackageOperationContext context = new(
                    serviceProvider,
                    GamePackageOperationKind.ExtractExecutable,
                    gameFileSystem,
                    default!,
                    ExtractGameExecutable(build),
                    default,
                    default);

                await gamePackageService.ExecuteOperationAsync(context).ConfigureAwait(false);
            }

            SophonDecodedBuild ExtractGameExecutable(SophonDecodedBuild decodedBuild)
            {
                SophonDecodedManifest manifest = decodedBuild.Manifests.First();
                SophonManifestProto proto = new();
                proto.Assets.Add(manifest.Data.Assets.Single(a => GameExecutableFileRegex.IsMatch(a.AssetName)));
                return new(decodedBuild.Tag, proto.Assets.Sum(a => a.AssetChunks.Sum(c => c.ChunkSize)), proto.Assets.Sum(a => a.AssetSize), [new(manifest.UrlPrefix, proto)]);
            }
        }
    }

    [Command("GenerateRedeemCodeCommand")]
    private async Task GenerateRedeemCodeAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Generate redeem code", "TestViewModel.Command"));

        RedeemCodeType type = RedeemCodeType.None;
        if (RedeemCodeGenerateOption.IsTimeLimited)
        {
            type |= RedeemCodeType.TimeLimited;
        }

        if (RedeemCodeGenerateOption.IsTimesLimited)
        {
            type |= RedeemCodeType.TimesLimited;
        }

        if (type is RedeemCodeType.None)
        {
            infoBarService.Warning("Please select at least one type");
            return;
        }

        if (RedeemCodeGenerateOption.ServiceType is RedeemCodeTargetServiceType.None)
        {
            infoBarService.Warning("Please select a service type");
            return;
        }

        RedeemGenerateRequest request = new()
        {
            Count = RedeemCodeGenerateOption.Count,
            Type = type,
            ServiceType = RedeemCodeGenerateOption.ServiceType,
            Value = RedeemCodeGenerateOption.Value,
            Description = RedeemCodeGenerateOption.Description,
            ExpireTime = DateTimeOffset.UtcNow.AddHours(RedeemCodeGenerateOption.ExpireHours),
            Times = RedeemCodeGenerateOption.Times,
        };

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HutaoAsAServiceClient hutaoAsAServiceClient = scope.ServiceProvider.GetRequiredService<HutaoAsAServiceClient>();
            HutaoResponse<RedeemGenerateResult> response = await hutaoAsAServiceClient.GenerateRedeemCodesAsync(request).ConfigureAwait(false);
            if (ResponseValidator.TryValidate(response, scope.ServiceProvider, out RedeemGenerateResult? generateResponse))
            {
                string message = $"""
                    {response.Message}
                    {string.Join(Environment.NewLine, generateResponse.Codes)}
                    Copied to clipboard
                    """;
                await clipboardProvider.SetTextAsync(string.Join(", ", generateResponse.Codes)).ConfigureAwait(false);
                infoBarService.Success(message, 0);
                await taskContext.SwitchToMainThreadAsync();
                RedeemCodeGenerateOption = new();
            }
        }
    }

    internal sealed class ExtractOptions
    {
        public bool IsOversea { get; set; }

        public bool IsPreDownload { get; set; }
    }

    internal sealed class DesignationOptions
    {
        public string UserName { get; set; } = default!;

        public int Days { get; set; }
    }

    internal sealed partial class RedeemCodeGenerateOptions : ObservableObject
    {
        public uint Count { get; set; }

        [ObservableProperty]
        public partial bool IsTimeLimited { get; set; }

        [ObservableProperty]
        public partial bool IsTimesLimited { get; set; }

        public RedeemCodeTargetServiceType ServiceType { get; set; }

        public int Value { get; set; }

        public string Description { get; set; } = default!;

        public int ExpireHours { get; set; }

        public uint Times { get; set; }
    }
}