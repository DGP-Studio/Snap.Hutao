// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Discord.GameSDK.ABI;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Unicode;

namespace Snap.Hutao.Service.Discord;

internal static class DiscordController
{
    // https://discord.com/developers/applications
    private const long HutaoAppId = 1173950861647552623L;
    private const long YuanshenId = 1175743396028088370L;
    private const long GenshinImpactId = 1175747474384760962L;

    private static readonly CancellationTokenSource StopTokenSource = new();
    private static readonly object SyncRoot = new();

    private static long currentClientId;
    private static unsafe IDiscordCore* discordCorePtr;
    private static bool isInitialized;

    public static async ValueTask<DiscordResult> SetDefaultActivityAsync(DateTimeOffset startTime)
    {
        await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);
        return SetDefaultActivity(startTime);

        static unsafe DiscordResult SetDefaultActivity(in DateTimeOffset startTime)
        {
            ResetManagerOrIgnore(HutaoAppId);

            if (discordCorePtr is null)
            {
                return DiscordResult.Ok;
            }

            IDiscordActivityManager* activityManagerPtr = discordCorePtr->get_activity_manager(discordCorePtr);

            DiscordActivity activity = default;
            activity.timestamps.start = startTime.ToUnixTimeSeconds();
            SetString(activity.assets.large_image, 128, "icon"u8);
            SetString(activity.assets.large_text, 128, SH.AppName);

            return new DiscordUpdateActivityAsyncAction(activityManagerPtr).WaitUpdateActivity(activity);
        }
    }

    public static async ValueTask<DiscordResult> SetPlayingYuanShenAsync()
    {
        await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);
        return SetPlayingYuanShen();

        static unsafe DiscordResult SetPlayingYuanShen()
        {
            ResetManagerOrIgnore(YuanshenId);

            if (discordCorePtr is null)
            {
                return DiscordResult.Ok;
            }

            IDiscordActivityManager* activityManagerPtr = discordCorePtr->get_activity_manager(discordCorePtr);

            DiscordActivity activity = default;
            SetString(activity.state, 128, SH.FormatServiceDiscordGameLaunchedBy(SH.AppName));
            SetString(activity.details, 128, SH.ServiceDiscordGameActivityDetails);
            activity.timestamps.start = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            SetString(activity.assets.large_image, 128, "icon"u8);
            SetString(activity.assets.large_text, 128, "原神"u8);
            SetString(activity.assets.small_image, 128, "app"u8);
            SetString(activity.assets.small_text, 128, SH.AppName);

            return new DiscordUpdateActivityAsyncAction(activityManagerPtr).WaitUpdateActivity(activity);
        }
    }

    public static async ValueTask<DiscordResult> SetPlayingGenshinImpactAsync()
    {
        await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);
        return SetPlayingGenshinImpact();

        static unsafe DiscordResult SetPlayingGenshinImpact()
        {
            ResetManagerOrIgnore(GenshinImpactId);

            if (discordCorePtr is null)
            {
                return DiscordResult.Ok;
            }

            IDiscordActivityManager* activityManagerPtr = discordCorePtr->get_activity_manager(discordCorePtr);

            DiscordActivity activity = default;
            SetString(activity.state, 128, SH.FormatServiceDiscordGameLaunchedBy(SH.AppName));
            SetString(activity.details, 128, SH.ServiceDiscordGameActivityDetails);
            activity.timestamps.start = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            SetString(activity.assets.large_image, 128, "icon"u8);
            SetString(activity.assets.large_text, 128, "Genshin Impact"u8);
            SetString(activity.assets.small_image, 128, "app"u8);
            SetString(activity.assets.small_text, 128, SH.AppName);

            return new DiscordUpdateActivityAsyncAction(activityManagerPtr).WaitUpdateActivity(activity);
        }
    }

    public static unsafe void Stop()
    {
        if (!isInitialized)
        {
            return;
        }

        lock (SyncRoot)
        {
            StopTokenSource.Cancel();
            try
            {
                discordCorePtr = default;
            }
            catch (SEHException)
            {
            }
        }
    }

    private static unsafe void ResetManagerOrIgnore(long clientId)
    {
        if (currentClientId == clientId)
        {
            return;
        }

        // Actually requires a discord client to be running on Windows platform.
        // If not, the following creation code will throw.
        System.Diagnostics.Process[] discordProcesses = System.Diagnostics.Process.GetProcessesByName("Discord");

        if (discordProcesses.Length <= 0)
        {
            return;
        }

        foreach (System.Diagnostics.Process process in discordProcesses)
        {
            try
            {
                _ = process.Handle;
            }
            catch (Win32Exception)
            {
                return;
            }
        }

        lock (SyncRoot)
        {
            DiscordCreateParams @params = default;
            Methods.DiscordCreateParamsSetDefault(&@params);
            @params.client_id = clientId;
            @params.flags = (uint)DiscordCreateFlags.Default;
            IDiscordCore* ptr = default;
            Methods.DiscordCreate(3, &@params, &ptr);

            currentClientId = clientId;
            discordCorePtr = ptr;
            discordCorePtr->set_log_hook(discordCorePtr, DiscordLogLevel.Debug, default, &DebugWriteDiscordMessage);
        }

        if (isInitialized)
        {
            return;
        }

        DiscordRunCallbacksAsync(StopTokenSource.Token).SafeForget();
        isInitialized = true;

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        static unsafe void DebugWriteDiscordMessage(void* state, DiscordLogLevel logLevel, sbyte* ptr)
        {
            ReadOnlySpan<byte> utf8 = MemoryMarshal.CreateReadOnlySpanFromNullTerminated((byte*)ptr);
            string message = System.Text.Encoding.UTF8.GetString(utf8);
            System.Diagnostics.Debug.WriteLine($"[Discord.GameSDK]:[{logLevel}]:{message}");
        }
    }

    private static async ValueTask DiscordRunCallbacksAsync(CancellationToken cancellationToken)
    {
        int notRunningCounter = 0;

        using (PeriodicTimer timer = new(TimeSpan.FromMilliseconds(500)))
        {
            try
            {
                while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    lock (SyncRoot)
                    {
                        try
                        {
                            DiscordResult result = DiscordCoreRunRunCallbacks();
                            if (result is not DiscordResult.Ok)
                            {
                                if (result is DiscordResult.NotRunning)
                                {
                                    if (++notRunningCounter > 20)
                                    {
                                        Stop();
                                    }
                                }
                                else
                                {
                                    notRunningCounter = 0;
                                    System.Diagnostics.Debug.WriteLine($"[Discord.GameSDK ERROR]:{result:D} {result}");
                                }
                            }
                        }
                        catch (SEHException ex)
                        {
                            // Known error codes:
                            // 0x80004005 E_FAIL
                            System.Diagnostics.Debug.WriteLine($"[Discord.GameSDK ERROR]:0x{ex.ErrorCode:X}");
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        unsafe DiscordResult DiscordCoreRunRunCallbacks()
        {
            if (discordCorePtr is not null)
            {
                return discordCorePtr->run_callbacks(discordCorePtr);
            }

            return DiscordResult.Ok;
        }
    }

    private static unsafe void SetString(sbyte* reference, int length, string source)
    {
        Span<sbyte> sbytes = new(reference, length);
        sbytes.Clear();
        Utf8.FromUtf16(source.AsSpan(), MemoryMarshal.Cast<sbyte, byte>(sbytes), out _, out _);
    }

    private static unsafe void SetString(sbyte* reference, int length, in ReadOnlySpan<byte> source)
    {
        Span<sbyte> sbytes = new(reference, length);
        sbytes.Clear();
        source.CopyTo(MemoryMarshal.Cast<sbyte, byte>(sbytes));
    }

    private struct DiscordAsyncAction
    {
        public DiscordResult Result;
        public bool IsCompleted;
    }

    private unsafe struct DiscordUpdateActivityAsyncAction
    {
        private readonly IDiscordActivityManager* activityManagerPtr;
        private DiscordAsyncAction discordAsyncAction;

        public DiscordUpdateActivityAsyncAction(IDiscordActivityManager* activityManagerPtr)
        {
            this.activityManagerPtr = activityManagerPtr;
        }

        public DiscordResult WaitUpdateActivity(DiscordActivity activity)
        {
            fixed (DiscordAsyncAction* actionPtr = &discordAsyncAction)
            {
                activityManagerPtr->update_activity(activityManagerPtr, &activity, actionPtr, &HandleResult);
            }

            SpinWaitPolyfill.SpinUntil(ref discordAsyncAction, &CheckActionCompleted);
            return discordAsyncAction.Result;
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        private static void HandleResult(void* state, DiscordResult result)
        {
            DiscordAsyncAction* action = (DiscordAsyncAction*)state;
            action->Result = result;
            action->IsCompleted = true;
        }

        private static bool CheckActionCompleted(ref readonly DiscordAsyncAction state)
        {
            return state.IsCompleted;
        }
    }
}