// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Model;
using Snap.Hutao.Win32.UI.Input.KeyboardAndMouse;
using System.Runtime.InteropServices;
using Windows.System;
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.UI.Input.HotKey;

[Injection(InjectAs.Singleton)]
internal sealed partial class HotKeyOptions : ObservableObject, IDisposable
{
    private static readonly WaitCallback RunMouseClickRepeatForever = MouseClickRepeatForever;

    private readonly object syncRoot = new();
    private readonly HotKeyMessageWindow hotKeyMessageWindow;

    private volatile CancellationTokenSource? cancellationTokenSource;

    private bool isDisposed;
    private HotKeyCombination mouseClickRepeatForeverKeyCombination;

    public HotKeyOptions(IServiceProvider serviceProvider)
    {
        hotKeyMessageWindow = new()
        {
            HotKeyPressed = OnHotKeyPressed,
        };

        mouseClickRepeatForeverKeyCombination = new(serviceProvider, hotKeyMessageWindow.HWND, SettingKeys.HotKeyMouseClickRepeatForever, 100000, default, VIRTUAL_KEY.VK_F8);
    }

    public List<NameValue<VIRTUAL_KEY>> VirtualKeys { get; } = Input.VirtualKeys.GetList();

    public HotKeyCombination MouseClickRepeatForeverKeyCombination
    {
        get => mouseClickRepeatForeverKeyCombination;
        set => SetProperty(ref mouseClickRepeatForeverKeyCombination, value);
    }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        isDisposed = true;

        MouseClickRepeatForeverKeyCombination.Unregister();

        hotKeyMessageWindow.Dispose();
        cancellationTokenSource?.Dispose();
    }

    public void RegisterAll()
    {
        MouseClickRepeatForeverKeyCombination.Register();
    }

    private static INPUT CreateInputForMouseEvent(MOUSE_EVENT_FLAGS flags)
    {
        INPUT input = default;
        input.type = INPUT_TYPE.INPUT_MOUSE;
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
            [
                CreateInputForMouseEvent(MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTDOWN),
                CreateInputForMouseEvent(MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTUP),
            ];

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

    private void OnHotKeyPressed(HotKeyParameter parameter)
    {
        if (parameter.Equals(MouseClickRepeatForeverKeyCombination))
        {
            ToggleMouseClickRepeatForever();
        }
    }

    private void ToggleMouseClickRepeatForever()
    {
        lock (syncRoot)
        {
            if (MouseClickRepeatForeverKeyCombination.IsOn)
            {
                // Turn off
                cancellationTokenSource?.Cancel();
                cancellationTokenSource = default;
                MouseClickRepeatForeverKeyCombination.IsOn = false;
            }
            else
            {
                // Turn on
                cancellationTokenSource = new();
                ThreadPool.QueueUserWorkItem(RunMouseClickRepeatForever, cancellationTokenSource.Token);
                MouseClickRepeatForeverKeyCombination.IsOn = true;
            }
        }
    }
}