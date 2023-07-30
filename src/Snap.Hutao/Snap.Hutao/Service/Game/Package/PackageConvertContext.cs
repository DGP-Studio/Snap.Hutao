// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using static Snap.Hutao.Service.Game.GameConstants;

namespace Snap.Hutao.Service.Game.Package;

internal readonly struct PackageConvertContext
{
    public readonly string GameFolder;
    public readonly string ServerCacheFolder;

    public readonly string ServerCacheBackupFolder; // From
    public readonly string ServerCacheTargetFolder; // To

    public readonly string FromDataFolderName;
    public readonly string ToDataFolderName;
    public readonly string FromDataFolder;
    public readonly string ToDataFolder;

    public readonly string ScatteredFilesUrl;
    public readonly string PkgVersionUrl;

    public PackageConvertContext(bool isTargetOversea, string dataFolder, string gameFolder, string scatteredFilesUrl)
    {
        GameFolder = gameFolder;
        ServerCacheFolder = Path.Combine(dataFolder, "ServerCache");

        string serverCacheOversea = Path.Combine(ServerCacheFolder, "Oversea");
        string serverCacheChinese = Path.Combine(ServerCacheFolder, "Chinese");
        (ServerCacheBackupFolder, ServerCacheTargetFolder) = isTargetOversea
            ? (serverCacheChinese, serverCacheOversea)
            : (serverCacheOversea, serverCacheChinese);

        (FromDataFolderName, ToDataFolderName) = isTargetOversea
            ? (YuanShenData, GenshinImpactData)
            : (GenshinImpactData, YuanShenData);

        (FromDataFolder, ToDataFolder) = (Path.Combine(GameFolder, FromDataFolderName), Path.Combine(GameFolder, ToDataFolderName));

        ScatteredFilesUrl = scatteredFilesUrl;
        PkgVersionUrl = $"{scatteredFilesUrl}/pkg_version";
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
        return Path.Combine(GameFolder, filePath);
    }
}