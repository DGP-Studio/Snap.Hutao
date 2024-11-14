// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.Caching;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.Graphics;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.Hashing;
using Snap.Hutao.Core.IO.Http.Sharding;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Picker;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Automation.ScreenCapture;
using Snap.Hutao.Service.Game.Package.Advanced;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml;
using Snap.Hutao.UI.Xaml.View.Window;
using Snap.Hutao.ViewModel.Game;
using Snap.Hutao.ViewModel.Guide;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;
using Snap.Hutao.Web.Hutao.HutaoAsAService;
using Snap.Hutao.Web.Response;
using Snap.Hutao.Win32.Foundation;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;

namespace Snap.Hutao.ViewModel;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class TestViewModel : Abstraction.ViewModel
{
    private readonly IFileSystemPickerInteraction fileSystemPickerInteraction;
    private readonly ICurrentXamlWindowReference currentXamlWindowReference;
    private readonly IGameScreenCaptureService gameScreenCaptureService;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly IInfoBarService infoBarService;
    private readonly ILogger<TestViewModel> logger;
    private readonly IMemoryCache memoryCache;
    private readonly ITaskContext taskContext;

    public UploadAnnouncement Announcement { get; set => SetProperty(ref field, value); } = new();

    public ExtractOptions ExtractExeOptions { get; } = new();

    public bool SuppressMetadataInitialization
    {
        get => LocalSetting.Get(SettingKeys.SuppressMetadataInitialization, false);
        set
        {
            if (IsViewDisposed)
            {
                return;
            }

            LocalSetting.Set(SettingKeys.SuppressMetadataInitialization, value);
        }
    }

    public bool OverrideElevationRequirement
    {
        get => LocalSetting.Get(SettingKeys.OverrideElevationRequirement, false);
        set
        {
            if (IsViewDisposed)
            {
                return;
            }

            LocalSetting.Set(SettingKeys.OverrideElevationRequirement, value);
        }
    }

    public bool OverrideUpdateVersionComparison
    {
        get => LocalSetting.Get(SettingKeys.OverrideUpdateVersionComparison, false);
        set
        {
            if (IsViewDisposed)
            {
                return;
            }

            LocalSetting.Set(SettingKeys.OverrideUpdateVersionComparison, value);
        }
    }

    public bool OverridePackageConvertDirectoryPermissionsRequirement
    {
        get => LocalSetting.Get(SettingKeys.OverridePackageConvertDirectoryPermissionsRequirement, false);
        set
        {
            if (IsViewDisposed)
            {
                return;
            }

            LocalSetting.Set(SettingKeys.OverridePackageConvertDirectoryPermissionsRequirement, value);
        }
    }

    public bool OverrideHardDriveType
    {
        get => LocalSetting.Get(SettingKeys.OverridePhysicalDriverType, false);
        set
        {
            if (IsViewDisposed)
            {
                return;
            }

            LocalSetting.Set(SettingKeys.OverridePhysicalDriverType, value);
            OnPropertyChanged();
        }
    }

    public bool OverrideHardDriveTypeIsSolidState
    {
        get => LocalSetting.Get(SettingKeys.PhysicalDriverIsAlwaysSolidState, false);
        set
        {
            if (IsViewDisposed)
            {
                return;
            }

            LocalSetting.Set(SettingKeys.PhysicalDriverIsAlwaysSolidState, value);
        }
    }

    public bool AlwaysIsFirstRunAfterUpdate
    {
        get => LocalSetting.Get(SettingKeys.AlwaysIsFirstRunAfterUpdate, false);
        set
        {
            if (IsViewDisposed)
            {
                return;
            }

            LocalSetting.Set(SettingKeys.AlwaysIsFirstRunAfterUpdate, value);
        }
    }

    public bool AlphaBuildUseCNPatchEndpoint
    {
        get => LocalSetting.Get(SettingKeys.AlphaBuildUseCNPatchEndpoint, false);
        set
        {
            if (IsViewDisposed)
            {
                return;
            }

            LocalSetting.Set(SettingKeys.AlphaBuildUseCNPatchEndpoint, value);
        }
    }

