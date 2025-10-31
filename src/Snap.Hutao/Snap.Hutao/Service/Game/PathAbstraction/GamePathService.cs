// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.FileSystem;
using Snap.Hutao.Service.Game.Locator;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Game.PathAbstraction;

[Service(ServiceLifetime.Singleton, typeof(IGamePathService))]
internal sealed partial class GamePathService : IGamePathService
{
    private readonly IGameLocatorFactory gameLocatorFactory;
    private readonly LaunchOptions launchOptions;
    private readonly ITaskContext taskContext;

    [GeneratedConstructor]
    public partial GamePathService(IServiceProvider serviceProvider);

    public async ValueTask<ValueResult<bool, string>> SilentLocateGamePathAsync()
    {
        // Found in setting
        string? gamePath = launchOptions.GamePathEntry.Value?.Path;
        if (!string.IsNullOrEmpty(gamePath))
        {
            return new(true, gamePath);
        }

        // Try to locate by unity log
        if (await gameLocatorFactory.LocateSingleAsync(GameLocationSourceKind.UnityLog).ConfigureAwait(false) is (true, { } path))
        {
            await taskContext.SwitchToMainThreadAsync();
            return new(true, launchOptions.PerformGamePathEntrySynchronization(path));
        }

        return new(false, SH.ServiceGamePathLocateFailed);
    }

    public async ValueTask SilentLocateAllGamePathAsync()
    {
        HashSet<string> paths = [];
        foreach (string path in await gameLocatorFactory.LocateMultipleAsync(GameLocationSourceKind.UnityLog).ConfigureAwait(false))
        {
            paths.Add(path);
        }

        const string LockTrace = $"{nameof(GamePathService)}.{nameof(SilentLocateAllGamePathAsync)}";
        using (await launchOptions.GamePathLock.WriterLockAsync(LockTrace).ConfigureAwait(false))
        {
            foreach (GamePathEntry entry in launchOptions.GamePathEntries.Value)
            {
                paths.Remove(entry.Path);
            }

            ImmutableArray<GamePathEntry>.Builder builder = launchOptions.GamePathEntries.Value.ToBuilder();
            builder.AddRange(paths.Select(GamePathEntry.Create));

            // Since all path we add are not in original list, we can skip calling PerformGamePathEntrySynchronization
            await taskContext.SwitchToMainThreadAsync();
            launchOptions.GamePathEntries.Value = builder.ToImmutable();
        }
    }
}