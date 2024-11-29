// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.DeprecatedFile;
using System.IO;
using System.IO.Compression;

namespace Snap.Hutao.Service.Game.Package;

internal abstract class PackageConverter : IPackageConverter
{
    public abstract ValueTask<bool> EnsureGameResourceAsync(PackageConverterContext context);

    public virtual async ValueTask EnsureDeprecatedFilesAndSdkAsync(PackageConverterDeprecationContext context)
    {
        // Just try to delete these files, always download from server when needed
        string gameDirectory = context.GameFileSystem.GetGameDirectory();
        FileOperation.Delete(Path.Combine(gameDirectory, GameConstants.YuanShenData, "Plugins\\PCGameSDK.dll"));
        FileOperation.Delete(Path.Combine(gameDirectory, GameConstants.GenshinImpactData, "Plugins\\PCGameSDK.dll"));
        FileOperation.Delete(Path.Combine(gameDirectory, GameConstants.YuanShenData, "Plugins\\EOSSDK-Win64-Shipping.dll"));
        FileOperation.Delete(Path.Combine(gameDirectory, GameConstants.GenshinImpactData, "Plugins\\EOSSDK-Win64-Shipping.dll"));
        FileOperation.Delete(Path.Combine(gameDirectory, GameConstants.YuanShenData, "Plugins\\PluginEOSSDK.dll"));
        FileOperation.Delete(Path.Combine(gameDirectory, GameConstants.GenshinImpactData, "Plugins\\PluginEOSSDK.dll"));
        FileOperation.Delete(Path.Combine(gameDirectory, "sdk_pkg_version"));

        if (context.GameChannelSdk is not null)
        {
            using (Stream sdkWebStream = await context.HttpClient.GetStreamAsync(context.GameChannelSdk.ChannelSdkPackage.Url).ConfigureAwait(false))
            {
                ZipFile.ExtractToDirectory(sdkWebStream, gameDirectory, true);
            }
        }

        if (context.DeprecatedFiles is not null)
        {
            foreach (DeprecatedFile file in context.DeprecatedFiles.DeprecatedFiles)
            {
                string filePath = Path.Combine(gameDirectory, file.Name);
                FileOperation.Move(filePath, $"{filePath}.backup", true);
            }
        }
    }
}