// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Service.Feature;
using Snap.Hutao.Service.Game.Launching;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace Snap.Hutao.Service.Game.Island;

internal sealed class GameIslandInterop : IGameIslandInterop
{
    private const string IslandEnvironmentName = "4F3E8543-40F7-4808-82DC-21E48A6037A7";

    private readonly LaunchExecutionContext context;
    private readonly bool resume;
    private readonly string dataFolderIslandPath;

    private IslandFunctionOffsets offsets;
    private int accumulatedBadStateCount;

    public GameIslandInterop(LaunchExecutionContext context, bool resume)
    {
        this.context = context;
        this.resume = resume;
        dataFolderIslandPath = Path.Combine(HutaoRuntime.DataFolder, "Snap.Hutao.UnlockerIsland.dll");
    }

    public async ValueTask<bool> PrepareAsync(CancellationToken token = default)
    {
        if (!context.TryGetGameFileSystem(out IGameFileSystemView? gameFileSystem))
        {
            return false;
        }

        if (!gameFileSystem.TryGetGameVersion(out string? gameVersion))
        {
            return false;
        }

        IFeatureService featureService = context.ServiceProvider.GetRequiredService<IFeatureService>();
        if (await featureService.GetGameIslandFeatureAsync(gameVersion).ConfigureAwait(false) is not { } feature)
        {
            return false;
        }

        offsets = context.TargetScheme.IsOversea ? feature.Oversea : feature.Chinese;

        if (!resume && !GlobalSwitch.PreventCopyIslandDll)
        {
            InstalledLocation.CopyFileFromApplicationUri("ms-appx:///Snap.Hutao.UnlockerIsland.dll", dataFolderIslandPath);
        }

        return true;
    }

    public async ValueTask WaitForExitAsync(CancellationToken token = default)
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
                // We do not inject the island to process that not started by us.
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
                InitializeIslandEnvironment(handle, in offsets, context.Options);
                if (!resume)
                {
                    DllInjectionUtilities.InjectUsingWindowsHook(dataFolderIslandPath, "DllGetWindowsHookForHutao", context.Process.Id);
                }

                using (PeriodicTimer timer = new(TimeSpan.FromMilliseconds(500)))
                {
                    while (await timer.WaitForNextTickAsync(token).ConfigureAwait(false))
                    {
                        if (!context.Process.IsRunning())
                        {
                            break;
                        }

                        IslandEnvironmentView view = UpdateIslandEnvironment(handle, context.Options);

                        if (view.State is IslandState.None or IslandState.Stopped)
                        {
                            if (Interlocked.Increment(ref accumulatedBadStateCount) >= 10)
                            {
                                HutaoException.Throw($"UnlockerIsland in bad state for too long, last state: {view.State}");
                            }
                        }
                        else
                        {
                            Interlocked.Exchange(ref accumulatedBadStateCount, 0);
                        }
                    }
                }
            }
        }
    }

    private static unsafe void InitializeIslandEnvironment(nint handle, ref readonly IslandFunctionOffsets offsets, LaunchOptions options)
    {
        IslandEnvironment* pIslandEnvironment = (IslandEnvironment*)handle;

        pIslandEnvironment->FunctionOffsets = offsets;
        pIslandEnvironment->UsingTouchScreen = options.UsingTouchScreen;

        UpdateIslandEnvironment(handle, options);
    }

    private static unsafe IslandEnvironmentView UpdateIslandEnvironment(nint handle, LaunchOptions options)
    {
        IslandEnvironment* pIslandEnvironment = (IslandEnvironment*)handle;

        pIslandEnvironment->EnableSetFieldOfView = options.IsSetFieldOfViewEnabled;
        pIslandEnvironment->FieldOfView = options.TargetFov;
        pIslandEnvironment->FixLowFovScene = options.FixLowFovScene;
        pIslandEnvironment->DisableFog = options.DisableFog;
        pIslandEnvironment->EnableSetTargetFrameRate = options.IsSetTargetFrameRateEnabled;
        pIslandEnvironment->TargetFrameRate = options.TargetFps;
        pIslandEnvironment->RemoveOpenTeamProgress = options.RemoveOpenTeamProgress;
        pIslandEnvironment->HideQuestBanner = options.HideQuestBanner;
        pIslandEnvironment->DisableEventCameraMove = options.DisableEventCameraMove;
        pIslandEnvironment->DisableShowDamageText = options.DisableShowDamageText;
        pIslandEnvironment->RedirectCombineEntry = options.RedirectCombineEntry;

        return *(IslandEnvironmentView*)pIslandEnvironment;
    }
}