// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO.Ini;
using Snap.Hutao.Service.Game.Configuration;
using Snap.Hutao.Service.Game.FileSystem;
using Snap.Hutao.Service.Game.Scheme;
using System.Collections.Immutable;
using System.IO;

namespace Snap.Hutao.Service.Game.Package.Advanced;

internal sealed class GameInstallPersistence
{
    private const string InstallingName = "snap_hutao_installing";
    private readonly IGameFileSystem gameFileSystem;

    private GameInstallPersistence(IGameFileSystem gameFileSystem)
    {
        this.gameFileSystem = gameFileSystem;
    }

    public static bool TryAcquire(IGameFileSystem gameFileSystem, string version, LaunchScheme launchScheme, [NotNullWhen(true)] out GameInstallPersistence? locker)
    {
        string gameDirectory = gameFileSystem.GetGameDirectory();
        Directory.CreateDirectory(gameDirectory);

        // If the directory is not empty
        if (Directory.EnumerateFileSystemEntries(gameDirectory).Any())
        {
            // we need to make sure config file exists and has our installing mark
            // Otherwise, we should prevent installation from proceeding
            if (!File.Exists(gameFileSystem.GetGameConfigurationFilePath()) || !GameConfiguration.Read(gameFileSystem, InstallingName))
            {
                locker = default;
                return false;
            }

            // If the mark exists, we can proceed
            locker = new(gameFileSystem);
            return true;
        }

        // Directory is empty, create config file with our installing mark
        GameConfiguration.Create(launchScheme, version, gameFileSystem.GetGameConfigurationFilePath());
        ImmutableArray<IniElement> elements = IniSerializer.DeserializeFromFile(gameFileSystem.GetGameConfigurationFilePath());
        IniSerializer.SerializeToFile(gameFileSystem.GetGameConfigurationFilePath(), elements.Add(new IniParameter(InstallingName, string.Empty)));

        locker = new(gameFileSystem);
        return true;
    }

    public void Release()
    {
        ImmutableArray<IniElement> elements = IniSerializer.DeserializeFromFile(gameFileSystem.GetGameConfigurationFilePath());
        IniSerializer.SerializeToFile(gameFileSystem.GetGameConfigurationFilePath(), elements.Remove(elements.Single(e => e is IniParameter { Key: InstallingName })));
    }
}