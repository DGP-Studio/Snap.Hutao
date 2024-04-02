// Copyright (c) DGP Studio. All rights reserved.
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
        if (File.Exists(source))
        {
            File.Copy(source, destination, true);
        }
    }
}