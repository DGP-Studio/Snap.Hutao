// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Service.Game.FileSystem;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using static Snap.Hutao.Service.Game.GameConstants;

namespace Snap.Hutao.Service.Game.Package;

internal sealed class PackageConverterContext
{
    private readonly AsyncKeyedLock<string> chunkLocks = new();

    public PackageConverterContext(LaunchScheme currentScheme, LaunchScheme targetScheme, IGameFileSystem fileSystem)
    {
        CurrentScheme = currentScheme;
        TargetScheme = targetScheme;
        GameFileSystem = fileSystem;

        ParallelOptions = new() { MaxDegreeOfParallelism = Environment.ProcessorCount, };

        ServerCacheFolder = HutaoRuntime.GetDataServerCacheDirectory();
        ServerCacheChunksFolder = Path.Combine(ServerCacheFolder, "Chunks");

        string serverCacheOversea = Path.Combine(ServerCacheFolder, "Oversea");
        string serverCacheChinese = Path.Combine(ServerCacheFolder, "Chinese");

        (ServerCacheBackupFolder, ServerCacheTargetFolder) = targetScheme.IsOversea
            ? (serverCacheChinese, serverCacheOversea)
            : (serverCacheOversea, serverCacheChinese);

        (FromDataFolderName, ToDataFolderName) = targetScheme.IsOversea
            ? (YuanShenData, GenshinImpactData)
            : (GenshinImpactData, YuanShenData);

        FromDataFolder = Path.Combine(fileSystem.GetGameDirectory(), FromDataFolderName);
        ToDataFolder = Path.Combine(fileSystem.GetGameDirectory(), ToDataFolderName);
    }

    public ParallelOptions ParallelOptions { get; }

    public string ServerCacheFolder { get; }

    public string ServerCacheChunksFolder { get; }

    public ConcurrentDictionary<string, Void> DuplicatedChunkNames { get; } = [];

    public string ServerCacheBackupFolder { get; }

    public string ServerCacheTargetFolder { get; }

    public string FromDataFolderName { get; }

    public string ToDataFolderName { get; }

    public string FromDataFolder { get; }

    public string ToDataFolder { get; }

    public LaunchScheme CurrentScheme { get; }

    public LaunchScheme TargetScheme { get; }

    public IGameFileSystem GameFileSystem { get; }

    public required HttpClient HttpClient { get; init; }

    public required IProgress<PackageConvertStatus> Progress { get; init; }

    public BranchWrapper? CurrentBranch { get; init; }

    public BranchWrapper? TargetBranch { get; init; }

    public string GetServerCacheBackupFilePath(string filePath)
    {
        return Path.Combine(ServerCacheBackupFolder, filePath);
    }

    public string GetServerCacheTargetFilePath(string filePath)
    {
        return Path.Combine(ServerCacheTargetFolder, filePath);
    }

    public string GetGameFolderFilePath(string filePath)
    {
        return Path.Combine(GameFileSystem.GetGameDirectory(), filePath);
    }

    [SuppressMessage("", "SH003")]
    public Task<AsyncKeyedLock<string>.Releaser> ExclusiveProcessChunkAsync(string chunkName, CancellationToken token = default)
    {
        return chunkLocks.LockAsync(chunkName);
    }
}