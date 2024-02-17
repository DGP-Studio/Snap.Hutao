// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Service.Notification;

namespace Snap.Hutao.Service.Discord;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IDiscordService))]
internal sealed partial class DiscordService : IDiscordService, IDisposable
{
    private readonly IInfoBarService infoBarService;
    private readonly RuntimeOptions runtimeOptions;

    private bool isInitialized;

    public async ValueTask SetPlayingActivityAsync(bool isOversea)
    {
        if (CheckDiscordStatus())
        {
            _ = isOversea
                ? await DiscordController.SetPlayingGenshinImpactAsync().ConfigureAwait(false)
                : await DiscordController.SetPlayingYuanShenAsync().ConfigureAwait(false);
        }
    }

    public async ValueTask SetNormalActivityAsync()
    {
        if (CheckDiscordStatus())
        {
            _ = await DiscordController.SetDefaultActivityAsync(runtimeOptions.AppLaunchTime).ConfigureAwait(false);
        }
    }

    public void Dispose()
    {
        DiscordController.Stop();
    }

    private bool CheckDiscordStatus()
    {
        try
        {
            // Actually requires a discord client to be running on Windows platform.
            // If not, discord core creation code will throw.
            System.Diagnostics.Process[] discordProcesses = System.Diagnostics.Process.GetProcessesByName("Discord");

            if (discordProcesses.Length <= 0)
            {
                if (!isInitialized)
                {
                    infoBarService.Warning("Discord 未运行，将无法设置 Discord Activity 状态。");
                }

                return false;
            }

            foreach (System.Diagnostics.Process process in discordProcesses)
            {
                try
                {
                    _ = process.Handle;
                }
                catch (Win32Exception)
                {
                    if (!isInitialized)
                    {
                        infoBarService.Warning("权限不足，将无法设置 Discord Activity 状态。");
                    }

                    return false;
                }
            }

            return true;
        }
        finally
        {
            if (!isInitialized)
            {
                isInitialized = true;
            }
        }
    }
}