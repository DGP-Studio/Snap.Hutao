// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO.Ini;
using Snap.Hutao.Win32;
using Snap.Hutao.Win32.Foundation;
using System.IO;

namespace Snap.Hutao.Service.Game.Configuration;

internal static class GameScriptVersion
{
    public static bool Patch(string configFilePath, string scriptVersionFilePath)
    {
        if (!File.Exists(configFilePath))
        {
            return false;
        }

        try
        {
            string? version = default;
            foreach (IniElement element in IniSerializer.DeserializeFromFile(configFilePath))
            {
                if (element is IniParameter { Key: GameConstants.GameVersion } parameter)
                {
                    version = parameter.Value;
                    break;
                }
            }

            if (string.IsNullOrEmpty(version))
            {
                return false;
            }

            string? directory = Path.GetDirectoryName(scriptVersionFilePath);
            ArgumentNullException.ThrowIfNull(directory);
            Directory.CreateDirectory(directory);

            File.WriteAllText(scriptVersionFilePath, version);
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            // Access to the path '.*?' is denied.
            return false;
        }
        catch (IOException ex)
        {
            if (HutaoNative.IsWin32(ex.HResult, [WIN32_ERROR.ERROR_NO_SUCH_DEVICE, WIN32_ERROR.ERROR_DEVICE_HARDWARE_ERROR, WIN32_ERROR.ERROR_FILE_CORRUPT]))
            {
                return false;
            }

            throw;
        }
    }
}