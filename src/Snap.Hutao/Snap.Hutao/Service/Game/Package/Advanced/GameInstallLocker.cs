// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO.Ini;
using Snap.Hutao.Service.Game.Scheme;
using System.Collections.Immutable;
using System.IO;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Service.Game.Package.Advanced;

internal sealed class GameInstallLocker
{
    private readonly IGameFileSystem gameFileSystem;

    private GameInstallLocker(IGameFileSystem gameFileSystem)
    {
        this.gameFileSystem = gameFileSystem;
    }

    public static bool TryLock(IGameFileSystem gameFileSystem, string version, LaunchScheme launchScheme, [NotNullWhen(true)] out GameInstallLocker? locker)
    {
        if (Directory.EnumerateFileSystemEntries(gameFileSystem.GetGameDirectory()).Any())
        {
            if (!File.Exists(gameFileSystem.GetGameConfigurationFilePath()))
            {
                locker = default;
                return false;
            }

            ImmutableArray<IniElement> ini = IniSerializer.DeserializeFromFile(gameFileSystem.GetGameConfigurationFilePath());
            if (!ini.Any(e => e is IniParameter { Key: "snap_hutao_installing" }))
            {
                locker = default;
                return false;
            }

            locker = new(gameFileSystem);
            return true;
        }

        gameFileSystem.GenerateConfigurationFile(version, launchScheme);
        List<IniElement> ini2 = IniSerializer.DeserializeFromFile(gameFileSystem.GetGameConfigurationFilePath()).ToList();
        ini2.Add(new IniParameter("snap_hutao_installing", string.Empty));
        IniSerializer.SerializeToFile(gameFileSystem.GetGameConfigurationFilePath(), ini2);

        locker = new(gameFileSystem);
        return true;
    }

    public void Release()
    {
        List<IniElement> ini2 = IniSerializer.DeserializeFromFile(gameFileSystem.GetGameConfigurationFilePath()).ToList();
        IniParameter? installing = (IniParameter?)ini2.FirstOrDefault(e => e is IniParameter { Key: "snap_hutao_installing" });
        ArgumentNullException.ThrowIfNull(installing);
        ini2.Remove(installing);
        IniSerializer.SerializeToFile(gameFileSystem.GetGameConfigurationFilePath(), ini2);
    }
}