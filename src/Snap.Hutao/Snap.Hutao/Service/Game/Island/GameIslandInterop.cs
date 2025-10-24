// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service.Feature;
using Snap.Hutao.Service.Game.FileSystem;
using Snap.Hutao.Service.Game.Launching.Context;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Hutao.Response;
using Snap.Hutao.Web.Response;
using System.IO;
using System.IO.MemoryMappedFiles;
using Windows.Devices.Input;

namespace Snap.Hutao.Service.Game.Island;

internal sealed class GameIslandInterop : IGameIslandInterop
{
    private const string IslandEnvironmentName = "4F3E8543-40F7-4808-82DC-21E48A6037A7";

    private static readonly string DataFolderIslandPath = HutaoRuntime.GetDataDirectoryFile("Snap.Hutao.UnlockerIsland.dll");
    private readonly bool resume;

    private IslandFunctionOffsets offsets;
    private int accumulatedBadStateCount;
    private uint previousUid;

    public GameIslandInterop(bool resume)
    {
        this.resume = resume;
    }

    public async ValueTask BeforeAsync(BeforeLaunchExecutionContext context, CancellationToken token = default)
    {
        if (!context.FileSystem.TryGetGameVersion(out string? gameVersion))
        {
            throw HutaoException.NotSupported(SH.ServiceGameIslandFileSystemGetGameVersionFailed);
        }

        IFeatureService featureService = context.ServiceProvider.GetRequiredService<IFeatureService>();
        if (await featureService.GetIslandFeatureAsync(gameVersion).ConfigureAwait(false) is not { } feature)
        {
            throw HutaoException.NotSupported(SH.ServiceGameIslandFeature404);
        }

        if (feature.Message is { } message)
        {
            throw HutaoException.NotSupported(message);
        }

        offsets = context.TargetScheme.IsOversea ? feature.Oversea : feature.Chinese;

        if (!resume && /* CopyDll */ !LocalSetting.Get(SettingKeys.PreventCopyIslandDll, false))
        {
            InstalledLocation.CopyFileFromApplicationUri("ms-appx:///Snap.Hutao.UnlockerIsland.dll", DataFolderIslandPath);
        }
    }

    public async ValueTask WaitForExitAsync(LaunchExecutionContext context, CancellationToken token = default)
    {
        MemoryMappedFile file;
        if (resume)
        {
            try
            {
                file = MemoryMappedFile.OpenExisting(IslandEnvironmentName);
            }
            catch (FileNotFoundException)
            {
                // https://github.com/DGP-Studio/Snap.Hutao/issues/2540
                // Simply return if the game is running without island injected previously
                return;
            }
        }
        else
        {
            file = MemoryMappedFile.CreateOrOpen(IslandEnvironmentName, 1024);
        }

        using (file)
        {
            using (MemoryMappedViewAccessor accessor = file.CreateViewAccessor())
            {
                nint handle = accessor.SafeMemoryMappedViewHandle.DangerousGetHandle();
                InitializeIslandEnvironment(handle, in offsets, context.LaunchOptions);
                if (!resume)
                {
                    DllInjectionUtilities.InjectUsingRemoteThread(DataFolderIslandPath, context.Process.Id);
                }

                await PeriodicUpdateIslandEnvironmentAsync(context, handle, token).ConfigureAwait(false);
            }
        }
    }

    private static unsafe void InitializeIslandEnvironment(nint handle, ref readonly IslandFunctionOffsets offsets, LaunchOptions options)
    {
        IslandEnvironment* pIslandEnvironment = (IslandEnvironment*)handle;

        pIslandEnvironment->FunctionOffsets = offsets;

        if (LocalSetting.Get(SettingKeys.LaunchForceUsingTouchScreenWhenIntegratedTouchPresent, false))
        {
            pIslandEnvironment->UsingTouchScreen = IsIntegratedTouchPresent();
        }
        else
        {
            pIslandEnvironment->UsingTouchScreen = options.UsingTouchScreen.Value;
        }

        UpdateIslandEnvironment(handle, options);
    }

