// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Core.Windowing.HotKey;

[SuppressMessage("", "CA1001")]
[ConstructorGenerated]
internal sealed partial class HotKeyController : IHotKeyController
{
    private static readonly WaitCallback RunMouseClickRepeatForever = MouseClickRepeatForever;

    private readonly object locker = new();

    private readonly HotKeyOptions hotKeyOptions;

    private volatile CancellationTokenSource? cancellationTokenSource;

    public void RegisterAll()
    {
        hotKeyOptions.MouseClickRepeatForeverKeyCombination.RegisterForCurrentWindow();
    }

    public void UnregisterAll()
    {
        hotKeyOptions.MouseClickRepeatForeverKeyCombination.UnregisterForCurrentWindow();
    }

    public void OnHotKeyPressed(in HotKeyParameter parameter)
    {
        if (parameter.Equals(hotKeyOptions.MouseClickRepeatForeverKeyCombination))
        {
            ToggleMouseClickRepeatForever();
        }
    }

    private static unsafe INPUT CreateInputForMouseEvent(MOUSE_EVENT_FLAGS flags)
    {
        INPUT input = new() { type = INPUT_TYPE.INPUT_MOUSE, };
        input.Anonymous.mi.dwFlags = flags;
        return input;
    }

    [SuppressMessage("", "SH007")]
    private static unsafe void MouseClickRepeatForever(object? state)
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

    private void ToggleMouseClickRepeatForever()
    {
        lock (locker)
        {
            if (hotKeyOptions.IsMouseClickRepeatForeverOn)
            {
                // Turn off
                cancellationTokenSource?.Cancel();
                cancellationTokenSource = default;
                hotKeyOptions.IsMouseClickRepeatForeverOn = false;
            }
            else
            {
                // Turn on
                cancellationTokenSource = new();
                ThreadPool.QueueUserWorkItem(RunMouseClickRepeatForever, cancellationTokenSource.Token);
                hotKeyOptions.IsMouseClickRepeatForeverOn = true;
            }
        }
    }
}