    [Command("ResetGuideStateCommand")]
    private static void ResetGuideState()
    {
        UnsafeLocalSetting.Set(SettingKeys.Major1Minor10Revision0GuideState, GuideState.Language);
    }

    [Command("ExceptionCommand")]
    private static void ThrowTestException()
    {
        HutaoException.Throw("Test Exception");
    }

    [Command("FileOperationRenameCommand")]
    private static void FileOperationRename()
    {
        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string source = Path.Combine(desktop, "TestFolder");
        DirectoryOperation.UnsafeRename(source, "TestFolder1");
    }

    [Command("ResetMainWindowSizeCommand")]
    private void ResetMainWindowSize()
    {
        if (currentXamlWindowReference.Window is MainWindow mainWindow)
        {
            double scale = mainWindow.GetRasterizationScale();
            mainWindow.AppWindow.Resize(new Windows.Graphics.SizeInt32(1372, 772).Scale(scale));
        }
    }

    [Command("UploadAnnouncementCommand")]
    private async Task UploadAnnouncementAsync()
    {
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
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HutaoAsAServiceClient hutaoAsAServiceClient = scope.ServiceProvider.GetRequiredService<HutaoAsAServiceClient>();
            Response response = await hutaoAsAServiceClient.GachaLogCompensationAsync(15).ConfigureAwait(false);
            if (ResponseValidator.TryValidate(response, scope.ServiceProvider))
            {
                infoBarService.Success(response.Message);
                await taskContext.SwitchToMainThreadAsync();
                Announcement = new();
            }
        }
    }

    [Obsolete]
    [Command("DebugPrintImageCacheFailedDownloadTasksCommand")]
    private void DebugPrintImageCacheFailedDownloadTasks()
    {
        if (memoryCache.TryGetValue($"{nameof(ImageCache)}.FailedDownloadTasks", out HashSet<string>? set))
        {
            logger.LogInformation("Failed ImageCache download tasks: [{Tasks}]", set?.ToString(','));
        }
    }

    [Command("ScreenCaptureCommand")]
    private async Task ScreenCaptureAsync()
    {
        HWND hwnd = currentXamlWindowReference.GetWindowHandle();
        if (gameScreenCaptureService.TryStartCapture(hwnd, true, out GameScreenCaptureSession? session))
        {
            using (session)
            {
                while (true)
                {
                    await session.RequestFrameAsync().ConfigureAwait(false);
                    await Task.Delay(1000).ConfigureAwait(false);
                }
            }
        }
    }

    [Command("SendRandomInfoBarNotificationCommand")]
    private void SendRandomInfoBarNotification()
    {
        infoBarService.PrepareInfoBarAndShow(builder => builder
            .SetSeverity((InfoBarSeverity)Random.Shared.Next((int)InfoBarSeverity.Error) + 1)
            .SetTitle("Lorem ipsum dolor sit amet")
            .SetMessage("Consectetur adipiscing elit. Nullam nec purus nec elit ultricies tincidunt. Donec nec sapien nec elit ultricies tincidunt. Donec nec sapien nec elit ultricies tincidunt."));
    }

    [Command("CheckPathBelongsToSSDCommand")]
    private void CheckPathBelongsToSSD()
    {
        (bool isOk, ValueFile file) = fileSystemPickerInteraction.PickFile("Pick any file!", default);
        if (isOk)
        {
            bool isSolidState = PhysicalDriver.DangerousGetIsSolidState(file);
            infoBarService.Success($"The path '{file}' belongs to a {(isSolidState ? "solid state" : "hard disk")} drive.");
        }
    }

    [Command("TestHttpShardDownload")]
    private async Task TestHttpShardDownloadAsync()
    {
        using (HttpClient httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient())
        {
            HttpShardCopyWorkerOptions<Void> options = new()
            {
                HttpClient = httpClient,
                SourceUrl = "https://ghproxy.qhy04.cc/https://github.com/DGP-Studio/Snap.Hutao/releases/download/1.11.0/Snap.Hutao.1.11.0.msix",
                DestinationFilePath = "D://test.file",
                StatusFactory = (bytesRead, totalBytes) => default,
            };

            Progress<Void> progress = new();

            using (IHttpShardCopyWorker<Void> worker = await HttpShardCopyWorker.CreateAsync(options).ConfigureAwait(false))
            {
                await worker.CopyAsync(progress).ConfigureAwait(false);
            }
        }

        string result = await Hash.FileToHexStringAsync(HashAlgorithmName.SHA256, "D://test.file").ConfigureAwait(false);
        logger.LogInformation("File SHA256: {SHA256}", result);
    }

    [Command("RunCodeCommand")]
    private async Task RunCodeAsync()
    {
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

    [Command("TestGamePackageOperationWindowCommand")]
    private void TestGamePackageOperationWindow()
    {
        serviceProvider.GetRequiredService<GamePackageOperationWindow>().DataContext.TestProgress();
    }

    [Command("ExtractGameBlocksCommand")]
    private async Task ExtractGameBlocksAsync()
    {
        IGamePackageService gamePackageService = serviceProvider.GetRequiredService<IGamePackageService>();
        LaunchGameShared launchGameShared = serviceProvider.GetRequiredService<LaunchGameShared>();
        HoyoPlayClient hoyoPlayClient = serviceProvider.GetRequiredService<HoyoPlayClient>();
        LaunchOptions launchOptions = serviceProvider.GetRequiredService<LaunchOptions>();

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
            infoBarService.Warning("Predownload not available");
            return;
        }

        if (!launchOptions.TryGetGameFileSystem(out GameFileSystem? gameFileSystem))
        {
            return;
        }

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

        GamePackageOperationContext context = new(
            serviceProvider,
            GamePackageOperationKind.ExtractBlk,
            gameFileSystem,
            gameBranch.Main.GetTaggedCopy(localVersion),
            gameBranch.PreDownload,
            default,
            extractDirectory);

        await gamePackageService.StartOperationAsync(context).ConfigureAwait(false);
    }

    [Command("ExtractGameExeCommand")]
    private async Task ExtractGameExeAsync()
    {
        IGamePackageService gamePackageService = serviceProvider.GetRequiredService<IGamePackageService>();
        HoyoPlayClient hoyoPlayClient = serviceProvider.GetRequiredService<HoyoPlayClient>();

        LaunchScheme launchScheme = KnownLaunchSchemes.Get().First(s => s.IsOversea == ExtractExeOptions.IsOversea);

        Response<GameBranchesWrapper> branchResp = await hoyoPlayClient.GetBranchesAsync(launchScheme).ConfigureAwait(false);
        if (!ResponseValidator.TryValidate(branchResp, serviceProvider, out GameBranchesWrapper? branchesWrapper))
        {
            return;
        }

        if (branchesWrapper.GameBranches.FirstOrDefault(b => b.Game.Id == launchScheme.GameId) is not { } gameBranch)
        {
            return;
        }

        BranchWrapper branch = ExtractExeOptions.IsPredownload ? gameBranch.PreDownload : gameBranch.Main;

        if (branch is null)
        {
            infoBarService.Warning("Predownload not available");
            return;
        }

        (bool isOk, string? extractDirectory) = fileSystemPickerInteraction.PickFolder("Select directory to extract the game blks");
        if (!isOk)
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
            GameFileSystem gameFileSystem = new(Path.Combine(extractDirectory, ExtractExeOptions.IsOversea ? GameConstants.GenshinImpactFileName : GameConstants.YuanShenFileName));

            GamePackageOperationContext context = new(
                serviceProvider,
                GamePackageOperationKind.ExtractExe,
                gameFileSystem,
                default!,
                branch,
                default,
                default);

            await gamePackageService.StartOperationAsync(context).ConfigureAwait(false);
        }
    }

    internal sealed class ExtractOptions
    {
        public bool IsOversea { get; set; }

        public bool IsPredownload { get; set; }
    }
}