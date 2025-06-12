// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Win32;
using Snap.Hutao.Win32.Foundation;
using System.IO;

namespace Snap.Hutao.Service.Game.Configuration;

[Injection(InjectAs.Singleton, typeof(IGameConfigurationFileService))]
internal sealed class GameConfigurationFileService : IGameConfigurationFileService
{
    private const string BackupChineseConfigurationFileName = "config_cn.ini";
    private const string BackupOverseaConfigurationFileName = "config_oversea.ini";

    public void Backup(string source, bool isOversea)
    {
        if (File.Exists(source))
        {
            string serverCacheFolder = HutaoRuntime.GetDataFolderServerCacheFolder();
            string configFileName = isOversea ? BackupOverseaConfigurationFileName : BackupChineseConfigurationFileName;
            FileOperation.Copy(source, Path.Combine(serverCacheFolder, configFileName), true);
        }
    }

    public void Restore(string destination, bool isOversea)
    {
        string serverCacheFolder = HutaoRuntime.GetDataFolderServerCacheFolder();
        string source = Path.Combine(serverCacheFolder, isOversea ? BackupOverseaConfigurationFileName : BackupChineseConfigurationFileName);

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

        try
        {
            FileOperation.Copy(source, destination, true);
        }
        catch (IOException ex)
        {
            if (HutaoNative.IsWin32(ex.HResult, WIN32_ERROR.ERROR_NO_SUCH_DEVICE))
            {
                return;
            }

            throw;
        }
    }
}