    private static unsafe IslandEnvironmentView UpdateIslandEnvironment(nint handle, LaunchOptions options)
    {
        IslandEnvironment* pIslandEnvironment = (IslandEnvironment*)handle;

        pIslandEnvironment->EnableSetFieldOfView = options.IsSetFieldOfViewEnabled.Value;
        pIslandEnvironment->FieldOfView = options.TargetFov.Value;
        pIslandEnvironment->FixLowFovScene = options.FixLowFovScene.Value;
        pIslandEnvironment->DisableFog = options.DisableFog.Value;
        pIslandEnvironment->EnableSetTargetFrameRate = options.IsSetTargetFrameRateEnabled.Value;
        pIslandEnvironment->TargetFrameRate = options.TargetFps.Value;
        pIslandEnvironment->RemoveOpenTeamProgress = options.RemoveOpenTeamProgress.Value;
        pIslandEnvironment->HideQuestBanner = options.HideQuestBanner.Value;
        pIslandEnvironment->DisableEventCameraMove = options.DisableEventCameraMove.Value;
        pIslandEnvironment->DisableShowDamageText = options.DisableShowDamageText.Value;
        pIslandEnvironment->RedirectCombineEntry = options.RedirectCombineEntry.Value;
        pIslandEnvironment->ResinListItemId000106Allowed = options.ResinListItemId000106Allowed.Value;
        pIslandEnvironment->ResinListItemId000201Allowed = options.ResinListItemId000201Allowed.Value;
        pIslandEnvironment->ResinListItemId107009Allowed = options.ResinListItemId107009Allowed.Value;
        pIslandEnvironment->ResinListItemId107012Allowed = options.ResinListItemId107012Allowed.Value;
        pIslandEnvironment->ResinListItemId220007Allowed = options.ResinListItemId220007Allowed.Value;

        return pIslandEnvironment->View;
    }

    private static bool IsIntegratedTouchPresent()
    {
        IReadOnlyList<PointerDevice> devices = PointerDevice.GetPointerDevices();

        // ReSharper disable once ForCanBeConvertedToForeach
        // https://github.com/microsoft/CsWinRT/issues/747
        for (int i = 0; i < devices.Count; i++)
        {
            PointerDevice device = devices[i];
            if (device is { PointerDeviceType: PointerDeviceType.Touch, IsIntegrated: true })
            {
                return true;
            }
        }

        return false;
    }

    private static async ValueTask HandleUidChangedAsync(LaunchExecutionContext context, uint uid, CancellationToken token)
    {
        using (IServiceScope scope = context.ServiceProvider.CreateScope())
        {
            HutaoResponse response = await scope.ServiceProvider
                .GetRequiredService<HutaoInfrastructureClient>()
                .AmIBannedAsync($"{uid}", token)
                .ConfigureAwait(false);

            if (!ResponseValidator.TryValidate(response, context.ServiceProvider))
            {
                context.Process.Kill();
            }
        }
    }

    private async ValueTask PeriodicUpdateIslandEnvironmentAsync(LaunchExecutionContext context, nint handle, CancellationToken token)
    {
        using (PeriodicTimer timer = new(TimeSpan.FromMilliseconds(500)))
        {
            while (await timer.WaitForNextTickAsync(token).ConfigureAwait(false))
            {
                if (!context.Process.IsRunning())
                {
                    break;
                }

                IslandEnvironmentView view = UpdateIslandEnvironment(handle, context.LaunchOptions);
                if (Interlocked.Exchange(ref previousUid, view.Uid) != view.Uid)
                {
                    await HandleUidChangedAsync(context, view.Uid, token).ConfigureAwait(false);
                }

                if (view.State is IslandState.None or IslandState.Stopped)
                {
                    if (Interlocked.Increment(ref accumulatedBadStateCount) >= 10)
                    {
                        HutaoException.Throw($"UnlockerIsland in bad state for too long, last state: {view.State}");
                    }
                }
                else
                {
                    unsafe
                    {
                        if (view.State is IslandState.Started && view.Size != sizeof(IslandEnvironment))
                        {
                            HutaoException.Throw("IslandEnvironment size mismatch");
                        }
                    }

                    Interlocked.Exchange(ref accumulatedBadStateCount, 0);
                }
            }
        }
    }
}