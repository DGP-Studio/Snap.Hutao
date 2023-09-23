// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Core.Windowing.HotKey;

[SuppressMessage("", "CA1001")]
internal sealed class HotKeyController : IHotKeyController
{
    private const int DefaultId = 100000;

    private readonly object locker = new();
    private readonly WaitCallback runMouseClickRepeatForever;
    private readonly HotKeyOptions hotKeyOptions;
    private readonly RuntimeOptions runtimeOptions;
    private volatile CancellationTokenSource? cancellationTokenSource;

    public HotKeyController(IServiceProvider serviceProvider)
    {
        hotKeyOptions = serviceProvider.GetRequiredService<HotKeyOptions>();
        runtimeOptions = serviceProvider.GetRequiredService<RuntimeOptions>();
        runMouseClickRepeatForever = MouseClickRepeatForever;
    }

    public bool Register(in HWND hwnd)
    {
        if (runtimeOptions.IsElevated)
        {
            return RegisterHotKey(hwnd, DefaultId, default, (uint)VIRTUAL_KEY.VK_F8);
        }

        return false;
    }

    public bool Unregister(in HWND hwnd)
    {
        if (runtimeOptions.IsElevated)
        {
            return UnregisterHotKey(hwnd, DefaultId);
        }

        return false;
    }

    public void OnHotKeyPressed(in HotKeyParameter parameter)
    {
        if (parameter is { Key: VIRTUAL_KEY.VK_F8, Modifier: 0 })
        {
            lock (locker)
            {
                if (hotKeyOptions.IsMouseClickRepeatForeverOn)
                {
                    cancellationTokenSource?.Cancel();
                    cancellationTokenSource = default;
                    hotKeyOptions.IsMouseClickRepeatForeverOn = false;
                }
                else
                {
                    cancellationTokenSource = new();
                    ThreadPool.QueueUserWorkItem(runMouseClickRepeatForever, cancellationTokenSource.Token);
                    hotKeyOptions.IsMouseClickRepeatForeverOn = true;
                }
            }
        }
    }

    private static unsafe INPUT CreateInputForMouseEvent(MOUSE_EVENT_FLAGS flags)
    {
        INPUT input = new() { type = INPUT_TYPE.INPUT_MOUSE, };
        input.Anonymous.mi.dwFlags = flags;
        return input;
    }

    [SuppressMessage("", "SH007")]
    private unsafe void MouseClickRepeatForever(object? state)
    {
        CancellationToken token = (CancellationToken)state!;

        // We want to use this thread for a long time
        while (!token.IsCancellationRequested)
        {
            INPUT[] inputs =
            {
                CreateInputForMouseEvent(MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTDOWN),
                CreateInputForMouseEvent(MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTUP),
            };

            if (SendInput(inputs.AsSpan(), sizeof(INPUT)) is 0)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetLastPInvokeError());
            }

            if (token.IsCancellationRequested)
            {
                return;
            }

            Thread.Sleep(Random.Shared.Next(100, 150));
        }
    }
}