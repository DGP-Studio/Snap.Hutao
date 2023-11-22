// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Discord.GameSDK;
using Snap.Discord.GameSDK.ABI;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Service.Discord;

internal static class DiscordController
{
    // https://discord.com/developers/applications
    private const long HutaoAppId = 1173950861647552623L;
    private const long YuanshenId = 1175743396028088370L;
    private const long GenshinImpactId = 1175747474384760962L;

    private static readonly WaitCallback RunDiscordRunCallbacks = DiscordRunCallbacks;
    private static readonly CancellationTokenSource StopTokenSource = new();
    private static readonly object SyncRoot = new();

    private static Snap.Discord.GameSDK.Discord? discordManager;
    private static bool isInitialized;

    public static async ValueTask<Result> SetDefaultActivityAsync(DateTimeOffset startTime)
    {
        ResetManagerOrIgnore(HutaoAppId);
        ActivityManager activityManager = discordManager.GetActivityManager();

        Activity activity = default;
        activity.Timestamps.Start = startTime.ToUnixTimeSeconds();
        activity.Assets.LargeImage = "icon";
        activity.Assets.LargeText = SH.AppName;

        return await activityManager.UpdateActivityAsync(activity).ConfigureAwait(false);
    }

    public static async ValueTask<Result> SetPlayingYuanShenAsync()
    {
        ResetManagerOrIgnore(YuanshenId);
        ActivityManager activityManager = discordManager.GetActivityManager();

        Activity activity = default;
        activity.State = SH.FormatServiceDiscordGameLaunchedBy(SH.AppName);
        activity.Details = SH.ServiceDiscordGameActivityDetails;
        activity.Timestamps.Start = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        activity.Assets.LargeImage = "icon";
        activity.Assets.LargeText = "原神";
        activity.Assets.SmallImage = "app";
        activity.Assets.SmallText = SH.AppName;

        return await activityManager.UpdateActivityAsync(activity).ConfigureAwait(false);
    }

    public static async ValueTask<Result> SetPlayingGenshinImpactAsync()
    {
        ResetManagerOrIgnore(GenshinImpactId);
        ActivityManager activityManager = discordManager.GetActivityManager();

        Activity activity = default;
        activity.State = SH.FormatServiceDiscordGameLaunchedBy(SH.AppName);
        activity.Details = SH.ServiceDiscordGameActivityDetails;
        activity.Timestamps.Start = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        activity.Assets.LargeImage = "icon";
        activity.Assets.LargeText = "Genshin Impact";
        activity.Assets.SmallImage = "app";
        activity.Assets.SmallText = SH.AppName;

        return await activityManager.UpdateActivityAsync(activity).ConfigureAwait(false);
    }

    public static void Stop()
    {
        if (!isInitialized)
        {
            return;
        }

        lock (SyncRoot)
        {
            StopTokenSource.Cancel();
            discordManager?.Dispose();
        }
    }

    [MemberNotNull(nameof(discordManager))]
    private static unsafe void ResetManagerOrIgnore(long clientId)
    {
        if (discordManager?.ClientId == clientId)
        {
            return;
        }

        lock (SyncRoot)
        {
            discordManager?.Dispose();
            discordManager = new(clientId, CreateFlags.NoRequireDiscord);
            discordManager.SetLogHook(Snap.Discord.GameSDK.LogLevel.Debug, SetLogHookHandler.Create(&DebugWriteDiscordMessage));
        }

        if (isInitialized)
        {
            return;
        }

        ThreadPool.UnsafeQueueUserWorkItem(RunDiscordRunCallbacks, StopTokenSource.Token);
        isInitialized = true;

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        static unsafe void DebugWriteDiscordMessage(Snap.Discord.GameSDK.LogLevel logLevel, byte* ptr)
        {
            ReadOnlySpan<byte> utf8 = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(ptr);
            string message = System.Text.Encoding.UTF8.GetString(utf8);
            System.Diagnostics.Debug.WriteLine($"[Discord.GameSDK]:[{logLevel}]:{message}");
        }
    }

    [SuppressMessage("", "SH007")]
    private static void DiscordRunCallbacks(object? state)
    {
        CancellationToken cancellationToken = (CancellationToken)state!;
        while (!cancellationToken.IsCancellationRequested)
        {
            lock (SyncRoot)
            {
                try
                {
                    discordManager?.RunCallbacks();
                }
                catch (SEHException ex)
                {
                    // Known error codes:
                    // 0x80004005 E_FAIL
                    System.Diagnostics.Debug.WriteLine($"[Discord.GameSDK]:[ERROR]:0x{ex.ErrorCode:X}");
                }
            }

            Thread.Sleep(100);
        }
    }
}