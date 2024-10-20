// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Service.Notification;
using System.Diagnostics;

namespace Snap.Hutao.Service.Discord;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IDiscordService))]
internal sealed partial class DiscordService : IDiscordService, IDisposable
{
    private readonly IInfoBarService infoBarService;

    private bool isInitialized;

    public async ValueTask SetPlayingActivityAsync(bool isOversea)
    {
        if (IsSupported())
        {
            _ = isOversea
                ? await DiscordController.SetPlayingGenshinImpactAsync().ConfigureAwait(false)
                : await DiscordController.SetPlayingYuanShenAsync().ConfigureAwait(false);
        }
    }

    public async ValueTask SetNormalActivityAsync()
    {
        if (IsSupported())
        {
            _ = await DiscordController.SetDefaultActivityAsync(HutaoRuntime.LaunchTime).ConfigureAwait(false);
        }
    }

    public void Dispose()
    {
        DiscordController.Stop();
    }

    private bool IsSupported()
    {
        // Actually requires a discord client to be running on Windows platform.
        // If not, discord core creation code will throw.
        Process[] discordProcesses = Process.GetProcessesByName("Discord");

        if (discordProcesses.Length <= 0)
        {
            return false;
        }

        foreach (ref readonly Process process in discordProcesses.AsSpan())
        {
            try
            {
                if (string.Equals(process.MainWindowTitle, "Discord Updater", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                _ = process.Handle;
            }
            catch (Exception)
            {
                if (!isInitialized)
                {
                    isInitialized = true;
                    infoBarService.Warning(SH.ServiceDiscordActivityElevationRequiredHint);
                }

                return false;
            }
        }

        return true;
    }
}