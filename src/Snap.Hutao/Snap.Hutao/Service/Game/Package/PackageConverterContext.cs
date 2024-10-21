// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Concurrent;
using Snap.Hutao.Core;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.ChannelSDK;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.DeprecatedFile;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Package;
using System.IO;
using System.Net.Http;
using static Snap.Hutao.Service.Game.GameConstants;

namespace Snap.Hutao.Service.Game.Package;

internal readonly struct PackageConverterContext
{
    public readonly HttpClient HttpClient;
    public readonly ParallelOptions ParallelOptions;
    public readonly LaunchScheme TargetScheme;
    public readonly GameFileSystem GameFileSystem;
    public readonly BranchWrapper? CurrentBranch;
    public readonly BranchWrapper? TargetBranch;
    public readonly GamePackage? TargetPackage;
    public readonly GameChannelSDK? GameChannelSDK;
    public readonly DeprecatedFilesWrapper? DeprecatedFiles;
    public readonly IProgress<PackageConvertStatus> Progress;

    public readonly string ServerCacheFolder;

    public readonly string ServerCacheChunksFolder;
    public readonly ConcurrentDictionary<string, Void> DuplicatedChunkNames = [];

    public readonly string ServerCacheBackupFolder; // From
    public readonly string ServerCacheTargetFolder; // To

    public readonly string FromDataFolderName;
    public readonly string ToDataFolderName;
    public readonly string FromDataFolder;
    public readonly string ToDataFolder;

    public readonly string? ScatteredFilesUrl;
    public readonly string? PkgVersionUrl;

    private readonly AsyncKeyedLock<string> chunkLocks = new();

    public PackageConverterContext(HttpClient httpClient, LaunchScheme targetScheme, GameFileSystem gameFileSystem, BranchWrapper currentBranch, BranchWrapper targetBranch, GameChannelSDK? gameChannelSDK, DeprecatedFilesWrapper? deprecatedFiles, IProgress<PackageConvertStatus> progress)
    {
        HttpClient = httpClient;
        TargetScheme = targetScheme;
        GameFileSystem = gameFileSystem;
        CurrentBranch = currentBranch;
        TargetBranch = targetBranch;
        GameChannelSDK = gameChannelSDK;
        DeprecatedFiles = deprecatedFiles;
        Progress = progress;

        ParallelOptions = new()
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount,
        };

        ServerCacheFolder = HutaoRuntime.GetDataFolderServerCacheFolder();
        ServerCacheChunksFolder = Path.Combine(ServerCacheFolder, "Chunks");

        string serverCacheOversea = Path.Combine(ServerCacheFolder, "Oversea");
        string serverCacheChinese = Path.Combine(ServerCacheFolder, "Chinese");
        (ServerCacheBackupFolder, ServerCacheTargetFolder) = TargetScheme.IsOversea
            ? (serverCacheChinese, serverCacheOversea)
            : (serverCacheOversea, serverCacheChinese);

        (FromDataFolderName, ToDataFolderName) = TargetScheme.IsOversea
            ? (YuanShenData, GenshinImpactData)
            : (GenshinImpactData, YuanShenData);

        FromDataFolder = Path.Combine(GameFileSystem.GameDirectory, FromDataFolderName);
        ToDataFolder = Path.Combine(GameFileSystem.GameDirectory, ToDataFolderName);
    }

    public PackageConverterContext(HttpClient httpClient, LaunchScheme targetScheme, GameFileSystem gameFileSystem, GamePackage gamePackage, GameChannelSDK? gameChannelSDK, DeprecatedFilesWrapper? deprecatedFiles, IProgress<PackageConvertStatus> progress)
    {
        HttpClient = httpClient;
        TargetScheme = targetScheme;
        GameFileSystem = gameFileSystem;
        TargetPackage = gamePackage;
        GameChannelSDK = gameChannelSDK;
        DeprecatedFiles = deprecatedFiles;
        Progress = progress;

        ParallelOptions = new()
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount,
        };

        ServerCacheFolder = HutaoRuntime.GetDataFolderServerCacheFolder();
        ServerCacheChunksFolder = Path.Combine(ServerCacheFolder, "Chunks");

        string serverCacheOversea = Path.Combine(ServerCacheFolder, "Oversea");
        string serverCacheChinese = Path.Combine(ServerCacheFolder, "Chinese");
        (ServerCacheBackupFolder, ServerCacheTargetFolder) = TargetScheme.IsOversea
            ? (serverCacheChinese, serverCacheOversea)
            : (serverCacheOversea, serverCacheChinese);

        (FromDataFolderName, ToDataFolderName) = TargetScheme.IsOversea
            ? (YuanShenData, GenshinImpactData)
            : (GenshinImpactData, YuanShenData);

        FromDataFolder = Path.Combine(GameFileSystem.GameDirectory, FromDataFolderName);
        ToDataFolder = Path.Combine(GameFileSystem.GameDirectory, ToDataFolderName);

        ScatteredFilesUrl = gamePackage.Main.Major.ResourceListUrl;
        PkgVersionUrl = $"{ScatteredFilesUrl}/pkg_version";
    }

    public readonly string GetScatteredFilesUrl(string file)
    {
        return $"{ScatteredFilesUrl}/{file}";
    }

    public readonly string GetServerCacheBackupFilePath(string filePath)
    {
        return Path.Combine(ServerCacheBackupFolder, filePath);
    }

    public readonly string GetServerCacheTargetFilePath(string filePath)
    {
        return Path.Combine(ServerCacheTargetFolder, filePath);
    }

    public readonly string GetGameFolderFilePath(string filePath)
    {
        return Path.Combine(GameFileSystem.GameDirectory, filePath);
    }

    [SuppressMessage("", "SH003")]
    public readonly Task<AsyncKeyedLock<string>.Releaser> ExclusiveProcessChunkAsync(string chunkName, CancellationToken token = default)
    {
        return chunkLocks.LockAsync(chunkName);
    }
}