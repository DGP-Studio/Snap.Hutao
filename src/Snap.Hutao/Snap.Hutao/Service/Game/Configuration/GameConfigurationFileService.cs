﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using System.IO;

namespace Snap.Hutao.Service.Game.Configuration;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGameConfigurationFileService))]
internal sealed partial class GameConfigurationFileService : IGameConfigurationFileService
{
    private const string ConfigurationFileName = "config.ini";
    private readonly RuntimeOptions runtimeOptions;

    public void Backup(string source)
    {
        if (File.Exists(source))
        {
            string serverCacheFolder = runtimeOptions.GetDataFolderServerCacheFolder();
            File.Copy(source, Path.Combine(serverCacheFolder, ConfigurationFileName), true);
        }
    }

    public void Restore(string destination)
    {
        string serverCacheFolder = runtimeOptions.GetDataFolderServerCacheFolder();
        string source = Path.Combine(serverCacheFolder, ConfigurationFileName);

        if (!File.Exists(source))
        {
            return;
        }

        // If target directory does not exist, do not copy the file
        // This often means user has moved the game folder away.
        string? directory = Path.GetDirectoryName(destination);
        if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
        {
            return;
        }

        File.Copy(source, destination, true);
    }
}