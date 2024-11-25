// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.ChannelSDK;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.DeprecatedFile;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Package;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using static Snap.Hutao.Service.Game.GameConstants;

namespace Snap.Hutao.Service.Game.Package;

internal readonly struct PackageConverterContext
{
    public readonly CommonReferences Common;
    public readonly ParallelOptions ParallelOptions;

    public readonly ScatteredFilesOnlyReferences ScatterFilesOnly;
    public readonly SophonChunksOnlyReferences SophonChunksOnly;

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

    public PackageConverterContext(CommonReferences common, BranchWrapper currentBranch, BranchWrapper targetBranch)
        : this(common)
    {
        Common = common;
        SophonChunksOnly = new(currentBranch, targetBranch);
    }

    public PackageConverterContext(CommonReferences common, GamePackage gamePackage)
        : this(common)
    {
        Common = common;
        ScatterFilesOnly = new(gamePackage);

        ScatteredFilesUrl = gamePackage.Main.Major.ResourceListUrl;
        PkgVersionUrl = $"{ScatteredFilesUrl}/pkg_version";
    }

    private PackageConverterContext(CommonReferences common)
    {
        ParallelOptions = new() { MaxDegreeOfParallelism = Environment.ProcessorCount, };

        ServerCacheFolder = HutaoRuntime.GetDataFolderServerCacheFolder();
        ServerCacheChunksFolder = Path.Combine(ServerCacheFolder, "Chunks");

        string serverCacheOversea = Path.Combine(ServerCacheFolder, "Oversea");
        string serverCacheChinese = Path.Combine(ServerCacheFolder, "Chinese");
        (ServerCacheBackupFolder, ServerCacheTargetFolder) = common.TargetScheme.IsOversea
            ? (serverCacheChinese, serverCacheOversea)
            : (serverCacheOversea, serverCacheChinese);

        (FromDataFolderName, ToDataFolderName) = common.TargetScheme.IsOversea
            ? (YuanShenData, GenshinImpactData)
            : (GenshinImpactData, YuanShenData);

        FromDataFolder = Path.Combine(common.GameFileSystem.GameDirectory, FromDataFolderName);
        ToDataFolder = Path.Combine(common.GameFileSystem.GameDirectory, ToDataFolderName);
    }

    public HttpClient HttpClient { get => Common.HttpClient; }

    public LaunchScheme CurrentScheme { get => Common.CurrentScheme; }

    public LaunchScheme TargetScheme { get => Common.TargetScheme; }

    public IGameFileSystem GameFileSystem { get => Common.GameFileSystem; }

    public GameChannelSDK? GameChannelSDK { get => Common.GameChannelSDK; }

    public DeprecatedFilesWrapper? DeprecatedFiles { get => Common.DeprecatedFiles; }

    public IProgress<PackageConvertStatus> Progress { get => Common.Progress; }

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
        return Path.Combine(Common.GameFileSystem.GameDirectory, filePath);
    }

    [SuppressMessage("", "SH003")]
    public readonly Task<AsyncKeyedLock<string>.Releaser> ExclusiveProcessChunkAsync(string chunkName, CancellationToken token = default)
    {
        return chunkLocks.LockAsync(chunkName);
    }

    internal readonly struct CommonReferences
    {
        public readonly HttpClient HttpClient;
        public readonly LaunchScheme CurrentScheme;
        public readonly LaunchScheme TargetScheme;
        public readonly IGameFileSystem GameFileSystem;
        public readonly GameChannelSDK? GameChannelSDK;
        public readonly DeprecatedFilesWrapper? DeprecatedFiles;
        public readonly IProgress<PackageConvertStatus> Progress;

        public CommonReferences(
            HttpClient httpClient,
            LaunchScheme currentScheme,
            LaunchScheme targetScheme,
            IGameFileSystem gameFileSystem,
            GameChannelSDK? gameChannelSDK,
            DeprecatedFilesWrapper? deprecatedFiles,
            IProgress<PackageConvertStatus> progress)
        {
            HttpClient = httpClient;
            CurrentScheme = currentScheme;
            TargetScheme = targetScheme;
            GameFileSystem = gameFileSystem;
            GameChannelSDK = gameChannelSDK;
            DeprecatedFiles = deprecatedFiles;
            Progress = progress;
        }
    }

    internal readonly struct ScatteredFilesOnlyReferences
    {
        public readonly GamePackage? TargetPackage;

        public ScatteredFilesOnlyReferences(GamePackage? targetPackage)
        {
            TargetPackage = targetPackage;
        }
    }

    internal readonly struct SophonChunksOnlyReferences
    {
        public readonly BranchWrapper? CurrentBranch;
        public readonly BranchWrapper? TargetBranch;

        public SophonChunksOnlyReferences(BranchWrapper currentBranch, BranchWrapper targetBranch)
        {
            CurrentBranch = currentBranch;
            TargetBranch = targetBranch;
        }
    }
}